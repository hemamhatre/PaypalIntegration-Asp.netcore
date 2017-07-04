using ChatValues.EmailClient;
using ChatValues.EmailClient.Models;
using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ChatValues.Web.Controllers
{
	public class MessageController : BaseController
	{
		private IHostingEnvironment hostingEnv;
		private EmailManager emailManager;


		public MessageController(ChatValuesContext context, IHostingEnvironment env, EmailManager emailManager) : base(context)
		{
			this.hostingEnv = env;
			this.emailManager = emailManager;
		}

		// GET: /<controller>/
		public IActionResult Index()
		{
			List<Message> messagesList = new List<Message>();
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			if (userIdVal != null)
			{
				ViewBag.ToUser = _context.Users.FirstOrDefault<User>(u => u.ID == int.Parse(userIdVal.Value));

				var messages = _context.Messages.Where(m => m.ToUserID == int.Parse(userIdVal.Value)).ToList();
				foreach (var message in messages)
				{
					var user = _context.Users.FirstOrDefault<User>(u => u.ID == message.FromUserID);
					message.FromUser = user;
					messagesList.Add(message);
				}
			}
			return View(messagesList);
		}

		[HttpPost]
		public IActionResult SendMessage()
		{
			var messageBody = Request.Form["txtSendMessage"];
			var toUserId = Request.Form["hdnToUserID"];
			var fromUserId = Request.Form["hdnFromUserID"];

			Message message = new Message();
			message.FromUserID = int.Parse(fromUserId);
			message.ToUserID = int.Parse(toUserId);
			message.MessageBody = messageBody;
			message.SentOn = DateTime.Now;

			var dummy = _context.Messages.ToList();
			if (dummy != null && dummy.Count > 0)
			{
				message.ID = dummy.Last<Message>().ID + 1;
			}
			else
			{
				message.ID = 1;
			}
			_context.Messages.Add(message);
			_context.SaveChanges();


			#region Send Email 

			var ToUserData = _context.Users.FirstOrDefault<User>(u => u.ID == message.ToUserID);

			var SenderUserData = _context.Users.FirstOrDefault<User>(u => u.ID == message.FromUserID);

			MailRecipient mailRecipient = new MailRecipient();
			mailRecipient.Name = ToUserData.Name;
			mailRecipient.Email = ToUserData.Email;

			Dictionary<string, string> templatePlaceholder = new Dictionary<string, string>();
			templatePlaceholder.Add("UserName", ToUserData.Name);
			templatePlaceholder.Add("Senderid", SenderUserData.ID.ToString());
			templatePlaceholder.Add("SenderName", SenderUserData.Name);
			templatePlaceholder.Add("MessageBody", messageBody);

			emailManager.SendEmail(mailRecipient, EmailTemplate.Invitation, templatePlaceholder);

			#endregion

			return RedirectToAction("UserProfile", "Account", new { id = message.ToUserID });
		}
	}
}
