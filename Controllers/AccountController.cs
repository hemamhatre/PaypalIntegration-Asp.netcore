using ChatValues.EmailClient;
using ChatValues.EmailClient.Models;
using ChatValues.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatValues.Web.Controllers
{
	public class AccountController : BaseController
	{
		private IHostingEnvironment hostingEnv;
		EmailManager emailManager;

		public AccountController(ChatValuesContext context, IHostingEnvironment env, EmailManager emailManager) : base(context)
		{
			this.hostingEnv = env;
			this.emailManager = emailManager;
		}

		// GET: /<controller>/
		public IActionResult Index()
		{
			return View();
		}


		public async Task<IActionResult> LoginUser()
		{
			var username = Request.Form["Email"];
			var password = Request.Form["LoginPassword"];

			var user = _context.Users.ToList<User>().FirstOrDefault(x => x.Email == username && x.Password == password);
			if (user != null && user.IsActive)
			{

				var dirPath = @"\images\Profile\" + user.ID;
				string filename = "/images/profile_img.jpg";
				if (Directory.Exists(hostingEnv.WebRootPath + $@"{dirPath}"))
				{
					string[] fileNames = Directory.GetFiles(hostingEnv.WebRootPath + $@"{dirPath}");
					if (fileNames.Length > 0)
					{
						filename = "/images/Profile/" + user.ID.ToString() + "/" + fileNames[0].Substring(fileNames[0].LastIndexOf(@"\") + 1);
					}
				}
				var claims = new List<Claim> {
					new Claim(ClaimTypes.Name, user.Name, ClaimValueTypes.String, Constants.Issuer),
					new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String, Constants.Issuer),
					new Claim(ClaimTypes.NameIdentifier, user.ID.ToString(), ClaimValueTypes.String, Constants.Issuer),
					new Claim(ClaimTypes.Webpage, filename, ClaimValueTypes.String, Constants.Issuer)
				};

				CookieOptions options = new CookieOptions();
				options.Expires = DateTime.Now.AddDays(1);
				Response.Cookies.Append("userid", user.ID.ToString(), options);

				var userIdentity = new ClaimsIdentity(claims);

				var userPrincipal = new ClaimsPrincipal(userIdentity);
				await HttpContext.Authentication.SignInAsync("ChatValuesCookieMiddlewareInstance", userPrincipal);

				//Maintain online users list
				var onlineUserLast = _context.OnlineUsers.LastOrDefault<OnlineUser>();
				var olUserID = 1;
				if (onlineUserLast != null)
				{
					olUserID = onlineUserLast.ID + 1;
				}

				OnlineUser ol = new OnlineUser();
				ol.ID = olUserID;
				ol.User = user;
				ol.UserID = user.ID;

				_context.OnlineUsers.Add(ol);

				_context.SaveChanges();
				return RedirectToAction("Sessions", "Home");
			}
			else
			{
				int errorCode = 0;
				if (user == null)
					errorCode = 1;
				else if (user.IsEmailConfirmed == false)
				{
					errorCode = 2;
				}
				else
					errorCode = 3;
				return RedirectToAction("Index", "Home", new { errorCode = errorCode });
			}
		}

		public async Task<IActionResult> LogoutUser()
		{
			//Remove from online users list
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			var olu = _context.OnlineUsers.FirstOrDefault<OnlineUser>(o => o.UserID == int.Parse(userIdVal.Value));
			if (olu != null)
			{
				_context.OnlineUsers.Remove(olu);
			}

			CookieOptions options = new CookieOptions();
			options.Expires = DateTime.Now.AddDays(1);
			Response.Cookies.Append("userid", "0", options);
			await HttpContext.Authentication.SignOutAsync("ChatValuesCookieMiddlewareInstance");

			return RedirectToAction("Index", "Home");
		}

		public IActionResult UserRegistration()
		{


			var userRoles = _context.Roles.ToList<Role>();
			if (userRoles.Count > 0)
			{
				ViewBag.UserRoles = userRoles;
			}
			var sessions = _context.Sessions.ToList<Session>();


			string objvalue = Newtonsoft.Json.JsonConvert.SerializeObject(sessions);

			#region Getting Questions and Options

			var allSessionsWithQuestions = new List<SessionQuestionOptionMapping>();

			foreach (var session in sessions)
			{

				var questionsWithOptions = new List<QuestionOptionMapping>();
				var questions = _context.Questions.Where(q => q.SessionID == session.ID).ToList<Question>();
				foreach (var question in questions)
				{
					var options = _context.Options.Where(o => o.QuestionID == question.ID).ToList<Option>();
					questionsWithOptions.Add(new QuestionOptionMapping() { Question = question, Options = options });
					string str = "";
					//var quesOptionMap = from question in questions
					//                    join option in _context.Options on question.ID equals option.QuestionID
					//                    select new { Question = question, Option = option };
				}
				allSessionsWithQuestions.Add(new SessionQuestionOptionMapping() { Session = session, QuestionOptionMapping = questionsWithOptions });
			}
			ViewBag.Sessions = allSessionsWithQuestions;
			#endregion

			return View(new User() { IsActive = false });
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser(User newUser)
		{

			var user = _context.Users.FirstOrDefault<User>(u => u.Email == newUser.Email);
			if (user == null)
			{
				newUser.CreateOn = DateTime.Now;
				newUser.ModifiedOn = DateTime.Now;
				newUser.LastLogon = DateTime.Now;
				newUser.IsEmailConfirmed = false;
				newUser.EmailConfirmationToken = Guid.NewGuid();
				newUser.Country = Request.Form["ddlcountry"].ToString();


				_context.Users.Add(newUser);
				_context.SaveChanges();

				var hiddenRadios = Request.Form.Where(rdo => rdo.Key.StartsWith("hdnRadio_") && !string.IsNullOrEmpty(rdo.Value)).ToList();
				foreach (var rdo in hiddenRadios)
				{
					var lastAanswers = _context.Answers;
					int ansId = 1;
					if (lastAanswers != null && lastAanswers.ToList().Count > 0)
					{
						var lastAanswer = lastAanswers.ToList().Last<Answer>();
						ansId = lastAanswer.ID + 1;
					}
					string rdoValue = rdo.Value[0];
					string[] strArray = rdoValue.Split(new char[] { '_' });
					rdoValue = strArray[1];
					Answer ans = new Answer();
					ans.ID = ansId;
					ans.OptionID = int.Parse(rdoValue);
					ans.UserID = newUser.ID;
					_context.Answers.Add(ans);
					_context.SaveChanges();
				}
				var txtOptions = Request.Form.Where(txt => txt.Key.EndsWith("_txt") && !string.IsNullOrEmpty(txt.Value)).ToList();
				foreach (var text in txtOptions)
				{
					if (!string.IsNullOrEmpty(text.Value[0]))
					{
						var lastAanswers = _context.Answers;
						int ansId = 1;
						if (lastAanswers != null && lastAanswers.ToList().Count > 0)
						{
							var lastAanswer = lastAanswers.ToList().Last<Answer>();
							ansId = lastAanswer.ID + 1;
						}
						//string txtValue = text.Value[0];
						//string[] strArray = txtValue.Split(new char[] { '_' });
						//txtValue = strArray[1];
						Answer ans = new Answer();
						ans.ID = ansId;
						ans.AnswerValue = text.Value[0];
						ans.UserID = newUser.ID;
						_context.Answers.Add(ans);
						_context.SaveChanges();
					}
				}

				var ddlOptions = Request.Form.Where(ddl => ddl.Key.EndsWith("_ddl") && !string.IsNullOrEmpty(ddl.Value)).ToList();
				foreach (var ddl in ddlOptions)
				{
					var lastAanswers = _context.Answers;
					int ansId = 1;
					if (lastAanswers != null && lastAanswers.ToList().Count > 0)
					{
						var lastAanswer = lastAanswers.ToList().Last<Answer>();
						ansId = lastAanswer.ID + 1;
					}
					Answer ans = new Answer();
					ans.ID = ansId;
					ans.OptionID = int.Parse(ddl.Value[0]);
					ans.UserID = newUser.ID;
					_context.Answers.Add(ans);
					_context.SaveChanges();
				}

				var chkOptions = Request.Form.Where(chk => chk.Key.EndsWith("_chk") && !string.IsNullOrEmpty(chk.Value)).ToList();
				foreach (var chk in chkOptions)
				{
					if (chk.Value.Count > 0)
					{
						foreach (var chkVal in chk.Value)
						{
							var lastAanswers = _context.Answers;
							int ansId = 1;
							if (lastAanswers != null && lastAanswers.ToList().Count > 0)
							{
								var lastAanswer = lastAanswers.ToList().Last<Answer>();
								ansId = lastAanswer.ID + 1;
							}

							Answer ans = new Answer();
							ans.ID = ansId;
							ans.OptionID = int.Parse(chkVal);
							ans.UserID = newUser.ID;
							_context.Answers.Add(ans);
							_context.SaveChanges();
						}
					}
				}

				var sessions = Request.Form.Where(chk => chk.Key.StartsWith("chkSession_")).ToList();
				foreach (var session in sessions)
				{

					var tempsession = _context.UserSessions.LastOrDefault<UserSession>();

					UserSession userSesn = new UserSession();
					userSesn.SessionID = int.Parse(session.Value[0]);
					userSesn.UserID = newUser.ID;
					userSesn.ID = tempsession.ID + 1;
					_context.UserSessions.Add(userSesn);
					_context.SaveChanges();
				}

				UserRole role = new UserRole();
				role.User = newUser;
				//var roleId = Request.Form["ddlUserRole"];
				var roleId = "3";
				if (string.IsNullOrEmpty(roleId))
				{
					if (!roleId.Equals("0"))
					{
						role.Role = _context.Roles.FirstOrDefault<Role>(x => x.ID == int.Parse(roleId));
						if (role.Role != null)
						{
							_context.UserRoles.Add(role);
							_context.SaveChanges();
						}
					}
				}

				MailRecipient mailRecipient = new MailRecipient();
				mailRecipient.Name = newUser.Name;
				mailRecipient.Email = newUser.Email;

				var tokenVerificationUrl = Url.Action("VerifyEmail", "Account", new { id = newUser.ID, token = newUser.EmailConfirmationToken }, Request.Scheme);
				Dictionary<string, string> confirmTemplatePlaceholder = new Dictionary<string, string>();
				confirmTemplatePlaceholder.Add("UserName", newUser.Name);
				confirmTemplatePlaceholder.Add("ConfirmEmail", tokenVerificationUrl);
				emailManager.SendEmail(mailRecipient, EmailTemplate.EmailVerification, confirmTemplatePlaceholder);
				return RedirectToAction("Confirm", "Account");

			}
			else
			{
				ViewBag.ErrorMessage = "Email already exists. Please use another email ID!";
				return View("UserRegistration", new User() { IsActive = true });
			}
		}

		/// <summary>
		///  forgot password functionality
		/// </summary>
		/// <returns></returns>

		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public IActionResult ForgotPassword(User model)
		{
			var usermodel = _context.Users.FirstOrDefault<User>(u => u.Email == model.Email);

			if (usermodel != null)
			{
				MailRecipient mailRecipient = new MailRecipient();
				mailRecipient.Name = usermodel.Name;
				mailRecipient.Email = usermodel.Email;
				Dictionary<string, string> templatePlaceholder = new Dictionary<string, string>();
				templatePlaceholder.Add("UserName", usermodel.Name);
				templatePlaceholder.Add("Senderid", usermodel.ID.ToString());
				templatePlaceholder.Add("Password", usermodel.Password);
				//templatePlaceholder.Add("", );
				emailManager.SendEmail(mailRecipient, EmailTemplate.ForgotPass, templatePlaceholder);
				ViewBag.Message = "Password Sent Successfully ";

				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.Message = "Invalid Email ID";
			}
			return View(model);
		}



		public IActionResult UpdateUser(UserJobCard updateUser)
		{
			var sessionId = Request.Form["ddlInterest"].ToString();
			var userSession = _context.UserSessions.FirstOrDefault<UserSession>(us => us.SessionID == int.Parse(sessionId) && us.UserID == updateUser.User.ID);
			if (userSession == null)
			{
				_context.UserSessions.Add(new UserSession() { SessionID = int.Parse(sessionId), UserID = updateUser.User.ID });
			}
			var user = _context.Users.FirstOrDefault<User>(u => u.ID == updateUser.User.ID);
			var userPassword = user.Password;
			user = updateUser.User;
			user.Password = userPassword;


			_context.SaveChanges();

			updateUser.JobCard.CreateOn = DateTime.Now;
			updateUser.JobCard.ModifiedOn = DateTime.Now;
			updateUser.JobCard.OwnerID = updateUser.User.ID;
			updateUser.User = updateUser.User;
			updateUser.JobCard.ApprovedOn = DateTime.Now;
			updateUser.JobCard.VouchersPaidOn = DateTime.Now;
			string statusMessageStr = string.Empty;
			if (Request.Form["rdoMessage"].ToString() == "2")
			{
				statusMessageStr = Request.Form["txtMessageTemplate"].ToString();
			}
			else if (Request.Form["rdoMessage"].ToString() == "0")
			{
				statusMessageStr = Request.Form["sp0_message"].ToString();
			}
			else if (Request.Form["rdoMessage"].ToString() == "1")
			{
				statusMessageStr = Request.Form["sp1_message"].ToString();
			}
			updateUser.JobCard.StatusMessage = statusMessageStr;

			_context.JobCards.Add(updateUser.JobCard);
			_context.SaveChanges();
			if (Request.Form.Files.Count > 0)
			{
				var dirPath = @"\images\jobcard\" + updateUser.JobCard.ID;
				DirectoryInfo di = Directory.CreateDirectory(hostingEnv.WebRootPath + $@"{dirPath}");
				long size = 0;

				foreach (var file in Request.Form.Files)
				{
					var filename = ContentDispositionHeaderValue
									.Parse(file.ContentDisposition)
									.FileName
									.Trim('"');
					filename = hostingEnv.WebRootPath + $@"{dirPath}\{filename}";
					size += file.Length;
					using (FileStream fs = System.IO.File.Create(filename))
					{
						file.CopyTo(fs);
						fs.Flush();
					}
				}



				_context.SaveChanges();
				ViewBag.ErrorMessage = "";
			}

			return RedirectToAction("UserProfile", "Account", new { id = updateUser.User.ID });
		}


        // vouchers bought and balance number of vouchers
        public void SetVoucherDetails(int userid)
        {

            var BroughtVouch = _context.Vouchers.Where(x => x.OwnerId == userid).Where(u => u.TransactionTypeId == 1).Sum<Voucher>(u => u.Quantity);
            ViewBag.BroughtVouchers = BroughtVouch;

            var Balance = _context.Vouchers.Where(x => x.OwnerId == userid).Sum<Voucher>(u => u.Quantity);
            ViewBag.BalanceVouchers = Balance;

            var recommendation = _context.Recommendations.Include(u => u.UserIDs).Where(c => c.UserID == userid).ToList();
            ViewBag.RecommendationCount = recommendation.Count<Recommend>(t => t.UserID == userid);
        }


        public IActionResult UserProfile(int id)
		{
			UserJobCard userJobCard = new UserJobCard();
			JobCardViews jobCardViews = new JobCardViews();
			var user = _context.Users.FirstOrDefault<User>(u => u.ID == id);


            // Setting Purchase , Balance and Total recomedations
              SetVoucherDetails(id);

			string birthdayString = user.DateOfBirth.ToString();
			// Save today's date.
			var today = DateTime.Today;

			// Calculate the age.
			var age = today.Year - user.DateOfBirth.Year;

			// Do stuff with it.
			if (user.DateOfBirth > today.AddYears(-age)) age--;

			ViewBag.AgeString = age.ToString();

			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			var jobCardId = _context.JobCards.FirstOrDefault<JobCard>(u => u.OwnerID == user.ID);
			//Add to implement view of the job
			if (user.ID != int.Parse(userIdVal.Value))
			{
				jobCardViews.JobCardID = jobCardId.ID;
				jobCardViews.ViewedBy = int.Parse(userIdVal.Value);
				jobCardViews.ViewedOn = DateTime.Now;
				_context.JobCardViews.Add(jobCardViews);
				_context.SaveChanges();
			}

			if (jobCardId != null)
				ViewBag.ViewJobCount = _context.JobCardViews.Count(t => t.JobCard.ID == jobCardId.ID);

			if (userIdVal != null)
			{
				ViewBag.UserId = userIdVal.Value;
				if (int.Parse(userIdVal.Value) == user.ID)
				{
					ViewBag.LoggedInUser = true;
				}
				else
				{
					ViewBag.LoggedInUser = false;
				}

				var recommendedUser = _context.Recommendations.FirstOrDefault<Recommend>(b => b.UserID == id && b.RecommendedBy == int.Parse(userIdVal.Value));
				if (recommendedUser != null)
				{
					ViewBag.IsUserRecommended = true;
				}
				else
				{
					ViewBag.IsUserRecommended = false;
				}

				var blockedUser = _context.BlockedUsers.FirstOrDefault<BlockedUser>(b => b.UserID == id && b.BlockedBy == int.Parse(userIdVal.Value));
				if (blockedUser != null)
				{
					ViewBag.IsUserBlocked = true;
				}
				else
				{
					ViewBag.IsUserBlocked = false;
				}

				var favouriteUser = _context.Favourites.FirstOrDefault<Favourite>(b => b.UserID == id && b.FavouriteBy == int.Parse(userIdVal.Value));
				if (favouriteUser != null)
				{
					ViewBag.IsSetAsFavourite = true;
				}
				else
				{
					ViewBag.IsSetAsFavourite = false;
				}
			}

			//var userPicVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.Webpage);
			//if (userPicVal != null)
			//{
			//    ViewBag.ProfilePhoto = userPicVal.Value;
			//}

			DateTime todayDate = DateTime.Now;
			TimeSpan ts = todayDate - user.CreateOn;
			if (ts.Days >= 10)
				//then dont show tag
				ViewBag.NewUserTag = false;
			else
				ViewBag.NewUserTag = true;


			var dirPath = @"\images\Profile\" + id;
			if (Directory.Exists(hostingEnv.WebRootPath + $@"{dirPath}"))
			{
				string[] fileNames = Directory.GetFiles(hostingEnv.WebRootPath + $@"{dirPath}");
				if (fileNames.Length > 0)
				{
					ViewBag.ProfilePhoto = "/images/Profile/" + id.ToString() + "/" + fileNames[0].Substring(fileNames[0].LastIndexOf(@"\") + 1);
				}
				else
				{
					ViewBag.ProfilePhoto = "/images/profile_img.jpg";
				}
			}
			else
			{
				ViewBag.ProfilePhoto = "/images/profile_img.jpg";
			}

			var jobCard = _context.JobCards.FirstOrDefault<JobCard>(j => j.OwnerID == id);
			if (jobCard != null)
			{
				ViewBag.IsNewJobCard = (todayDate - jobCard.CreateOn).Days <= 10;
				ViewBag.JobCardCreateOn = jobCard.CreateOn.ToString("dd-MM-yyyy");
				dirPath = @"\images\jobcard\" + jobCard.ID;
				if (Directory.Exists(hostingEnv.WebRootPath + $@"{dirPath}"))
				{
					string[] fileNames = Directory.GetFiles(hostingEnv.WebRootPath + $@"{dirPath}");

					if (fileNames.Length > 0)
					{
						ViewBag.FileNames = fileNames;
					}
				}
			}
			userJobCard.JobCard = jobCard;
			userJobCard.User = user;
			return View(userJobCard);
		}

		[HttpPost]
		public IActionResult CreateJobCard(UserJobCard userJobCard)
		{
			userJobCard.JobCard.CreateOn = DateTime.Now;
			userJobCard.JobCard.ModifiedOn = DateTime.Now;
			var user = _context.Users.FirstOrDefault<User>(u => u.ID == userJobCard.User.ID);
			userJobCard.JobCard.OwnerID = user.ID;
			userJobCard.User = user;
			userJobCard.JobCard.ApprovedOn = DateTime.Now;
			userJobCard.JobCard.VouchersPaidOn = DateTime.Now;

			_context.JobCards.Add(userJobCard.JobCard);
			_context.SaveChanges();
			if (Request.Form.Files.Count > 0)
			{
				var dirPath = @"\images\jobcard\" + userJobCard.JobCard.ID;
				DirectoryInfo di = Directory.CreateDirectory(hostingEnv.WebRootPath + $@"{dirPath}");
				long size = 0;

				foreach (var file in Request.Form.Files)
				{
					var filename = ContentDispositionHeaderValue
									.Parse(file.ContentDisposition)
									.FileName
									.Trim('"');
					filename = hostingEnv.WebRootPath + $@"{dirPath}\{filename}";
					size += file.Length;
					using (FileStream fs = System.IO.File.Create(filename))
					{
						file.CopyTo(fs);
						fs.Flush();
					}
				}
			}

			return RedirectToAction("UserProfile", new { id = userJobCard.User.ID });
		}
		[HttpPost]
		public IActionResult UploadProfilePhoto()
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			if (Request.Form.Files.Count > 0)
			{
				var dirPath = @"\images\Profile\" + userIdVal.Value;
				DirectoryInfo di = Directory.CreateDirectory(hostingEnv.WebRootPath + $@"{dirPath}");
				if (Directory.Exists(hostingEnv.WebRootPath + $@"{dirPath}"))
				{
					foreach (FileInfo file in di.GetFiles())
					{
						file.Delete();
					}
				}

				long size = 0;

				foreach (var file in Request.Form.Files)
				{
					var filename = ContentDispositionHeaderValue
									.Parse(file.ContentDisposition)
									.FileName
									.Trim('"');
					filename = hostingEnv.WebRootPath + $@"{dirPath}\{filename}";
					size += file.Length;
					using (FileStream fs = System.IO.File.Create(filename))
					{
						file.CopyTo(fs);
						fs.Flush();
					}
				}
			}
			return RedirectToAction("UserProfile", new { id = userIdVal.Value });
		}

		public IActionResult SendDealRequest(int jobCardId, int userId)
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			var jobCard = _context.JobCards.FirstOrDefault<JobCard>(j => j.ID == jobCardId);
			if (jobCard != null)
			{
				var user = _context.Users.FirstOrDefault<User>(u => u.ID == userId);
				if (user != null)
				{
					//jobCard.AssignedTo = user;
					//_context.JobCards.Update(jobCard);
					//_context.SaveChanges();
					var id = _context.JobCardDeals.LastOrDefault<JobCardDeals>();

					JobCardDeals deal = new JobCardDeals();
					deal.JobCardID = jobCardId;
					deal.BoughtBy = int.Parse(userIdVal.Value);
					deal.SoldBy = userId;
					deal.Status = "In process";
					if (id != null)
					{
						deal.ID = id.ID + 1;
					}
					else
					{
						deal.ID = 1;
					}
					_context.JobCardDeals.Add(deal);
					_context.SaveChanges();


					#region emailNotification 


					var Puruser = _context.Users.FirstOrDefault<User>(u => u.ID == deal.BoughtBy);

					MailRecipient mailRecipient = new MailRecipient();
					mailRecipient.Name = user.Name;
					mailRecipient.Email = user.Email;
					Dictionary<string, string> templatePlaceholder = new Dictionary<string, string>();
					templatePlaceholder.Add("UserName", user.Name);
					templatePlaceholder.Add("MessageContent", "Job Card ID : " + deal.JobCardID + " has been bought By " + Puruser.Name);

					emailManager.SendEmail(mailRecipient, EmailTemplate.JobPurchase, templatePlaceholder);

					#endregion

					return RedirectToAction("Index", "JobCentre");
				}
				else
				{
					ViewBag.ErrorMessage = "Error assigning user to job card. Please try again later.";
					return RedirectToAction("UserProfile", new { id = userIdVal.Value });
				}
			}
			else
			{
				ViewBag.ErrorMessage = "Error in fetching details of job card. Please try again later.";
				return RedirectToAction("UserProfile", new { id = userIdVal.Value });
			}
		}

		public IActionResult BlockUser(int userId)
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			BlockedUser bUser = new BlockedUser();
			var blockedUser = _context.BlockedUsers.LastOrDefault<BlockedUser>();
			if (blockedUser != null)
			{
				bUser.ID = blockedUser.ID + 1;
			}
			else
			{
				bUser.ID = 1;
			}

			bUser.BlockedBy = int.Parse(userIdVal.Value);
			bUser.UserID = userId;
			_context.BlockedUsers.Add(bUser);
			_context.SaveChanges();
			return RedirectToAction("UserProfile", new { id = userId });
		}

		public IActionResult UnBlockUser(int userId)
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			var blockedUser = _context.BlockedUsers.FirstOrDefault<BlockedUser>(b => b.BlockedBy == int.Parse(userIdVal.Value) && b.UserID == userId);
			if (blockedUser != null)
			{
				_context.BlockedUsers.Remove(blockedUser);
				_context.SaveChanges();
			}
			return RedirectToAction("UserProfile", new { id = userId });
		}

		public IActionResult FavouriteUser(int userId)
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			Favourite fUser = new Favourite();
			var favUser = _context.Favourites.LastOrDefault<Favourite>();
			if (favUser != null)
			{
				fUser.ID = favUser.ID + 1;
			}
			else
			{
				fUser.ID = 1;
			}

			fUser.FavouriteBy = int.Parse(userIdVal.Value);
			fUser.UserID = userId;
			_context.Favourites.Add(fUser);
			_context.SaveChanges();
			return RedirectToAction("UserProfile", new { id = userId });
		}

		public IActionResult RemoveFavouriteUser(int userId)
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			Favourite fUser = new Favourite();
			var favUser = _context.Favourites.FirstOrDefault<Favourite>(b => b.FavouriteBy == int.Parse(userIdVal.Value) && b.UserID == userId);
			if (favUser != null)
			{
				_context.Favourites.Remove(favUser);
				_context.SaveChanges();
			}
			return RedirectToAction("UserProfile", new { id = userId });
		}

		public IActionResult RecommendUser(int userId)
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			Recommend bUser = new Recommend();
			var recommendedUser = _context.Recommendations.LastOrDefault<Recommend>();
			if (recommendedUser != null)
			{
				bUser.ID = recommendedUser.ID + 1;
			}
			else
			{
				bUser.ID = 1;
			}

			var userdata = _context.Users.FirstOrDefault<User>(u => u.ID == userId);

			bUser.RecommendedBy = int.Parse(userIdVal.Value);
			bUser.UserID = userId;
			_context.Recommendations.Add(bUser);
			_context.SaveChanges();

			#region Send Email 

			var recomendedUserdata = _context.Users.FirstOrDefault<User>(u => u.ID == userId);
			var RecomendedByUserData = _context.Users.FirstOrDefault<User>(u => u.ID == bUser.RecommendedBy);


			MailRecipient mailRecipient = new MailRecipient();
			mailRecipient.Name = userdata.Name;
			mailRecipient.Email = userdata.Email;
			Dictionary<string, string> templatePlaceholder = new Dictionary<string, string>();
			templatePlaceholder.Add("UserName", userdata.Name);
			templatePlaceholder.Add("RecommendedBy", RecomendedByUserData.Name);


			emailManager.SendEmail(mailRecipient, EmailTemplate.Recommendation, templatePlaceholder);


			#endregion

			return RedirectToAction("UserProfile", new { id = userId });
		}

		public IActionResult UnRecommendUser(int userId)
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			var recommendedUser = _context.Recommendations.FirstOrDefault<Recommend>(b => b.RecommendedBy == int.Parse(userIdVal.Value) && b.UserID == userId);
			if (recommendedUser != null)
			{
				_context.Recommendations.Remove(recommendedUser);
				_context.SaveChanges();
			}
			return RedirectToAction("UserProfile", new { id = userId });
		}

		public IActionResult CreateJobCard()
		{
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			var user = _context.Users.FirstOrDefault<User>(u => u.ID == int.Parse(userIdVal.Value));
			UserJobCard userJobCard = new UserJobCard();
			userJobCard.User = user;
			return View(userJobCard);
		}

		public IActionResult EditJobCard(UserJobCard userJobCard)
		{
			var jobCard = _context.JobCards.FirstOrDefault<JobCard>(j => j.ID == userJobCard.JobCard.ID);
			jobCard.Title = userJobCard.JobCard.Title;
			jobCard.Description = userJobCard.JobCard.Description;
			//jobCard.Benefits = userJobCard.JobCard.Benefits;
			//jobCard.Achievements = userJobCard.JobCard.Achievements;
			//jobCard.Deliverables = userJobCard.JobCard.Deliverables;
			//jobCard.Requirements = userJobCard.JobCard.Requirements;

			//_context.JobCards.Update(jobCard);
			_context.Entry(jobCard).State = EntityState.Modified;
			_context.SaveChanges();

			if (Request.Form.Files.Count > 0)
			{
				var dirPath = @"\images\jobcard\" + userJobCard.JobCard.ID;
				DirectoryInfo di = Directory.CreateDirectory(hostingEnv.WebRootPath + $@"{dirPath}");
				long size = 0;

				foreach (var file in Request.Form.Files)
				{
					var filename = ContentDispositionHeaderValue
									.Parse(file.ContentDisposition)
									.FileName
									.Trim('"');
					filename = hostingEnv.WebRootPath + $@"{dirPath}\{filename}";
					size += file.Length;
					using (FileStream fs = System.IO.File.Create(filename))
					{
						file.CopyTo(fs);
						fs.Flush();
					}
				}
			}
			return RedirectToAction("UserProfile", new { id = userJobCard.User.ID });
		}

		public IActionResult CheckSession(int sessionID)
		{
			bool flag = false;
			var userClaim = (ClaimsIdentity)User.Identity;
			var userIdVal = userClaim.Claims.FirstOrDefault<Claim>(c => c.Type == ClaimTypes.NameIdentifier);
			var userSession = _context.UserSessions.FirstOrDefault<UserSession>(us => us.SessionID == sessionID && us.UserID == int.Parse(userIdVal.Value));
			if (userSession != null)
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
			return Json(new AddedSessionCheck() { SessionExists = flag });
		}

		public IActionResult Confirm(string Email)
		{
			ViewBag.Email = Email;
			return View();
		}
		public IActionResult VerifyEmail(int id, Guid token)
		{

			ViewBag.Message = "Invalid Activation code.";
			if (id != 0)
			{
				var user = _context.Users.Where(p => p.EmailConfirmationToken == token && p.ID == id).FirstOrDefault();
				if (user != null)
				{
					user.IsActive = true;
					user.IsEmailConfirmed = true;
					_context.SaveChanges();
					ViewBag.Message = "Activation successful.";

					Dictionary<string, string> WelcomeTemplatePlaceholder = new Dictionary<string, string>();
					WelcomeTemplatePlaceholder.Add("UserName", user.Name);
					emailManager.SendEmail(new MailRecipient() { Email = user.Email, Name = user.Name }, EmailTemplate.Welcome, WelcomeTemplatePlaceholder);

					return RedirectToAction("Index", "Home");
				}
			}
			return View();
		}
	}
}
