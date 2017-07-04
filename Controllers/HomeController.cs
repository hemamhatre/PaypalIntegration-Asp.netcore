using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatValues.Web.Models;
using System.Security.Claims;

namespace ChatValues.Web.Controllers
{
	public class HomeController : BaseController
	{
		public HomeController(ChatValuesContext context) : base(context)
		{
		}

		public IActionResult Index([FromQuery]int errorCode)
		{
			switch(errorCode)
			{
				case 1:
					ViewBag.ErrorMessage = Constants.LoginError;
					break;
				case 2:
					ViewBag.ErrorMessage = Constants.ConfirmEmailError;
					break;
				case 3:
					ViewBag.ErrorMessage = Constants.AccountDisabledError;
					break;

			}

			return View();
		}

		public IActionResult Sessions(int id)
		{
			List<UserSessionItem> userSessionMapping = new List<UserSessionItem>();
			if (id == 0)
			{
				var sessionModel = _context.Sessions.ToList<Session>().OrderBy(x=>x.Title);
				foreach (var sessionVar in sessionModel)
				{
					var members = _context.UserSessions.Where(us => us.Session.ID == sessionVar.ID).ToList();

                    userSessionMapping.Add(new UserSessionItem() { Session = sessionVar, MemberCount = members.Count, Categories = null });
				}
				ViewBag.DisplaySessions = true;
				return View(userSessionMapping);
			}
			else
			{
				var sessionItem = _context.Sessions.FirstOrDefault<Session>(se => se.ID == id);
				var categories = _context.Categories.Where(cat => cat.Session.ID == sessionItem.ID).ToList();
				userSessionMapping.Add(new UserSessionItem() { Session = sessionItem, Categories = categories });
				ViewBag.DisplaySessions = false;
				return View(userSessionMapping);
			}
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
