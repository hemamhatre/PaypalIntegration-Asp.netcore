using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class Transaction
    {

        public int ID { get; set; }

        public int OwnerID { get; set; }
        
        public int Voucher { get; set; }

        public int Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime CreateOn { get; set; }

       public DateTime ModifiedOn { get; set; }
       public int Status { get; set; }

        [ForeignKey("OwnerID")]
        public User User { get; set; }
    }
}
