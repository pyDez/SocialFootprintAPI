using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class PostMedia : DapperManagedObject<PostMedia>, IGetId, IDeeplyCloneable<PostMedia>
    {
        public PostMedia()
        {
        }
        public int PostMediaId { get; set; }
        public string PostMediaTwitterId { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public int PostId { get; set; }
        public string Provider { get; set; }
        public bool IsNew
        {
            get
            {
                return this.PostMediaId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public PostMedia DeepClone()
        {
            var res = new PostMedia()
            {
                PostMediaId = this.PostMediaId,
                PostMediaTwitterId = this.PostMediaTwitterId,
                Url = this.Url,
                Type = this.Type,
                PostId = this.PostId,
                Provider = this.Provider
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return PostMediaId;
        }
    }
}
