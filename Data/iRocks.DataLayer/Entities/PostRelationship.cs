using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class PostRelationship : DapperManagedObject<PostRelationship>, IGetId, IDeeplyCloneable<PostRelationship>
    {
        public int PostRelationshipId {get; set;}
        public int ParentPostId {get; set;}
        public int TargetedUserId {get; set;}
        public string TargetedUserName {get; set;}

        public AppUser TargetedUser { get; set; }
        public int GetId()
        {
            return PostRelationshipId;
        }

        public PostRelationship DeepClone()
        {
            var res = new PostRelationship()
            {
                PostRelationshipId = this.PostRelationshipId,
                ParentPostId = this.ParentPostId,
                TargetedUserId = this.TargetedUserId,
                TargetedUserName = this.TargetedUserName,
                Snapshot = this.Snapshot
            };
            return res;
        }
    }
}
