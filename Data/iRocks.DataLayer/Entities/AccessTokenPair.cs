using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iRocks.DataLayer
{

    public class AccessTokenPair : DapperManagedObject<AccessTokenPair>, IGetId, IDeeplyCloneable<AccessTokenPair>
    {
        public AccessTokenPair()
        {
        }
        public int AccessTokenPairId { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public bool IsNew
        {
            get
            {
                return this.AccessTokenPairId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public AccessTokenPair DeepClone()
        {
            var res = new AccessTokenPair()
            {
                AccessToken = this.AccessToken,
                AccessTokenSecret = this.AccessTokenSecret
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return AccessTokenPairId;
        }
    }
}
