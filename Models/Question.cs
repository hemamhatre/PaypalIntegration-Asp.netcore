using System;

namespace ChatValues.Web.Models
{
    public class Question
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        public string ControlType { get; set; }

        public int SessionID { get; set; }

        public Session Session { get; set; }

        public int? OptionID { get; set; }

        //public Option Option { get; set; }
    }
}
