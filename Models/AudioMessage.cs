using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class AudioMessage
    {
        public int ID { get; set; }
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public string FileName { get; set; }

        [ForeignKey("FromUserID")]
        public User FromUser { get; set; }

        [ForeignKey("ToUserID")]
        public User ToUser { get; set; }
    }

    public class AudioMessageRequest
    {
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public object FileBlob { get; set; }
    }

    public class AudioMessageView
    {
        public int FromUserID { get; set; }
    }
}
