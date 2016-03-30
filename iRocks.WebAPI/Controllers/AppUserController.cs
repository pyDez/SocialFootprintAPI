using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Filters;
using iRocks.WebAPI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using log4net;
using iRocks.AI.Helpers.Loging;
using System.Threading;

namespace iRocks.WebAPI.Controllers
{
   
    public class AppUserController : BaseApiController
    {
        const int PAGE_SIZE = -1;
        private ModelFactory factory;
        


        public AppUserController(IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            factory = new ModelFactory();
        }

        [iRocksAuthorizeAttribute]
        [LoggingAspect]
        public object Get()
        {
            //log.Info("entering Get");
            AppUser user = TheUserRepository.Select(DephtLevel.NewsFeed, new { UserName = User.Identity.Name }).FirstOrDefault();

            if (user != null)
            {
                List<AppUserModel> models = new List<AppUserModel>();
               

                IEnumerable<AppUser> results = user.Newsfeed.Select(x => x.User).GroupBy(y => y.AppUserId).Select(z => z.First());
                if (user.IsProvidedBy(Provider.Facebook))
                    factory.AccessToken = user.FacebookDetail.FacebookAccessToken;
                results.ToList().ForEach(u => models.Add(factory.Create(u, user.Locale)));
               // log.Info("leaving Get");
                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "no corresponding userName");
        }
        [iRocksAuthorizeAttribute]
        [LoggingAspect]
        public HttpResponseMessage Get(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                //IUserRepository repository = BusinessHelper.CreateRepository();
                AppUser user = TheUserRepository.Select(DephtLevel.NewsFeed, new { UserName = User.Identity.Name }).FirstOrDefault();
                if (user != null)
                {
                    List<AppUser> result = new List<AppUser>();
                    if (id == user.AppUserId)
                    {
                        result.Add(user);
                    }
                    else
                    {
                        IEnumerable<int> results = user.Newsfeed.Select(x => x.User.AppUserId);
                        if (results.Contains(id))
                        {
                            result = TheUserRepository.Select(DephtLevel.Friends, new { AppUserId = id }).ToList();
                        }
                        else
                        {
                            result = TheUserRepository.Select(DephtLevel.UserProfile, new { AppUserId = id }).ToList();
                        }
                    }
                    if (result.Count <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    
                    result.First().Posts = result.First().Posts.OrderByDescending(p => p.IsProvidedBy(Provider.Facebook) ? p.FacebookDetail.UpdateTime : p.IsProvidedBy(Provider.Twitter) ? p.TwitterDetail.CreationTime : p.CreationDate).ToList();
                    if(user.IsProvidedBy(Provider.Facebook))
                        factory.AccessToken = user.FacebookDetail.FacebookAccessToken;
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        isAuthenticated = true,

                        Result = factory.Create(result.FirstOrDefault(), user.Locale)
                    });

                }
            }
            var notAuthenticatedResult = TheUserRepository.Select(DephtLevel.UserProfile, new { AppUserId = id });
            if (notAuthenticatedResult == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                isAuthenticated = false,
                Result = factory.Create(notAuthenticatedResult.FirstOrDefault())
            });


        }

        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Post(bool get, [FromBody]JObject criteria)
        {
            try
            {

                if (!get || criteria == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "unable to understand request");

                // Save the new Entry
                var result = TheUserRepository.Select(DephtLevel.Friends, criteria);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                List<AppUserModel> models = new List<AppUserModel>();
                result.ToList().ForEach(u => models.Add(factory.Create(u)));
                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Post([FromBody]AppUser entity)
        {
            try
            {
                if (entity == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "could not read User in body");

                // Save the new Entry
                try
                {
                    TheUserRepository.Save(DephtLevel.Friends, entity);
                    return Request.CreateResponse(HttpStatusCode.Created, factory.Create(entity));

                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to the database : " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }



}
