using System;

namespace ChatValues.Web.Models
{
    public class Answers
    {
        public int ID { get; set; }

        public User User { get; set; }

        public UserSCQMapping UserSCQ { get; set; }

        public string AnswerText { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class Answer
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }

        public int OptionID { get; set; }

        public Option Option { get; set; }

        public string AnswerValue { get; set; }
    }
}
