using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.EmailClient.Models
{
    public class MailSettings
    {

        public string Server { get; set; }
        public int Port { get; set; }
        public string SSL { get; set; }
        public string EmailRecipients { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

    }
}
