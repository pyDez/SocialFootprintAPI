﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iRocks.DataLayer;
using iRocks.WebAPI.Filters;
using iRocks.WebAPI.Models;
using iRocks.AI;
using iRocks.WebAPI.Helpers;
using System.Threading.Tasks;
using NClassifier;
using iRocks.AI.Helpers.Loging;

namespace iRocks.WebAPI.Controllers
{
    public class CategoryController : BaseApiController
    {

        private INewsFeedSorter _sorter;
        private ITrainableClassifier _classifier;
        private ModelFactory _factory;
        private INewsFeedHelper _NewsFeedHelper;
        public CategoryController(INewsFeedHelper newsFeedHelper, IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository, INewsFeedSorter Sorter, ITrainableClassifier Classifier)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            this._sorter = Sorter;
            this._classifier = Classifier;
            this._factory = new ModelFactory();
            this._NewsFeedHelper = newsFeedHelper;
        }
        [LoggingAspect]
        public HttpResponseMessage Get()
        {
            var result = TheCategoryRepository.Select();

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            List<CategoryModel> models = new List<CategoryModel>();
            result.ToList().ForEach(c => models.Add(_factory.Create(c)));
            return Request.CreateResponse(HttpStatusCode.OK, models);
            
        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Get(string locale)
        {
            var result = TheCategoryRepository.Select();


            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            List<CategoryModel> models = new List<CategoryModel>();
            result.ToList().ForEach(c => models.Add(_factory.Create(c, locale)));

            return Request.CreateResponse(HttpStatusCode.OK, models);

        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public async Task<HttpResponseMessage> Post([FromBody]CategorizationModel entity)
        {
            try
            {
                if (entity == null) Request.CreateResponse(HttpStatusCode.BadRequest, "Could not read categorization Model in body");

                //apply categorization modification
                var category = TheCategoryRepository.Select(new { CategoryId = entity.CategoryId }).FirstOrDefault();
                if (category == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Could not found given Category");
                var post = ThePostRepository.Select(new { PostId = entity.PostId }).FirstOrDefault();
                if (post == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Could not found given post");
                post.CategoryId = category.CategoryId;
                post.PostCategory = category;

                try
                {
                    // Save the new Entry
                    ThePostRepository.Update(post);
                    TeachCategorizerRecursive(post, category);

                    //get a  match post for dueling

                    entity.PostsBlackList.Add(post.PostId);
                    KeyValuePair<AppUser, IEnumerable<Publication>> UserAndPost = await _NewsFeedHelper.GetUsefullPosts(User.Identity.Name, entity.PostsBlackList, false);
                    
                     var postUser = TheUserRepository.Select(DephtLevel.NewsFeed, new { AppUserId = post.AppUserId }).FirstOrDefault();

                     Publication initialpost = new Publication(post, postUser);
                     Duel duel = new Duel(initialpost, _sorter.GetMatchedPost(UserAndPost.Value.Where(p => !entity.PostsBlackList.Contains(p.Post.PostId)).ToList(), initialpost));

                    if (UserAndPost.Key.IsProvidedBy(Provider.Facebook))
                        _factory.AccessToken = UserAndPost.Key.FacebookDetail.FacebookAccessToken;
                    return Request.CreateResponse(HttpStatusCode.OK, _factory.Create(duel, UserAndPost.Key.Locale));
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to the database: " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

      private void TeachCategorizerRecursive(Post post, Category category)
        {
            if (post.IsProvidedBy(Provider.Facebook))
            {
                if (!String.IsNullOrWhiteSpace(post.FacebookDetail.Message))
                    _classifier.TeachMatchAsync(category, post.FacebookDetail.Message);
                if (!String.IsNullOrWhiteSpace(post.FacebookDetail.LinkName))
                    _classifier.TeachMatchAsync(category, post.FacebookDetail.LinkName);
                if (post.FacebookDetail.ChildPublication != null)
                {
                    TeachCategorizerRecursive(post.FacebookDetail.ChildPublication.Post, category);
                }
            }
            if (post.IsProvidedBy(Provider.Twitter))
            {
                if (!String.IsNullOrWhiteSpace(post.TwitterDetail.Text))
                    _classifier.TeachMatchAsync(category, post.TwitterDetail.Text);
                if (post.TwitterDetail.RetweetedPublication != null)
                {
                    TeachCategorizerRecursive(post.TwitterDetail.RetweetedPublication.Post, category);
                }
            }
            
        }

    }
}
