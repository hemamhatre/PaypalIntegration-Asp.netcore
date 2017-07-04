using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatValues.Web.Models;
using ChatValues.PayPal;
using Microsoft.AspNetCore.Http;

namespace ChatValues.Web.Controllers
{
    public class PaymentController: BaseController
    {
        public PaymentController(ChatValuesContext context) : base(context)
        {
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            //PayPalManager payPalManager = new PayPalManager();
            //var payPalbutton = payPalManager.GenerateButton(TransactionType.PurchaseVoucer, VoucherAmount);
            //HttpContext.Session.SetString("payPalbutton", payPalbutton);
            //HttpContext.Session.SetString("VoucherAmount", VoucherAmount.ToString());
            //ViewBag.VoucherAmount = HttpContext.Session.GetString("VoucherAmount"); ;
            //ViewBag.PaypalButton = HttpContext.Session.GetString("payPalbutton");
            return View();
        }

    }
}
