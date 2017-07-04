using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatValues.Web.Models;

namespace ChatValues.Web.Controllers
{
    public class BaseController : Controller
    {
        public ChatValuesContext _context;

        public bool IsLoggedInUser; 

        public BaseController(ChatValuesContext context)
        {
            _context = context;
            if (User != null)
            {
                var userClaim = (ClaimsIdentity)User.Identity;
                var userIdVal = userClaim.Claims.Where<Claim>(c => c.Type == ClaimTypes.NameIdentifier).ToList();
                if (userIdVal != null)
                {
                    IsLoggedInUser = true;
                }
                else
                {
                    IsLoggedInUser = false;
                }
            }
        }
    }
}
