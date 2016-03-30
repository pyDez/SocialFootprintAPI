using iRocks.AI;
using iRocks.AI.Helpers.Loging;
using iRocks.DataLayer;
using iRocks.WebAPI.Filters;
using iRocks.WebAPI.Helpers;
using iRocks.WebAPI.Models;
using NClassifier;
using Ninject;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace iRocks.WebAPI.Controllers
{
    public class NewsFeedController : BaseApiController
    {
        const int PAGE_SIZE = 10;
        private INewsFeedSorter _sorter;
        private ITrainableClassifier _Classifier;
        private ModelFactory _factory;
        private INewsFeedHelper _NewsFeedHelper;

        public NewsFeedController(INewsFeedHelper newsFeedHelper, IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository,
            INewsFeedSorter Sorter, ITrainableClassifier classifier)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            _sorter = Sorter;
            _Classifier = classifier;
            _factory = new ModelFactory();
            _NewsFeedHelper = newsFeedHelper;
        }

        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public async Task<object> Post([FromBody]List<int> PostsBlackList, int page = 0, bool update = false, string topFlop = "")
        {
            if (PostsBlackList == null)
                PostsBlackList = new List<int>();


            KeyValuePair<AppUser, IEnumerable<Publication>> UserAndPost = await _NewsFeedHelper.GetUsefullPosts(User.Identity.Name, PostsBlackList, update);


            IEnumerable<Duel> newsFeed = new List<Duel>();
            switch (topFlop)
            {
                case "top":
                    newsFeed = _sorter.GetNewsFeed(UserAndPost.Value.Where(p => !PostsBlackList.Contains(p.Post.PostId) && p.Post.Score > 0).ToList(), UserAndPost.Key.Votes, PAGE_SIZE);
                    break;
                case "flop":
                    newsFeed = _sorter.GetNewsFeed(UserAndPost.Value.Where(p => !PostsBlackList.Contains(p.Post.PostId) && p.Post.Score < 0).ToList(), UserAndPost.Key.Votes, PAGE_SIZE);
                    break;
                default:
                    topFlop = "";
                    newsFeed = _sorter.GetNewsFeed(UserAndPost.Value.Where(p => !PostsBlackList.Contains(p.Post.PostId)).ToList(), UserAndPost.Key.Votes, PAGE_SIZE);
                    break;
            }
            if (UserAndPost.Key.IsProvidedBy(Provider.Facebook))
                _factory.AccessToken = UserAndPost.Key.FacebookDetail.FacebookAccessToken;
            return FormatResult(newsFeed, page, topFlop, UserAndPost.Key.Locale);
        }


        private object FormatResult(IEnumerable<Duel> newsFeed, int page, string topFlop, string locale )
        {
            List<DuelModel> models = new List<DuelModel>();
            var helper = new UrlHelper(Request);
            var prevUrl = page > 0 ? helper.Link("NewsFeed", new { page = page - 1 }) : "";
            var nextUrl = helper.Link("NewsFeed", new { page = page + 1, topFlop = topFlop }); //page < totalPages - 1 ? helper.Link("NewsFeed", new { page = page + 1 }) : "";
            newsFeed.ToList().ForEach(d => models.Add(_factory.Create(d, locale)));


            return new
            {
                
                PrevPageUrl = prevUrl,
                NextPageUrl = nextUrl,
                Results = models
            };
        }

     
    }
}
