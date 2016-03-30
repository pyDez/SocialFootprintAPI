using iRocks.DataLayer;
using System.Collections.Generic;

namespace iRocks.WebAPI.Models
{
    public class AppUserModel
    {
        public int AppUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public string Locale { get; set; }
        public List<Skill> Footprint { get; set; }
        public List<PostModel> Posts { get; set; }
        public List<Vote> Votes { get; set; }
        public List<NotificationModel> Notifications { get; set; }
        public List<AppUserModel> Friends { get; set; }
        public List<BadgeCollected> Badges { get; set; }
        public List<PublicationModel> Newsfeed { get; set; }
        public FacebookUserDetailModel FacebookDetail { get; set; }
        public TwitterUserDetailModel TwitterDetail { get; set; }
        public bool IsFacebookProvided { get; set; }
        public bool IsTwitterProvided { get; set; }
    }
}