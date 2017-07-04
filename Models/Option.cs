using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class Option
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public int QuestionID { get; set; }

        public Question Question { get; set; }
    }

    public class QuestionOptionMapping
    {
        public Question Question { get; set; }

        public List<Option> Options { get; set; }
    }

    public class SessionQuestionOptionMapping
    {
        public Session Session { get; set; }

        public List<QuestionOptionMapping> QuestionOptionMapping { get; set; }
    }
}
