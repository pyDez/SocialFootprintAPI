using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRocks.WebAPI.Models
{
    public class VoteResponseModel
    {
        public VoteResponseModel(IEnumerable<Skill> skills, BadgeCollected badgeWon, DataLayer.Notification newNotification)
        {
            this.Skills = skills;
            this.BadgeWon = badgeWon;
            this.NewNotification = newNotification;
        }
        public IEnumerable<Skill> Skills { get; set; }
        public BadgeCollected BadgeWon { get; set; }
        public DataLayer.Notification NewNotification { get; set; }
    }
}