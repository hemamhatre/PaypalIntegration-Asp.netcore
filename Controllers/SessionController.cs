using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace ChatValues.Web.Controllers
{
    public class SessionController : BaseController
    {
        private IHostingEnvironment hostingEnv;

        public SessionController(ChatValuesContext context, IHostingEnvironment env) : base(context)
        {
            this.hostingEnv = env;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        // GET: /<controller>/
        public IActionResult Members(int id)
        {
             var userClaim = (ClaimsIdentity)User.Identity;
            var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
            ViewBag.LoggedInUserID = userIdVal.Value;

            var userdata = _context.Users.FirstOrDefault<User>(x => x.ID == int.Parse(userIdVal.Value));

            var sessionItem = _context.Sessions.FirstOrDefault<Session>(se => se.ID == id);
            var members = _context.UserSessions.Where(m => m.SessionID == id).OrderByDescending(x => x.User.CreateOn).OrderByDescending(x => x.User.Country == userdata.Country).ToList();
            MemberView memberView = new MemberView();
            List<UserJobCard> userJobcards = new List<UserJobCard>();
            foreach (var member in members)
            {
                var jobCard = _context.JobCards.Where(j => j.OwnerID == member.UserID).ToList();
                var user = _context.Users.FirstOrDefault<User>(u => u.ID == member.UserID);
                ViewBag.AgeString = Age(user.DateOfBirth);
                if (jobCard.Count > 0)
                {                    
                    userJobcards.Add(new UserJobCard() { JobCard = jobCard[0], User = user, SessionName = sessionItem.Title });
                    memberView.UserJobCards = userJobcards;
                }
                else
                {
                    userJobcards.Add(new UserJobCard() { JobCard = null, User = user, SessionName = sessionItem.Title });
                    memberView.UserJobCards = userJobcards;
                }
            }
            memberView.Users = _context.Users.ToList();
            var dirPath = @"\images\Profile\";
            ViewBag.ProfilePicPath = hostingEnv.WebRootPath + $@"{dirPath}";
            return View(memberView);
        }

        public string Age(DateTime birthday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - birthday.Year;
            if (now < birthday.AddYears(age)) age--;

            return age.ToString();
        }
    }
}
