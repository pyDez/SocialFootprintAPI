using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRocks.WebAPI.Models
{
    public class NotificationModel
    {

        public int NotificationId { get; set; }
        public int AppUserId { get; set; }
        public string Information { get; set; }
        public bool IsRed { get; set; }
        public DateTime NotificationDate { get; set; }

        public string ObjectType { get; set; }
        public int ObjectId { get; set; }
        public object ObjectEntity { get; set; }
        public bool IsOld { get; set; }
    }
}