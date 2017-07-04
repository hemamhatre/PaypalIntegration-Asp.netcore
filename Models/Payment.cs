using System;

namespace ChatValues.Web.Models
{
    public enum PaymentType
    {
        Subscription, JobCardPayment
    }

    public class Payment
    {
        public int ID { get; set; }

        public User PaidBy { get; set; }

        public PaymentType? PaymentType { get; set; }

        public DateTime PaidOn { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
