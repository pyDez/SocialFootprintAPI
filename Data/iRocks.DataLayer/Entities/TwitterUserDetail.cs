using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class TwitterUserDetail : DapperManagedObject<TwitterUserDetail>, IGetId, IDeeplyCloneable<TwitterUserDetail>
    {
        public TwitterUserDetail()
        {
            
        }
        
        public int TwitterUserDetailId { get; set; }
        
        public int AppUserId { get; set; }
        
        public string TwitterAccessToken { get; set; }
        public string TwitterAccessTokenSecret { get; set; }
        
        public string TwitterUserId { get; set; }
        public string ScreenName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public bool IsNew
        {
            get
            {
                return this.TwitterUserDetailId == default(int);
            }
        }
        
        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public TwitterUserDetail DeepClone()
        {
            var res = new TwitterUserDetail()
            {
                TwitterUserDetailId = this.TwitterUserDetailId,
                AppUserId = this.AppUserId,
                TwitterAccessToken = this.TwitterAccessToken,
                TwitterAccessTokenSecret =this.TwitterAccessTokenSecret,
                TwitterUserId = this.TwitterUserId,
                ScreenName = this.ScreenName,
                Description = this.Description,
                Url = this.Url
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return TwitterUserDetailId;
        }

    }
}
