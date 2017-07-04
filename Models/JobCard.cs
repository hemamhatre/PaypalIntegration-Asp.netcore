using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatValues.Web.Models
{
    public enum JobCardStatus
    {
        New, Approved, Accepted, Rejected, Completed, Closed, Cancelled
    }

    public class JobCard
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Benefits { get; set; }

        public string Achievements { get; set; }

        public string Deliverables { get; set; }

        public string Requirements { get; set; }

        public int VoucherValue { get; set; }

        public User AssignedTo { get; set; }

        public int VouchersPaid { get; set; }

        public DateTime VouchersPaidOn { get; set; }

        public int OwnerID { get; set; }

        public User ApprovedBy { get; set; }

        public JobCardStatus? Status { get; set; }

        public string StatusMessage { get; set; }

        public DateTime ApprovedOn { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        [ForeignKey("OwnerID")]
        public User User { get; set; }
    }

    public class UserJobCard
    {
        public JobCard JobCard { get; set; }

        public User User { get; set; }

        public string SessionName { get; set; }
    }
}
