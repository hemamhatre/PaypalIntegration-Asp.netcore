using System;

namespace ChatValues.Web.Models
{
    public class Subscription
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
