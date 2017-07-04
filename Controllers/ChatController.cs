using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using System.IO;
using Microsoft.Net.Http.Headers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatValues.Web.Controllers
{
    public class ChatController : BaseController
    {
        private IHostingEnvironment hostingEnv;

        public ChatController(ChatValuesContext context, IHostingEnvironment env) : base(context)
        {
            this.hostingEnv = env;
        }

        // GET: /<controller>/
        public IActionResult AudioMessage(int id)
        {
            AudioMessageView vi = new AudioMessageView();
            vi.FromUserID = 0;
            return View(vi);
        }

        // POST: 
        [HttpPost]
        public IActionResult UploadAudioMessage(object objReq)
        {
            ViewBag.AudioMessageSuccess = true;
            var userClaim = (ClaimsIdentity)User.Identity;
            AudioMessageRequest req = JsonConvert.DeserializeObject<AudioMessageRequest>(objReq.ToString());
            var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
            
            return View();
        }
    }
}
