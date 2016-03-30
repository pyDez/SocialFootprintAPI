using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRocks.DataLayer;
using System.Runtime.Serialization;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{

    public class AppUser : DapperManagedObject<AppUser>, IGetId, IDeeplyCloneable<AppUser>, IUser
    {
        private int skillDefaultlevel = 50;
        public AppUser()
        {
            this.SigningUpDate = DateTime.Now;
            this.LastLogInDate = DateTime.Now;
            this.Birthday = DateTime.Now;
            this.Posts = new List<Post>();
            this.Votes = new List<Vote>();
            this.Notifications = new List<Notification>();
            this.Friends = new List<AppUser>();
            this.Footprint = new List<Skill>();
            this.ExternalLogins = new List<ExternalLogin>();
            this.Newsfeed = new List<Publication>();
            this.Badges = new List<BadgeCollected>();
            this.SecurityStamp = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public int AppUserId { get; set; }

        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }
        public bool Gender { get; set; }

        public DateTime Birthday { get; set; }
        public bool Activated { get; set; }
        public string Locale { get; set; }

        public DateTime SigningUpDate { get; set; }

        public DateTime LastLogInDate { get; set; }


        public List<Skill> Footprint { get; set; }

        public List<Post> Posts { get; set; }

        public List<Vote> Votes { get; set; }
        public List<Notification> Notifications { get; set; }

        public List<AppUser> Friends { get; set; }
        public List<ExternalLogin> ExternalLogins { get; set; }

        public List<Publication> Newsfeed { get; set; }
        public List<BadgeCollected> Badges { get; set; }

        public FacebookUserDetail FacebookDetail { get; set; }
        public TwitterUserDetail TwitterDetail { get; set; }
        [DapperIgnore]
        public string ReturnUrl { get; set; }
        public bool IsNew
        {
            get
            {
                return this.AppUserId == default(int);
            }
        }
        public bool IsProvidedBy(Provider provider)
        {
            switch (provider.Value)
            {
                case "Facebook":
                    if (this.FacebookDetail != null)
                        if (!string.IsNullOrEmpty(this.FacebookDetail.FacebookUserId))
                            return true;
                    return false;
                case "Twitter":
                    if (this.TwitterDetail != null)
                        if (!string.IsNullOrEmpty(this.TwitterDetail.TwitterUserId))
                            return true;
                    return false;
                default:
                    return false;
            }
        }
        public double GetSkillLevel(int CategoryId)
        {
            var skill = this.Footprint.Where(c => c.CategoryId == CategoryId).SingleOrDefault();
            if (skill != null)
                return skill.SkillLevel;
            else
                return skillDefaultlevel;

        }
        public void SetSkillLevel(int CategoryId, double newSkillLevel, int postId)
        {
            var diff = 0.0;
            var skill = this.Footprint.Where(c => c.CategoryId == CategoryId).SingleOrDefault();
            if (skill != null)
            {
                diff = newSkillLevel - skill.SkillLevel;
                skill.SkillLevel = newSkillLevel;
            }
            else
            {
                this.Footprint.Add(new Skill
                {
                    SkillLevel = newSkillLevel,
                    CategoryId = CategoryId,
                    AppUserId = this.AppUserId
                });
            }

        }
        //Deep Clone
        public AppUser DeepClone()
        {
            var res = new AppUser
            {
                AppUserId = this.AppUserId,
                UserName = this.UserName,
                PasswordHash = this.PasswordHash,
                SecurityStamp = this.SecurityStamp,
                FirstName = this.FirstName,
                LastName = this.LastName,
                EmailAddress = this.EmailAddress,
                Gender = this.Gender,
                Birthday = this.Birthday,
                Activated = this.Activated,
                SigningUpDate = this.SigningUpDate,
                LastLogInDate = this.LastLogInDate,
                Locale=this.Locale,
                Footprint = new List<Skill>(this.Footprint.Select(x => x.DeepClone())),
                Posts = new List<Post>(this.Posts.Select(x => x.DeepClone())),
                Votes = new List<Vote>(this.Votes.Select(x => x.DeepClone())),
                Notifications = new List<Notification>(this.Notifications.Select(x => x.DeepClone())),
                Friends = new List<AppUser>(this.Friends.Select(x => x.DeepClone())),
                ExternalLogins = new List<ExternalLogin>(this.ExternalLogins.Select(x => x.DeepClone())),
                Newsfeed = new List<Publication>(this.Newsfeed.Select(x => new Publication(x.Post.DeepClone(), x.User.DeepClone()))),
                Badges= new List<BadgeCollected>(this.Badges.Select(x =>x.DeepClone())),
                FacebookDetail = IsProvidedBy(Provider.Facebook) ? this.FacebookDetail.DeepClone() : new FacebookUserDetail(),
                TwitterDetail = IsProvidedBy(Provider.Twitter) ? this.TwitterDetail.DeepClone() : new TwitterUserDetail(),
                Snapshot = this.Snapshot
            };
            return res;
        }


        public string Id
        {
            get { return this.AppUserId.ToString(); }
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }





        public int GetId()
        {
            return this.AppUserId;
        }
    }
}
