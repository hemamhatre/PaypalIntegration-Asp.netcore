using ChatValues.PayPal;
using System;

namespace ChatValues.Web.Models
{
	public class PayPalNotification
	{

		public string PaymentStatus { get; set; }
		public string RawRequest { get; set; }
		public Guid Id { get; set; }
		public string TransactionId { get; set; }
		public TransactionType TransactionType { get; set; }
		public decimal Amount { get; set; }
		public int UserId { get; set; }
		public bool FromPayPal { get; set; }
	}
}
