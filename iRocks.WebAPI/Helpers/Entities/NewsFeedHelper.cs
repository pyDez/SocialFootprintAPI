using iRocks.AI;
using iRocks.DataLayer;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace iRocks.WebAPI.Helpers
{
    public class NewsFeedHelper: INewsFeedHelper
    {
        private IUserRepository _userRepository;
        private IPostRepository _postRepository;
        private ISocialNetworkDataFeed _fbDataFeeder;
        private ISocialNetworkDataFeed _tDataFeeder;
        public NewsFeedHelper(IUserRepository userRepository, IPostRepository postRepository, [FacebookFeeder] ISocialNetworkDataFeed fbDataFeeder, [TwitterFeeder] ISocialNetworkDataFeed tDataFeeder)
        {
            this._userRepository = userRepository;
            this._postRepository = postRepository;
            this._fbDataFeeder = fbDataFeeder;
            this._tDataFeeder = tDataFeeder;
        }
        public async Task<KeyValuePair<AppUser, IEnumerable<Publication>>> GetUsefullPosts(string UserName, List<int> PostsBlackList, bool update)
        {
            if (PostsBlackList == null)
                PostsBlackList = new List<int>();

            AppUser user;
            if (update)
            {
                user = _userRepository.Select(DephtLevel.UserBasic, new { UserName = UserName }).SingleOrDefault();
                if(user.IsProvidedBy(Provider.Facebook))
                    await _fbDataFeeder.FeedData(user);
                if (user.IsProvidedBy(Provider.Twitter))
                    await _tDataFeeder.FeedData(user);

            }
            user = _userRepository.Select(DephtLevel.NewsFeed, new { UserName = UserName }).SingleOrDefault();
            user.Newsfeed.Where(p=>p.Post.IsProvidedBy(Provider.Facebook)).ToList().ForEach(p => p.Post.FacebookDetail.setAccessTokenToPictureUrl(user.FacebookDetail.FacebookAccessToken));
            IEnumerable<Publication> posts = user.Newsfeed;

            if (update && PostsBlackList.Count() > 0)
            {
                var BLPosts = _postRepository.Select(new { PostId = PostsBlackList });
                posts = user.Newsfeed.Where
                    (
                        p => (p.Post.IsProvidedBy(Provider.Facebook)? p.Post.FacebookDetail.UpdateTime: p.Post.IsProvidedBy(Provider.Twitter) ? p.Post.TwitterDetail.CreationTime : p.Post.CreationDate)> BLPosts
                        .Select(blp => blp.IsProvidedBy(Provider.Facebook) ? blp.FacebookDetail.UpdateTime : blp.IsProvidedBy(Provider.Twitter) ? blp.TwitterDetail.CreationTime : blp.CreationDate)
                        .Max()
                    );
            }

            posts.ToList().ForEach(p => p.filterStories(user.Locale));
            return new KeyValuePair<AppUser, IEnumerable<Publication>>( user, posts);
        }
    }
}