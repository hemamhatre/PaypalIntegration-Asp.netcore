using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Controllers
{
    public class InboxController: BaseController
    {
        private IHostingEnvironment hostingEnv;

        public InboxController(ChatValuesContext context, IHostingEnvironment env) : base(context)
        {
            this.hostingEnv = env;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
