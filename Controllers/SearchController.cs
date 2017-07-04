using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;

namespace ChatValues.Web.Controllers
{
	public class SearchController : BaseController
	{
		private IHostingEnvironment hostingEnv;

		public SearchController(ChatValuesContext context, IHostingEnvironment env) : base(context)
		{
			this.hostingEnv = env;
		}

		// GET: /<controller>/
		public IActionResult Index()
		{
			var searchString = Request.Form["searchTextBox"];
			SearchResultView resultItems = new SearchResultView();

			var _users = from u in _context.Users
						 where u.Name.Contains(searchString) || u.City.Contains(searchString)
						 || u.Country.Contains(searchString) || _context.UserSessions.Any(us => us.UserID == u.ID && us.Session.Title.Contains(searchString) == true)
						 || _context.JobCards.Any(j=> j.Achievements.Contains(searchString) || j.Benefits.Contains(searchString) || j.Deliverables.Contains(searchString)  || j.Description.Contains(searchString) || j.Requirements.Contains(searchString) || j.Title.Contains(searchString))
						 select u;

			resultItems.Users = _users.Distinct().ToList();
			return View(resultItems);
		}
	}
}
