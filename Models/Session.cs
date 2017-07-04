using System;
using System.Collections.Generic;

namespace ChatValues.Web.Models
{
    public class Session
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string ImageName { get; set; }

        public string Description { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class SCQMapping
    {
        public int ID { get; set; }
        public Session Session { get; set; }

        public Category Category { get; set; }

        public Question Question { get; set; }
    }

    public class UserSessionItem
    {
        public Session Session { get; set; }

        public int MemberCount { get; set; }

        public List<Category> Categories {get; set; }
    }

    public class AddedSessionCheck
    {
        public bool SessionExists { get; set; }
    }
}
