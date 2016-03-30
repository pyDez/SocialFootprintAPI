using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Services;
using iRocks.WebAPI.Filters;
using iRocks.WebAPI.Models;
using iRocks.AI.Helpers.Loging;

namespace iRocks.WebAPI.Controllers
{
    public class VoteController : BaseApiController
    {
        private IRanking _rankingHelper;
        private IBadgeRepository _badgeRepository;
        private IBadgeCollectedRepository _badgeCollectedRepository;
        private INotificationRepository _notificationRepository;
        private ModelFactory _factory;

        public VoteController(IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository,
                              IRanking rankingHelper, IBadgeRepository badgeRepository, IBadgeCollectedRepository badgeCollectedRepository, INotificationRepository notificationRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            _rankingHelper = rankingHelper;
            _badgeCollectedRepository = badgeCollectedRepository;
            _badgeRepository = badgeRepository;
            _notificationRepository = notificationRepository;
            _factory = new ModelFactory();
        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public IEnumerable<Vote> Get()
        {
            var task = TheUserRepository.FindByNameAsync(User.Identity.Name);
            AppUser currentUser = task.Result;
            return TheVoteRepository.Select(new { AppUserId = currentUser.AppUserId }).ToList();

        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Get(int postId)
        {
            var currentUser = TheUserRepository.Select(DephtLevel.NewsFeed, new { UserName = User.Identity.Name }).SingleOrDefault();

            if (currentUser.Newsfeed.Where(p => p.Post.PostId == postId).Any())
            {
                var votes = TheVoteRepository.Select(new { UpPostId = postId, DownPostId = postId }, SQLKeyWord.Or);


                if (votes == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                var voters = TheUserRepository.Select(DephtLevel.UserBasic, new { AppUserId = votes.Select(v => v.AppUserId).ToList() });
                var results = new List<VoterModel>();
                foreach (var vote in votes)
                {
                    var voter = voters.Where(v => v.AppUserId == vote.AppUserId).FirstOrDefault();
                    if (voter != null)
                        results.Add(new VoterModel(vote, _factory.Create(voter)));
                }

                return Request.CreateResponse(HttpStatusCode.OK, results);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);

        }
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Post([FromBody]Vote entity)
        {
            try
            {
                var currentUser = TheUserRepository.Select(DephtLevel.Friends, new { UserName = User.Identity.Name }).SingleOrDefault();

                if (entity == null) Request.CreateResponse(HttpStatusCode.BadRequest, "Could not read Vote in body");
                entity.AppUserId = currentUser.AppUserId;

                var votedPosts = ThePostRepository.Select(new { PostId = new List<int>() { entity.DownPostId, entity.UpPostId } });
                var downPost = votedPosts.Where(p => p.PostId == entity.DownPostId).SingleOrDefault();
                var upPost = votedPosts.Where(p => p.PostId == entity.UpPostId).SingleOrDefault();
                if (downPost == null) return Request.CreateResponse(HttpStatusCode.NotFound, "Could not found Down Voted Post");
                if (upPost == null) return Request.CreateResponse(HttpStatusCode.NotFound, "Could not found Up Voted Post");
                //if (entity.AppUserId != _identityService.CurrentUserName) return Request.CreateResponse(HttpStatusCode.Unauthorized, "You are not the connected user");

                //apply Vote modification
                List<int> downUserIds = GetImplicatedUserIdsRecursive(downPost).ToList();
                List<int> upUserIds = GetImplicatedUserIdsRecursive(upPost).ToList();
                var implicatedUsers = TheUserRepository.Select(DephtLevel.Friends, new { AppUserId = downUserIds.Concat(upUserIds).ToList() });
                var downUsers = implicatedUsers.Where(u => downUserIds.Contains(u.AppUserId));
                var upUsers = implicatedUsers.Where(u => upUserIds.Contains(u.AppUserId));
                var mainDownUser = downUsers.Where(u => u.AppUserId == downPost.AppUserId).FirstOrDefault();
                var mainUpUser = upUsers.Where(u => u.AppUserId == upPost.AppUserId).FirstOrDefault();
                if (downUsers.Count()<=0 || mainDownUser==null) return Request.CreateResponse(HttpStatusCode.NoContent, "Could not found Down Voted User");
                if (upUsers.Count() <= 0 || mainUpUser == null) return Request.CreateResponse(HttpStatusCode.NoContent, "Could not found Up Voted User");

                if (entity.AppUserId == upPost.AppUserId || entity.AppUserId == downPost.AppUserId) return Request.CreateResponse(HttpStatusCode.Conflict, "You can not vote for one of your post");

                foreach (var downUser in downUsers)
                {
                    var newRank = _rankingHelper.getNewRankings(downUser.GetSkillLevel(downPost.CategoryId), mainUpUser.GetSkillLevel(upPost.CategoryId), false);
                    downUser.SetSkillLevel(downPost.CategoryId, newRank.Item1, downPost.PostId);
                   
                    var newNotification = new DataLayer.Notification()
                    {
                        IsRed = false,
                        AppUserId = downUser.AppUserId,
                        ObjectType = NotificationObject.Post.ToString(),
                        ObjectId = downPost.PostId,
                        Information = TranslationHelper.GetTranslation(downUser.Locale, "VOTE_NOTIFICATION_2")

                    };
                    downUser.Notifications.Add(newNotification);
                }

                foreach(var upUser in upUsers)
                {
                    var newRank = _rankingHelper.getNewRankings(mainDownUser.GetSkillLevel(downPost.CategoryId), upUser.GetSkillLevel(upPost.CategoryId), false);
                    upUser.SetSkillLevel(upPost.CategoryId, newRank.Item2, upPost.PostId);

                    var newNotification = new DataLayer.Notification()
                    {
                        IsRed = false,
                        AppUserId = upUser.AppUserId,
                        ObjectType = NotificationObject.Post.ToString(),
                        ObjectId = upPost.PostId,
                        Information = TranslationHelper.GetTranslation(upUser.Locale, "VOTE_NOTIFICATION_1")

                    };
                    upUser.Notifications.Add(newNotification);
                }

                // Save the new Entry and add badges if suitable
                try
                {


                    TheVoteRepository.Insert(entity);
                    currentUser.Votes.Add(entity);//on le comptabilise pour l'ajout de badge
                    
                    var skills = mainDownUser.Footprint.Where(s => s.CategoryId == downPost.CategoryId).Concat(mainUpUser.Footprint.Where(s => s.CategoryId == upPost.CategoryId));
                    foreach (var skill in skills)
                    {
                        skill.SkillCategory = TranslationHelper.GetCategoryTranslation(skill.SkillCategory, currentUser.Locale);
                    }

                    var badges = _badgeRepository.Select();
                    var categories = TheCategoryRepository.Select();
                    var recompense = BadgeHelper.AddCurrentUserBadge(currentUser, entity, badges.ToList(), _badgeCollectedRepository, _notificationRepository);

                    foreach (var user in downUsers.Concat(upUsers))
                    {
                        TheUserRepository.Save(DephtLevel.Friends, user);

                        BadgeHelper.AddCategoryBadge(user, badges.ToList(), categories.ToList(), _badgeCollectedRepository, _notificationRepository);

                        var post = GetAssociatedPost(downPost, user.AppUserId);
                        if (post == null)
                        {
                            post = GetAssociatedPost(upPost, user.AppUserId);
                        }
                        if (post != null)
                        {
                            BadgeHelper.AddPostBadge(user, post, badges.ToList(), _badgeCollectedRepository, _notificationRepository);
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.Created, new VoteResponseModel(skills, recompense != null ? recompense.Item1 : null, recompense != null ? recompense.Item2 : null));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Could not save to the database: " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        private IEnumerable<int> GetImplicatedUserIdsRecursive(Post post)
        {
            List<int> userIds = new List<int>();
            if (post.IsProvidedBy(Provider.Facebook))
            {
                if (post.FacebookDetail.ChildPublication != null)
                {
                    userIds = userIds.Concat(GetImplicatedUserIdsRecursive(post.FacebookDetail.ChildPublication.Post)).ToList();
                }
            }
            if (post.IsProvidedBy(Provider.Twitter))
            {
                if (post.TwitterDetail.RetweetedPublication != null)
                {
                    userIds = userIds.Concat(GetImplicatedUserIdsRecursive(post.TwitterDetail.RetweetedPublication.Post)).ToList();
                }
            }
            userIds.Add(post.AppUserId);
            return userIds;
        }
        private Post GetAssociatedPost(Post post, int appUserId)
        {
            if (post.AppUserId == appUserId)
            {
                return post;
            }
            if (post.IsProvidedBy(Provider.Facebook))
            {
                if (post.FacebookDetail.ChildPublication != null)
                {
                    return GetAssociatedPost(post.FacebookDetail.ChildPublication.Post, appUserId);
                }
            }
            if (post.IsProvidedBy(Provider.Twitter))
            {
                if (post.TwitterDetail.RetweetedPublication != null)
                {
                    return GetAssociatedPost(post.TwitterDetail.RetweetedPublication.Post, appUserId);
                }
            }

            return null;
        }

        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Delete(int Id)
        {
            try
            {
                var task = TheUserRepository.FindByNameAsync(User.Identity.Name);
                AppUser currentUser = task.Result;
                Vote entity = TheVoteRepository.Select(new { VoteId = Id, AppUserId = currentUser.AppUserId }, SQLKeyWord.And).SingleOrDefault();
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                try
                {
                    TheVoteRepository.Delete(entity);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "BDD can not delete this element :" + e.Message);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPatch]
        [HttpPut]
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Patch(int Id, [FromBody]Vote UpdatedVote)
        {
            try
            {
                var task = TheUserRepository.FindByNameAsync(User.Identity.Name);
                AppUser currentUser = task.Result;
                var entity = TheVoteRepository.Select(new { VoteId = Id }).SingleOrDefault();
                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                if (entity.AppUserId != currentUser.AppUserId) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not update Vote for another user");

                if (UpdatedVote == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                if (entity.DownPostId != UpdatedVote.DownPostId || entity.UpPostId != UpdatedVote.UpPostId)
                {
                    entity.DownPostId = UpdatedVote.DownPostId;
                    entity.UpPostId = UpdatedVote.UpPostId;
                    try
                    {
                        TheVoteRepository.Update(entity);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to the database: " + ex.Message);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
