using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class MemberView
    {
        public List<UserJobCard> UserJobCards { get; set; }

        public List<User> Users { get; set; }
    }
}
