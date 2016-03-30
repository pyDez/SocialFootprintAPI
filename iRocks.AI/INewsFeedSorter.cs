using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.AI
{
    public interface INewsFeedSorter
    {
        IEnumerable<Duel> GetNewsFeed(List<Publication> posts, List<Vote> votes, int take);
        Publication GetMatchedPost(List<Publication> posts, Publication postToMatch);
    }
}
