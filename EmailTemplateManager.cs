using ChatValues.EmailClient;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web
{
	public class EmailTemplateManager : ITemplateManager
	{
		string path;

		public EmailTemplateManager(IHostingEnvironment hostingEnvironment)
		{
			path = Path.Combine(hostingEnvironment.WebRootPath, "EmailTemplate");
		}
		public string GetDirectory()
		{
			return path;
		}
	}
}
