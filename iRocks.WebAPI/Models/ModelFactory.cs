using iRocks.AI;
using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRocks.WebAPI.Models
{
    public class ModelFactory
    {
        public string AccessToken { get; set; }
        public AppUserModel Create(AppUser user)
        {
            if (user == null)
                return null;
            var model = new AppUserModel()
            {
                AppUserId = user.AppUserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Locale = user.Locale,
                Footprint = user.Footprint,
                Posts = user.Posts.Select(p => Create(p)).ToList(),
                Votes = user.Votes,
                Badges = user.Badges,
                Notifications = user.Notifications.Select(n => Create(n)).ToList(),
                Friends = user.Friends.Select(f => Create(f)).ToList(),
                Newsfeed = user.Newsfeed.Select(p => Create(p)).ToList(),
                TwitterDetail = Create(user.TwitterDetail),

                IsFacebookProvided = user.IsProvidedBy(Provider.Facebook),

                IsTwitterProvided = user.IsProvidedBy(Provider.Twitter)
            };
            if (user.IsProvidedBy(Provider.Facebook))
            {
                model.FacebookDetail = Create(user.FacebookDetail);
                AccessToken = user.FacebookDetail.FacebookAccessToken;
            }
            return model;
        }

        public AppUserModel Create(AppUser user, string locale)
        {
            if (user == null)
                return null;
            user.Badges.ForEach(b => b.Badge = TranslationHelper.GetBadgeTranslation(b.Badge, locale));
            user.Footprint.ForEach(s => s.SkillCategory = TranslationHelper.GetCategoryTranslation(s.SkillCategory, locale));
            var model = Create(user);
            model.Posts = user.Posts.Select(p => Create(p, locale)).ToList();
            model.Friends = user.Friends.Select(f => Create(f, locale)).ToList();
            model.Newsfeed = user.Newsfeed.Select(p => Create(p, locale)).ToList();
            return model;
        }

        public NotificationModel Create(DataLayer.Notification notification)
        {
            if (notification == null)
                return null;
            var model = new NotificationModel()
            {
                NotificationId = notification.NotificationId,
                AppUserId = notification.AppUserId,
                Information = notification.Information,
                IsRed = notification.IsRed,
                NotificationDate = notification.NotificationDate,

                ObjectType = notification.ObjectType,
                ObjectId = notification.ObjectId,
               // ObjectEntity = notification.ObjectEntity,
                IsOld = (DateTime.Now - notification.NotificationDate).Days>30 && !notification.IsRed
            };
            //switch(notification.ObjectType)
            //{
            //    case "Post":
            //        model.ObjectEntity = Create((Publication)notification.ObjectEntity);
            //        break;
            //    case "AppUser":
            //        model.ObjectEntity = Create((AppUser)notification.ObjectEntity);
            //        break;
            //    case "Badge":
            //        model.ObjectEntity = notification.ObjectEntity;
            //        break;
            //    default:
            //        break;
            //}
            return model;
        }

        public PublicationModel Create(Publication publication)
        {
            if (publication == null)
                return null;
            return new PublicationModel()
            {
                Post = Create(publication.Post),
                User = Create(publication.User)

            };
        }
        public PublicationModel Create(Publication publication, string locale)
        {
            if (publication == null)
                return null;
            return new PublicationModel()
            {
                Post = Create(publication.Post, locale),
                User = Create(publication.User, locale)

            };
        }
        public DuelModel Create(Duel duel)
        {
            if (duel == null)
                return null;
            return new DuelModel()
            {
                FirstPublication = Create(duel.FirstPublication),
                SecondPublication = Create(duel.SecondPublication),
                DuelResult = duel.DuelResult,
                CategoryLabel = duel.FirstPublication.Post.PostCategory.Label
            };
        }
        public DuelModel Create(Duel duel, string locale)
        {
            if (duel == null)
                return null;
            return new DuelModel()
            {
                FirstPublication = Create(duel.FirstPublication, locale),
                SecondPublication = Create(duel.SecondPublication, locale),
                DuelResult = duel.DuelResult,
                CategoryLabel = TranslationHelper.GetCategoryTranslation(duel.FirstPublication.Post.PostCategory, locale).Label
            };
        }
        public PostModel Create(Post post)
        {
            if (post == null)
                return null;
            return new PostModel()
            {
                PostId = post.PostId,

                AppUserId = post.AppUserId,

                CategoryId = post.CategoryId,

                CreationDate = post.CreationDate,

                PostCategory = Create(post.PostCategory),

                UpVotes = post.UpVotes,

                DownVotes = post.DownVotes,

                FacebookDetail = Create(post.FacebookDetail),

                TwitterDetail = Create(post.TwitterDetail),

                Score = post.Score,

                IsFacebookProvided = post.IsProvidedBy(Provider.Facebook),

                IsTwitterProvided = post.IsProvidedBy(Provider.Twitter)
            };
        }

        public PostModel Create(Post post, string locale )
        {
            if (post == null)
                return null;
            var model = Create(post);

            model.PostCategory = Create(post.PostCategory, locale);

            model.FacebookDetail = Create(post.FacebookDetail, locale);
            model.TwitterDetail = Create(post.TwitterDetail, locale);

            return model;
            
        }
        public CategoryModel Create(Category category)
        {
            if (category == null)
                return null;
            return new CategoryModel()
            {
                CategoryId = category.CategoryId,

                Label = category.Label


            };
        }
        public CategoryModel Create(Category category, string locale)
        {
            if (category == null)
                return null;

            return Create(TranslationHelper.GetCategoryTranslation(category, locale));
        }
        public FacebookUserDetailModel Create(FacebookUserDetail facebookUserDetail)
        {
            if (facebookUserDetail == null)
                return null;
            return new FacebookUserDetailModel()
            {
                FacebookUserId = facebookUserDetail.FacebookUserId

            };
        }
        public FacebookPostDetailModel Create(FacebookPostDetail facebookPostDetail)
        {
            if (facebookPostDetail == null)
                return null;
            var model = new FacebookPostDetailModel()
            {
                FacebookPostDetailId = facebookPostDetail.FacebookPostDetailId,

                PostId = facebookPostDetail.PostId,

                FacebookPostId = facebookPostDetail.FacebookPostId,

                Link = facebookPostDetail.Link,
                Caption = facebookPostDetail.Caption,

                Message = facebookPostDetail.Message,

                LinkName = facebookPostDetail.LinkName,

                AttachedObjectId = facebookPostDetail.AttachedObjectId,

                AttachedObjectUrl = facebookPostDetail.AttachedObjectUrl,

                Picture = facebookPostDetail.Picture,

                Privacy = facebookPostDetail.Privacy,
                VideoSource = facebookPostDetail.VideoSource,
                StatusType = facebookPostDetail.StatusType,

                Stories = facebookPostDetail.Stories,

                AnonymousStory = facebookPostDetail.AnonymousStory,

                GeneralStatusType = facebookPostDetail.GeneralStatusType,

                UpdateTime = facebookPostDetail.UpdateTime,

                ChildPostId = facebookPostDetail.ChildPostId,
                ChildPublication = Create(facebookPostDetail.ChildPublication),

                Target = facebookPostDetail.Target

            };
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                if (facebookPostDetail.VideoSource != null)
                {
                    if (facebookPostDetail.VideoSource.Contains("fbcdn"))
                    {

                        model.VideoSource = FacebookVideoHelper.GetVideoSource(facebookPostDetail.VideoSource, AccessToken, facebookPostDetail.FacebookPostId);
                    }
                    else
                        model.VideoSource = facebookPostDetail.VideoSource;
                }
            }
            return model;
        }
        public FacebookPostDetailModel Create(FacebookPostDetail facebookPostDetail, string locale)
        {
            if (facebookPostDetail == null)
                return null;
            var model = Create(facebookPostDetail);
            model.ChildPublication = Create(facebookPostDetail.ChildPublication, locale);
           
            return model;
        }

        public TwitterUserDetailModel Create(TwitterUserDetail facebookUserDetail)
        {
            if (facebookUserDetail == null)
                return null;
            return new TwitterUserDetailModel()
            {
                TwitterUserId = facebookUserDetail.TwitterUserId,
                Url = facebookUserDetail.Url,
                Description = facebookUserDetail.Description,
                ScreenName = facebookUserDetail.ScreenName

            };
        }
        public TwitterPostDetailModel Create(TwitterPostDetail twitterPostDetail)
        {
            if (twitterPostDetail == null)
                return null;
            var model = new TwitterPostDetailModel()
            {
                TwitterPostDetailId = twitterPostDetail.TwitterPostDetailId,

                PostId = twitterPostDetail.PostId,

                TwitterPostId = twitterPostDetail.TwitterPostId,

                CreationTime= twitterPostDetail.CreationTime,
                RetweetedPostId = twitterPostDetail.RetweetedPostId,
                RetweetedPublication = Create(twitterPostDetail.RetweetedPublication),
                Text = twitterPostDetail.Text,
                Hashtags = twitterPostDetail.Hashtags.Select(h=>h.Text).ToList(),
                Medias = twitterPostDetail.Medias.Select(h => h.Url).ToList(),
                Urls = twitterPostDetail.Urls.Select(u => u.Url).ToList()

            };
            foreach(var user in twitterPostDetail.MentionedUsers)
            {
                if (user.IsProvidedBy(Provider.Twitter) && !model.MentionedUsers.ContainsKey(user.TwitterDetail.ScreenName))
                    model.MentionedUsers.Add(user.TwitterDetail.ScreenName, user.AppUserId);
            }
            return model;
        }
        public TwitterPostDetailModel Create(TwitterPostDetail twitterPostDetail, string locale)
        {
            if (twitterPostDetail == null)
                return null;
            var model = Create(twitterPostDetail);
            model.RetweetedPublication = Create(twitterPostDetail.RetweetedPublication, locale);

            return model;
        }

    }
}