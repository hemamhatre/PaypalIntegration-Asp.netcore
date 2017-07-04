using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatValues.Web.Models
{
	public class User
	{
		public int ID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

        [Required]
       
        public string Name { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

		public string AboutMe { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

		public string Password { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Country { get; set; }

		public int Pincode { get; set; }

		public DateTime LastLogon { get; set; }

		public DateTime CreateOn { get; set; }

		public DateTime ModifiedOn { get; set; }

		public string JobTitle { get; set; }

		public string StatusMessage { get; set; }

		public string MaritalStatus { get; set; }

		public string Gender { get; set; }

		public bool IsActive { get; set; }

		public int? ViewCount { get; set; }

		public Guid? EmailConfirmationToken { get; set; }

       
        public bool? IsEmailConfirmed { get; set; }

	}

	public class UserRole
	{
		public int ID { get; set; }

		public User User { get; set; }

		public Role Role { get; set; }
	}

	public class UserSubscription
	{
		public int ID { get; set; }

		public int UserID { get; set; }

		public Subscription Subscription { get; set; }
	}

	public class UserSCQMapping
	{
		public int ID { get; set; }

		public User User { get; set; }

		public SCQMapping SCQ { get; set; }
	}

	public class LoggedInUser
	{
		public int UserId { get; set; }

		public string Email { get; set; }

		public UserRole Role { get; set; }

		public UserSubscription Subscription { get; set; }

		public bool IsActive { get; set; }
	}

	public class OnlineUser
	{
		public int ID { get; set; }

		public int UserID { get; set; }

		public User User { get; set; }
	}
}
