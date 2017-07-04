using ChatValues.EmailClient;
using ChatValues.EmailClient.Models;
using ChatValues.Web.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatValues.Service
{
	public class JobCardExpiryTimer : JobTimer
	{
		public ChatValuesContext _context;

		private IConfiguration _configuration;
		EmailManager _emailManager;

		public JobCardExpiryTimer(ChatValuesContext context, IConfiguration configuration, EmailManager emailManager) : base(configuration, "JobCardExpiryTimer:Interval")
		{
			_emailManager = emailManager;
			_configuration = configuration;
			_context = context;
   

        }

		protected override void OnTimerElapsed()
		{
            DateTime todayDate = DateTime.Now;
            //for Time being
            var jobCardList = _context.JobCards.Where(c => ((TimeSpan)(todayDate - c.CreateOn)).Days == 24).ToList();
            if (jobCardList.Count > 0)
            {
                foreach (var jobCard in jobCardList)
                {
                    var user = _context.Users.FirstOrDefault<User>(u => u.ID == jobCard.OwnerID);
                    Dictionary<string, string> jobExpiryTemplatePlaceholder = new Dictionary<string, string>();
                    jobExpiryTemplatePlaceholder.Add("UserName", user.Name);
                    jobExpiryTemplatePlaceholder.Add("Days", "7");
                    _emailManager.SendEmail(new MailRecipient() { Email = user.Email, Name = user.Name }, EmailTemplate.JobExpiry, jobExpiryTemplatePlaceholder);
                }
			}

		}
	}
}
