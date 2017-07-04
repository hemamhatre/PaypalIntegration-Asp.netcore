using ChatValues.EmailClient;
using ChatValues.EmailClient.Models;
using ChatValues.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace ChatValues.Service
{
	public class Program
	{


		public static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
			.SetBasePath(Path.Combine(AppContext.BaseDirectory))
			.AddJsonFile("appsettings.json");

			Configuration = builder.Build();//missing
			var connection = Configuration.GetConnectionString("DefaultConnection");

			var services = new ServiceCollection()
				.AddDbContext<ChatValuesContext>(options => options.UseSqlServer(connection))
			.AddSingleton<ITemplateManager, TemplateManager>()
			.AddTransient<VoucherTransferTimer>()
			.AddTransient<JobCardExpiryTimer>()
			.AddSingleton<IConfiguration>(builder.Build())

			.Configure<MailSettings>(Configuration.GetSection("smtp"))
			.AddSingleton<EmailManager>()
			.BuildServiceProvider();

			var voucherExpiryTimer = services.GetService<VoucherTransferTimer>();
			//var jobCardExpiryTimer = services.GetService<JobCardExpiryTimer>();

			voucherExpiryTimer.Start();
			//jobCardExpiryTimer.Start();
			Console.WriteLine("Press Enter to exit.");
			Console.ReadLine();

		}

		public static IConfigurationRoot Configuration { get; set; }
	}
}
