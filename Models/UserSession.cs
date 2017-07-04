using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatValues.Web.Models
{
    public class UserSession
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int SessionID { get; set; }
        
        public User User { get; set; }

        public Session Session { get; set; }
    }
}
