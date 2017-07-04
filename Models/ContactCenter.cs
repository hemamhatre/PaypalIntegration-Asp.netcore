using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class ContactCenter
    {
        public List<User> BlockedUsers { get; set; }

        public List<User> RecommendedUsersByMe { get; set; }

        public List<User> UsersRecommendedMe { get; set; }

        public List<User> FavouriteUsers { get; set; }
    }
}
