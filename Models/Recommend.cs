using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class Recommend
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int RecommendedBy { get; set; }

        [ForeignKey("UserID")]
        public virtual User UserIDs { get; set; }

        [ForeignKey("RecommendedBy")]
        public virtual User RecommendedBys { get; set; }
    }
}
