using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class Vote : DapperManagedObject<Vote>, IGetId, IDeeplyCloneable<Vote>
    {
        public Vote()
        {

        }
        
        public int VoteId { get; set; }
        
        public int AppUserId { get; set; }
        
        public int UpPostId { get; set; }
        
        public int DownPostId { get; set; }
        
        public bool IsNew
        {
            get
            {
                return this.VoteId == default(int);
            }
        }
        
        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public Vote DeepClone()
        {
            var res = new Vote()
            {
                VoteId = this.VoteId,
                AppUserId = this.AppUserId,
                UpPostId = this.UpPostId,
                DownPostId = this.DownPostId,
                
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return VoteId;
        }

    }
}
