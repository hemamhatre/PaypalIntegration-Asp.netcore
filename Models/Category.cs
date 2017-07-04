using System;

namespace ChatValues.Web.Models
{
    public class Category
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Session Session { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
