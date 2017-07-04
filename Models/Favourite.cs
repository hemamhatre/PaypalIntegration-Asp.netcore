using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class Favourite
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int FavouriteBy { get; set; }
    }
}
