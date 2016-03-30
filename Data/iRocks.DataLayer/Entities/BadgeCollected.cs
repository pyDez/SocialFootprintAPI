using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iRocks.DataLayer
{

    public class BadgeCollected : DapperManagedObject<BadgeCollected>, IGetId, IDeeplyCloneable<BadgeCollected>
    {
        public BadgeCollected()
        {

        }
        public int BadgeCollectedId { get; set; }
        public int AppUserId { get; set; }
        public int BadgeId { get; set; }
        public int? PostId { get; set; }
        public Badge Badge { get; set; }
        public DateTime CollectDate { get; set; }
        public bool IsNew
        {
            get
            {
                return this.BadgeCollectedId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public BadgeCollected DeepClone()
        {
            var res = new BadgeCollected()
            {
                BadgeCollectedId = this.BadgeCollectedId,
                AppUserId = this.AppUserId,
                BadgeId = this.BadgeId,
                PostId = this.PostId,
                CollectDate = this.CollectDate,
                Badge = this.Badge != null? this.Badge.DeepClone():null

            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return BadgeCollectedId;
        }
    }
}
