using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatValues.Web.Controllers
{
    public class ContactCentreController : BaseController
    {
        private IHostingEnvironment hostingEnv;

        public ContactCentreController(ChatValuesContext context, IHostingEnvironment env) : base(context)
        {
            this.hostingEnv = env;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var userClaim = (ClaimsIdentity)User.Identity;
            var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);

            ContactCenter cc = new ContactCenter();
            var recUserIds = _context.Recommendations.Where(rec => rec.RecommendedBy == int.Parse(userIdVal.Value)).ToList<Recommend>().GroupBy(x => new { x.UserID }).Select(g => g.First()); 
            var users = from user in _context.Users
                        join recUser in recUserIds on user.ID equals recUser.UserID
                        select user;
            cc.RecommendedUsersByMe = users.ToList<User>();

            var userRecIds = _context.Recommendations.Where(rec => rec.UserID == int.Parse(userIdVal.Value)).ToList<Recommend>().GroupBy(x => new { x.RecommendedBy }).Select(g => g.First());
            var usersRecomMe = from user in _context.Users
                        join recUser in userRecIds on user.ID equals recUser.RecommendedBy
                        select user;

            cc.UsersRecommendedMe = usersRecomMe.ToList<User>();

            var favUserIds = _context.Favourites.Where(fav => fav.FavouriteBy == int.Parse(userIdVal.Value)).ToList<Favourite>().GroupBy(x => new { x.UserID }).Select(g => g.First());
            var favUsers = from user in _context.Users
                               join favUser in favUserIds on user.ID equals favUser.UserID
                               select user;

            cc.FavouriteUsers = favUsers.ToList<User>();

            var blockUserIds = _context.BlockedUsers.Where(bl => bl.BlockedBy == int.Parse(userIdVal.Value)).ToList<BlockedUser>().GroupBy(x => new { x.UserID }).Select(g => g.First());
            var blockUsers = from user in _context.Users
                           join blockUser in blockUserIds on user.ID equals blockUser.UserID
                           select user;

            cc.BlockedUsers = blockUsers.ToList<User>();


            return View(cc);
        }
    }
}
