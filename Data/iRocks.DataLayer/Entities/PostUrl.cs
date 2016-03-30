using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class PostUrl : DapperManagedObject<PostUrl>, IGetId, IDeeplyCloneable<PostUrl>
    {
        public PostUrl()
        {
        }
        public int PostUrlId { get; set; }
        public string Url { get; set; }
        public int PostId { get; set; }
        public string Provider { get; set; }
        public bool IsNew
        {
            get
            {
                return this.PostUrlId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public PostUrl DeepClone()
        {
            var res = new PostUrl()
            {
                PostUrlId = this.PostUrlId,
                Url = this.Url,
                PostId = this.PostId,
                Provider = this.Provider
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return PostUrlId;
        }
    }
}
