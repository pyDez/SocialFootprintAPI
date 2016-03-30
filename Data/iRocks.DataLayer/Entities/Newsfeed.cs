using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class Newsfeed : DapperManagedObject<Newsfeed>, IGetId
    {
        public int NewsfeedId {get; set;}
        public int AppUserId {get; set;}
        public int PostId {get; set;}
        public int GetId()
        {
            return NewsfeedId;
        }
    }
}
