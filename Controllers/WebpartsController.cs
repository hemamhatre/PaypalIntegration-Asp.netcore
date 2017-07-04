using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatValues.Web.Controllers
{
    public class OnlineUserWP : ViewComponent
    {
        private IHostingEnvironment hostingEnv;

        public ChatValuesContext _context;

        public OnlineUserWP(ChatValuesContext context, IHostingEnvironment env)
        {
            this.hostingEnv = env;
            this._context = context;
        }

        // GET: /<controller>/
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var olUsers = await GetItemsAsync();
            //ViewBag.OnlineUsers = olUsers;
            List<OnlineUser> olItems = new List<OnlineUser>();
            
            foreach (var olUser in olUsers)
            {
                var user = _context.Users.FirstOrDefault<User>(u => u.ID == olUser.UserID);
                olItems.Add(new OnlineUser() { ID = olUser.ID, User = user, UserID = user.ID });
            }
            var dirPath = @"\images\Profile\";
            ViewBag.ProfilePicPath = hostingEnv.WebRootPath + $@"{dirPath}";
            return View(olItems);
        }

        private Task<List<OnlineUser>> GetItemsAsync()
        {
            
            
            return _context.OnlineUsers.ToListAsync<OnlineUser>(); 
        }
    }
}
