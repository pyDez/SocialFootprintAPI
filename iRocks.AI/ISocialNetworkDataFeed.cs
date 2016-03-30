using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRocks.DataLayer;

namespace iRocks.AI
{
    public interface ISocialNetworkDataFeed
    {
        Task FeedData(AppUser dbUser);

    }
    public class FacebookFeeder : Attribute { }
    public class TwitterFeeder : Attribute { }
}
