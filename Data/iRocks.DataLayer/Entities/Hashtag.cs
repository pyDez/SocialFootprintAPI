using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class Hashtag : DapperManagedObject<Hashtag>, IGetId, IDeeplyCloneable<Hashtag>
    {
        public Hashtag()
        {
        }
        public int HashtagId { get; set; }
        public string Text { get; set; }
        public int PostId { get; set; }
        public string Provider { get; set; }
        public bool IsNew
        {
            get
            {
                return this.HashtagId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public Hashtag DeepClone()
        {
            var res = new Hashtag()
            {
                HashtagId = this.HashtagId,
                Text = this.Text,
                PostId = this.PostId,
                Provider = this.Provider
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return HashtagId;
        }
    }
}
