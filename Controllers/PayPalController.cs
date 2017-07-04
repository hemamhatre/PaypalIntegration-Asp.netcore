using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ChatValues.PayPal;
using ChatValues.Web.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatValues.Web.Controllers
{
    public class PayPalController : BaseController
    {


        public PayPalController(ChatValuesContext context) : base(context)
        {
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.VoucherQuantity = HttpContext.Session.GetString("VoucherQuantity");
            int? transactionID = HttpContext.Session.GetInt32("TransactionID");
            //By Default Value
            ViewBag.TotalAmount = "$" + 5.00;
            PayPalManager payPalManager = new PayPalManager();
            var payPalbutton = payPalManager.GenerateButton(TransactionType.PurchaseVoucer, (decimal)5.00, Convert.ToInt32(ViewBag.VoucherQuantity), transactionID);
            ViewBag.PaypalButton = payPalbutton;
            return View();
        }

        [HttpPost]
        public IActionResult IPN()
        {
            return Ok();
        }

        [HttpGet("paypal/purchase/complete/{transactionID:int}")]
        public IActionResult BoughtVoucher(int transactionID)
        {
            Voucher voucher = new Voucher();
            var completeTransaction = _context.Transaction.FirstOrDefault<Transaction>(u => u.ID == transactionID);
            voucher.CreateOn = DateTime.Now;
            voucher.TransactionDate = DateTime.Now;
            voucher.ModifiedOn = DateTime.Now;
            voucher.OwnerId = completeTransaction.OwnerID;
            voucher.CreditorId = completeTransaction.OwnerID;
            voucher.Quantity = completeTransaction.Voucher;
            voucher.TransactionTypeId = 7;//Completed
            _context.Vouchers.Add(voucher);
            _context.SaveChanges();
           
            return Ok();
        }

        [HttpGet("paypal/purchase/cancel/{transactionID:int}")]
        public IActionResult Cancel(int transactionID)
        {
            var cancelledTransaction = _context.Transaction.FirstOrDefault<Transaction>(u => u.ID == transactionID);
            cancelledTransaction.ModifiedOn = DateTime.Now;
            cancelledTransaction.Status = 8;//Cancelled Status as Transaction table
            _context.Entry(cancelledTransaction).State = EntityState.Modified;
            _context.SaveChanges();
            ViewBag.ErrorMessage = "Oops!The transaction has failed.Please click here to continue";//Which URL needs to be there on clikc here
            return Content(ViewBag.ErrorMessage);
        }

        public IActionResult ConfirmPurchase(int quantity, decimal amount)
        {
            int? transactionID = HttpContext.Session.GetInt32("TransactionID");
            ViewBag.VoucherQuantity = HttpContext.Session.GetString("VoucherQuantity");
            PayPalManager payPalManager = new PayPalManager();
            var payPalbutton = payPalManager.GenerateButton(TransactionType.PurchaseVoucer, amount, quantity, transactionID);
            ViewBag.PaypalButton = payPalbutton;
            ViewBag.VoucherQuantity = quantity;
            ViewBag.TotalAmount = amount;
            return View("Index");
        }
    }
}
