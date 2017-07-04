using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class JobCardViews
    {
        public int ID { get; set; }
        public int JobCardID { get; set; }
        public int ViewedBy { get; set; }
        public DateTime ViewedOn { get; set; }

        [ForeignKey("JobCardID")]
        public JobCard JobCard { get; set; }

    }
}
