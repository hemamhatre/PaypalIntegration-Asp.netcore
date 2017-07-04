using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class UserActivation
    {
        public int ID { get; set; }
        public int UserId {get; set;}

        public Guid ActivationCode { get; set; }
    }
}
