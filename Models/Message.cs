using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class Message
    {
        public int ID { get; set; }

        public int FromUserID { get; set; }

        public User FromUser { get; set; }

        public int ToUserID { get; set; }

        public User ToUser { get; set; }

        public DateTime SentOn { get; set; }

        public string MessageBody { get; set; }
    }
}
