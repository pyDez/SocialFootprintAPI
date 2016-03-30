using iRocks.AI;
using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace iRocks.WebAPI.Helpers
{
    public interface INewsFeedHelper
    {
         Task<KeyValuePair<AppUser, IEnumerable<Publication>>> GetUsefullPosts(string UserName, List<int> PostsBlackList, bool update);
    }
}