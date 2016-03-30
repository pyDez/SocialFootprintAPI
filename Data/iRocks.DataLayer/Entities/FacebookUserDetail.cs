using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class FacebookUserDetail : DapperManagedObject<FacebookUserDetail>, IGetId, IDeeplyCloneable<FacebookUserDetail>
    {
        public FacebookUserDetail()
        {
            
        }
        
        public int FacebookUserDetailId { get; set; }
        
        public int AppUserId { get; set; }
        
        public string FacebookAccessToken { get; set; }
        public string FacebookUserId { get; set; }
        
        public bool IsNew
        {
            get
            {
                return this.FacebookUserDetailId == default(int);
            }
        }
        
        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public FacebookUserDetail DeepClone()
        {
            var res = new FacebookUserDetail()
            {
                FacebookUserDetailId = this.FacebookUserDetailId,
                AppUserId = this.AppUserId,
                FacebookAccessToken = this.FacebookAccessToken
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return FacebookUserDetailId;
        }

    }
}
