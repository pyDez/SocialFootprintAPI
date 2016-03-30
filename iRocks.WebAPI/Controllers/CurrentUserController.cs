using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using iRocks.DataLayer;
using iRocks.WebAPI.Services;
using iRocks.WebAPI.Filters;
using iRocks.WebAPI.Models;
using iRocks.AI.Helpers.Loging;

namespace iRocks.WebAPI.Controllers
{

    public class CurrentUserController : BaseApiController
    {
        private ModelFactory factory;
        public CurrentUserController(IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            factory = new ModelFactory();
        }
        [iRocksAuthorizeAttribute]
        [LoggingAspect]
        public HttpResponseMessage Get(string dephtLevel = "Friends")
        {
            AppUser result;
            switch (dephtLevel)
            {
                case "UserBasic":
                    result = TheUserRepository.Select(DephtLevel.UserBasic, new { UserName = User.Identity.Name }).SingleOrDefault();
                    break;
                case "UserFootprint":
                    result = TheUserRepository.Select(DephtLevel.UserFootprint, new { UserName = User.Identity.Name }).SingleOrDefault();
                    break;
                case "UserProfile":
                    result = TheUserRepository.Select(DephtLevel.UserProfile, new { UserName = User.Identity.Name }).SingleOrDefault();
                    break;
                case "Friends":
                    result = TheUserRepository.Select(DephtLevel.Friends, new { UserName = User.Identity.Name }).SingleOrDefault();
                    break;
                case "NewsFeed":
                    result = TheUserRepository.Select(DephtLevel.NewsFeed, new { UserName = User.Identity.Name }).SingleOrDefault();
                    break;
                default:
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            result.Posts = result.Posts.OrderByDescending(p => p.IsProvidedBy(Provider.Facebook) ? p.FacebookDetail.UpdateTime : p.IsProvidedBy(Provider.Twitter) ? p.TwitterDetail.CreationTime : p.CreationDate).ToList();
            if (result.IsProvidedBy(Provider.Facebook))
                factory.AccessToken = result.FacebookDetail.FacebookAccessToken;
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                isAuthenticated = true,
                Result = factory.Create(result, result.Locale)
            });


        }


    }
}
