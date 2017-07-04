using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using ChatValues.EmailClient.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace ChatValues.EmailClient
{
	public class EmailManager
	{

		private readonly MailSettings _settings;
		private ITemplateManager templateManager;
		public string[] Subjects = { "You're Welcome!","Email Verification", "Job Card is about to Expire","You got recommendation!", "New Message Arrived", "New Voice Message Arrived","You're invited!","Job card purchased!" , "Password Recovery"};

		public EmailManager(IOptions<MailSettings> settings, ITemplateManager templateManager)
		{
			this._settings = settings.Value;
			this.templateManager = templateManager;
		}
		public void SendEmail(MailRecipient to, EmailTemplate template, Dictionary<string, string> placeholders)
		{
			string body = this.PrepareMailBody(placeholders, template);
			this.Send(to, Subjects[(int)template], body);
		}

		private string PrepareMailBody(Dictionary<string, string> placeholders, EmailTemplate template)
		{
			StringBuilder sb = new StringBuilder();
			string htmlbody = File.ReadAllText(templateManager.GetDirectory() + "\\" + template + ".html");

			sb.Append(htmlbody);

			foreach (var item in placeholders)
			{
				sb.Replace("{" + item.Key + "}", item.Value);
			}

			return sb.ToString();
		}

		private void Send(MailRecipient to, string subject, string body)
		{
			MimeMessage mailMessage = new MimeMessage();

			mailMessage.From.Add(new MailboxAddress("ChatValues", _settings.User));

			mailMessage.Subject = subject;

			mailMessage.Body = new TextPart("html") { Text = body };

			mailMessage.To.Add(new MailboxAddress(to.Name, to.Email));

			using (var client = new SmtpClient())
			{
				client.AuthenticationMechanisms.Remove("XOAUTH2");
				client.Connect(_settings.Server, _settings.Port, false);
				client.Authenticate(new NetworkCredential(_settings.User, _settings.Password));
				client.Send(mailMessage);
				client.Disconnect(true);
			}
		}
	}


}
