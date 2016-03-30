using iRocks.AI;
using iRocks.AI.Helpers.Loging;
using iRocks.DataLayer;
using iRocks.WebAPI.Filters;
using iRocks.WebAPI.Models;
using iRocks.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iRocks.WebAPI.Controllers
{
    public class PostController : BaseApiController
    {
        private ModelFactory _factory;
        public PostController(IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            _factory = new ModelFactory();
        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Get() // get all authenticated user posts encapsulated in PublicationModel
        {
            var currentUser = TheUserRepository.Select(DephtLevel.Friends, new { UserName = User.Identity.Name }).SingleOrDefault();

            if (currentUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            var results = new List<Publication>();
            currentUser.Posts.ForEach(p => results.Add(new Publication(p, currentUser)));

            List<PublicationModel> models = new List<PublicationModel>();
            results.ForEach(p => models.Add(_factory.Create(p, currentUser.Locale)));
            return Request.CreateResponse(HttpStatusCode.OK, models); 
            
        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Get(int id)
        {
            var currentUser = TheUserRepository.Select(DephtLevel.NewsFeed, new { UserName = User.Identity.Name }).SingleOrDefault();

            if (currentUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var publications = new List<Publication>();
            currentUser.Posts.ForEach(p => publications.Add(new Publication(p, currentUser)));
            publications.AddRange(currentUser.Newsfeed);

            var result = publications.Where(p => p.Post.PostId == id).FirstOrDefault();

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            if (currentUser.IsProvidedBy(Provider.Facebook))
                _factory.AccessToken = currentUser.FacebookDetail.FacebookAccessToken;
            return Request.CreateResponse(HttpStatusCode.OK, _factory.Create(result, currentUser.Locale));

        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Post(int userId, [FromBody]Post entity)
        {
            try
            {
                //var entity = TheModelFactory.Parse(model, userId);
                entity.AppUserId = userId;
                if (entity == null) Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read post in body");

                //if (userId != _identityService.CurrentUserId) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Could not Post for another user");
                // Save the new Entry
                try 
                {
                    ThePostRepository.Update(entity);
                    return Request.CreateResponse(HttpStatusCode.Created, _factory.Create(entity));
                }
                catch(Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to the database: " +ex.Message );
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


    }
}
