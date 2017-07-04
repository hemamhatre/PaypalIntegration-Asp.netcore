using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class JobCentre
    {
        public List<JobCardDeals> Sold { get; set; }

        public List<JobCardDeals> Bought { get; set; }

        public List<Recommend> Recommendation { get; set; }
    }

    public class JobCardDeals
    {
        public int ID { get; set; }
        public int JobCardID { get; set; }
        public int SoldBy { get; set; }
        public int BoughtBy { get; set; }
        public string Status { get; set; }
        public DateTime? DealOn { get; set; }
        public DateTime? ClosedOn { get; set; }

        [ForeignKey("JobCardID")]
        public JobCard JobCard { get; set; }

        [ForeignKey("SoldBy")]
        public virtual User SoldByUser { get; set; }

        [ForeignKey("BoughtBy")]
        public virtual User BoughtByUser { get; set; }
    }
}
