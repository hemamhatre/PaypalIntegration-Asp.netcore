using ChatValues.EmailClient;
using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ChatValues.Web.Controllers
{
	public class JobCentreController : BaseController
    {

        public JobCentreController(ChatValuesContext context) : base(context)
        {
        }

        // GET: /<controller>/
        public IActionResult Index()
        {

            var userClaim = (ClaimsIdentity)User.Identity;
            var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
            JobCentre jobCentre = new JobCentre();

            var soldDeals = _context.JobCardDeals.Include(i => i.BoughtByUser).Include(i => i.SoldByUser).Include(i => i.JobCard).Where(j => j.SoldBy == int.Parse(userIdVal.Value)).ToList();
            ViewBag.SoldDealsInProgressCount = soldDeals.Count<JobCardDeals>(s => s.Status == "In process");

            var boughtDeals = _context.JobCardDeals.Include(i => i.BoughtByUser).Include(i => i.SoldByUser).Include(i => i.JobCard).Where(j => j.BoughtBy == int.Parse(userIdVal.Value)).ToList();
            ViewBag.BoughtDealsInProgressCount = boughtDeals.Count<JobCardDeals>(b => b.Status == "In process");

            var recommendation = _context.Recommendations.Include(u => u.UserIDs).Where(c => c.UserID == int.Parse(userIdVal.Value)).ToList();
            ViewBag.RecommendationCount = recommendation.Count<Recommend>(t => t.UserID == int.Parse(userIdVal.Value));

            var jobCardId = _context.JobCards.FirstOrDefault<JobCard>(u => u.OwnerID == int.Parse(userIdVal.Value));
            ViewBag.ViewJobCount = ViewBag.ViewJobCount = _context.JobCardViews.Count(t => t.JobCard.ID == jobCardId.ID);

            jobCentre.Sold = soldDeals;
            jobCentre.Bought = boughtDeals;


            return View(jobCentre);
        }

        public IActionResult Redeem()
        {
            return View();
        }

        public IActionResult Vouchers()
        {
            return View();
        }

        public IActionResult BuyVouchers(Voucher voucher)
        {
            //voucher.CreateOn = DateTime.Now;
            //voucher.TransactionDate = DateTime.Now;
            //voucher.ModifiedOn = DateTime.Now;
            Transaction transaction = new Transaction();

            var userClaim = (ClaimsIdentity)User.Identity;
            var userEmailVal = userClaim.Claims.Where<Claim>(c => c.Type == ClaimTypes.Email).ToList();
            if (userEmailVal.Count > 0)
            {
                transaction.OwnerID = _context.Users.FirstOrDefault<User>(u => u.Email == userEmailVal[0].Value).ID;
                transaction.Voucher = voucher.Quantity;
                transaction.Amount = voucher.Quantity * 5;
                transaction.TransactionDate = DateTime.Now;
                transaction.CreateOn = DateTime.Now;
                transaction.ModifiedOn = DateTime.Now;
                transaction.Status = 1;
                _context.Transaction.Add(transaction);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("TransactionID", transaction.ID);
                HttpContext.Session.SetString("VoucherQuantity", transaction.Voucher.ToString());
                return RedirectToAction("PayPal");
            }
            ViewBag.ErrorMessage = "Couldn't buy vouchers. Please try again after sometime.";
            return View("Index");
        }
        public IActionResult PendingVouchers()
        {

            // var item = _context.GrantedVouchers.Where<GrantedVouchers>(x => x.Status == GrantStatus.Pending);
            var item = (from t in _context.Users
                        join t1 in _context.GrantedVouchers on t.ID equals t1.GrantedByID
                        where t1.Status == GrantStatus.Pending
                        select new GrantedVouchers
                        {
                            grantusername = _context.Users.FirstOrDefault<User>(x => x.ID == t1.GrantedToID).Name,
                            Name = t.Name,
                            ID = t1.ID,
                            Vouchers = t1.Vouchers,
                            GrantedToID = t1.GrantedToID,
                            GrantedOn = t1.GrantedOn,
                            GrantedByID = t1.GrantedByID,
                        }).ToList();

            return View(item);

        }

        [HttpPost]
        public IActionResult CancelPendingVouchers()
        {

            var Reason = Request.Form["txtreason"];
            var toUserId = Request.Form["hdnToUserID"];


            var grantvouchers = _context.GrantedVouchers.FirstOrDefault<GrantedVouchers>(x => x.ID == int.Parse(toUserId));

            grantvouchers.CancellationReason = (string)Reason;
            grantvouchers.CancelledOn = DateTime.Now;
            grantvouchers.Status = GrantStatus.Cancelled;

            _context.SaveChanges();

            return RedirectToAction("PendingVouchers", "JobCentre");
        }


        public IActionResult GrantVouchers(Voucher voucher)
        {
            try
            {

                var userClaim = (ClaimsIdentity)User.Identity;
                var userEmailVal = userClaim.Claims.Where<Claim>(c => c.Type == ClaimTypes.Email).ToList();

                if (userEmailVal.Count > 0)
                {

                    voucher.OwnerId = _context.Users.FirstOrDefault<User>(u => u.Email == userEmailVal[0].Value).ID;

                    String GrantUserName = Request.Form["SrchUser"].ToString();

                    var grantuser = _context.Users.FirstOrDefault<User>(u => u.Name == GrantUserName);
                    if (grantuser != null)
                    {
                        if (CheckGrantStatus(voucher.OwnerId, voucher.Quantity))
                        {

                            GrantedVouchers grantVoucher = new GrantedVouchers();

                            grantVoucher.GrantedByID = voucher.OwnerId;

                            grantVoucher.GrantedToID = grantuser.ID;

                            grantVoucher.GrantedOn = DateTime.Now;

                            grantVoucher.Status = GrantStatus.Pending;

                            grantVoucher.Vouchers = voucher.Quantity;


                            _context.GrantedVouchers.Add(grantVoucher);


                            //Deduct Voucher
                            _context.Add(new Voucher() { Quantity = -1 * voucher.Quantity, CreateOn = DateTime.Now, ModifiedOn = DateTime.Now, TransactionDate = DateTime.Now, OwnerId = voucher.OwnerId });

                            _context.SaveChanges();

                            ViewBag.Message = "Request submitted successfully for Granting your vouchers and is Pending For Approval";
                        }
                        else
                        {
                            ViewBag.Message = "You Dont Have Sufficient Voucher Balance To Transfer";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Requested user is not present in portal";
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("Vouchers");
        }


        public Boolean CheckGrantStatus(int UserId, int Qty)
        {
            var credit = _context.Vouchers.Where(u => u.OwnerId == UserId).Sum<Voucher>(u => u.Quantity);
            return credit >= Qty;
        }

        public IEnumerable<User> GetUser(String searchvalue)
        {

            var userClaim = (ClaimsIdentity)User.Identity;

            var userEmailVal = userClaim.Claims.Where<Claim>(c => c.Type == ClaimTypes.Email).ToList();

            var userdtls = _context.Users.FirstOrDefault<User>(u => u.Email == userEmailVal[0].Value);


            var getuser = from t in _context.Users
                          where t.ID != userdtls.ID
                          where t.Name.StartsWith(searchvalue)
                          select new User
                          {
                              Name = t.Name,
                              ID = t.ID
                          };

            return getuser;
        }

        [HttpGet]
        public JsonResult Details(string Prefix)
        {
            return Json(GetUser(Prefix));
        }


        public IActionResult RedeemVouchers(Voucher voucher)
        {
            VoucherRedeem voucherRedeem = new VoucherRedeem();
            voucherRedeem.CreatedOn = DateTime.Now;
            voucherRedeem.RedeemedOn = DateTime.Now;
            voucherRedeem.ModifiedOn = DateTime.Now;
            voucherRedeem.QuantityRedeemed = voucher.Quantity;
            _context.VoucerRedeems.Add(voucherRedeem);
            _context.SaveChanges();
            return View("Redeem");
        }

        public IActionResult MarkJobCardComplete(int id)
        {
            var jd = _context.JobCardDeals.FirstOrDefault<JobCardDeals>(j => j.ID == id);
            jd.Status = "Completed";
            jd.ClosedOn = DateTime.Now;
            _context.JobCardDeals.Update(jd);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult PayPal()
        {
            return RedirectToAction("Index", "PayPal");
        }
    }
}
