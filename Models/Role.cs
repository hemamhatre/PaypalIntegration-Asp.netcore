using System;

namespace ChatValues.Web.Models
{
    public class Role
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
