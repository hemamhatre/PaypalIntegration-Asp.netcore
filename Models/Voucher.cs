using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatValues.Web.Models
{
    public enum VoucherStatus
    {
        Purchased, Transfered, Redeemed
    }

    public enum GrantStatus
    {
        Pending = 1, Transfered = 2, Cancelled = 3
    }

    public class Voucher
    {
        public int ID { get; set; }

        public int Quantity { get; set; }

        public int OwnerId { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime CreateOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public int? CreditorId { get; set; }

        public int TransactionTypeId { get; set; }

        [ForeignKey("OwnerId")]
        public User User { get; set; }
    }

    public class VoucherRedeem
    {
        public int ID { get; set; }

        public int QuantityRedeemed { get; set; }

        public VoucherStatus? Status { get; set; }

        public DateTime RedeemedOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }

    //public class TempGrantVoucher
    //{
    //    public GrantedVouchers gvoucher { get; set; }

    //    public string Name { get; set; }
    //}

    public class GrantedVouchers
    {
        public int ID { get; set; }

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public string grantusername { get; set; }

        public int GrantedByID { get; set; }

        public int GrantedToID { get; set; }

        public DateTime GrantedOn { get; set; }

        public int Vouchers { get; set; }

        public GrantStatus? Status { get; set; }

        public DateTime? CancelledOn { get; set; }

        public string CancellationReason { get; set; }
    }
}
