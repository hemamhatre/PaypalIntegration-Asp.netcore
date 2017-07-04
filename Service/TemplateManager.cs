using ChatValues.EmailClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Service
{
	public class TemplateManager : ITemplateManager
	{
		string path;

		public TemplateManager()
		{
			path = Path.Combine(AppContext.BaseDirectory, "EmailTemplate");
		}
		public string GetDirectory()
		{
			return path;
		}
	}
}
