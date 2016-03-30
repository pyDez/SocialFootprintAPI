using iRocks.AI.Helpers;
using iRocks.DataLayer;
using iRocks.DataLayer.Helpers;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iRocks.AI.Entities
{
    public class TwitterDataFeed : ISocialNetworkDataFeed
    {

        private IPostRepository _PostRepository;
        private IUserRepository _UserRepository;
        private ITwitterUserDetailRepository _TwitterUserDetailRepository;
        private INotificationRepository _NotificationRepository;
        private ICategoryRepository _CategoryRepository;
        private IClassifier _Classifier;

        private List<DataLayer.Category> _Categories;
        public TwitterDataFeed(IClassifier classifier, IUserRepository userRepository, IPostRepository postRepository, ITwitterUserDetailRepository twitterUserDetailRepository, ICategoryRepository categoryRepository, INotificationRepository notificationRepository)
        {
            _UserRepository = userRepository;
            _PostRepository = postRepository;
            _TwitterUserDetailRepository = twitterUserDetailRepository;
            _NotificationRepository = notificationRepository;
            _CategoryRepository = categoryRepository;
            _Categories = new List<DataLayer.Category>();
            _Classifier = classifier;
        }


        public async Task FeedData(AppUser dbUser)
        {
            await Task.Run(() => //Get information about posts and friends posts
                   {
                       //int forHowManyYears = 3;
                       dbUser = _UserRepository.Select(DephtLevel.NewsFeed, new { AppUserId = dbUser.AppUserId }).FirstOrDefault();
                       dbUser = FeedShortTermNewsfeed(dbUser);
                        _UserRepository.Save(DephtLevel.NewsFeed, dbUser);
                       //for (int i = 0; i < forHowManyYears; ++i)
                       //{
                       //    dbUser = FeedOneYear(dbUser, DateTime.Today.AddYears(-i), DateTime.Today.AddYears(-i - 1));
                       //    _UserRepository.Save(DephtLevel.NewsFeed, dbUser);
                       //}

                       //Classification
                       var Categories = _CategoryRepository.Select();
                       foreach (var publication in dbUser.Newsfeed)
                       {
                           if (publication.Post.CategoryId == 1)
                           {
                               var bestCategory = _Classifier.Classify(publication.Post, Categories);
                               publication.Post.CategoryId = bestCategory.CategoryId;
                               publication.Post.PostCategory = bestCategory;
                           }
                       }
                       //we have agreggate all data from twitter and DB
                       //we need to delete obsolete data from FB
                       dbUser = CleanDbUser(dbUser);
                       _UserRepository.Save(DephtLevel.NewsFeed, dbUser);
                   });
        }


        private AppUser FeedShortTermNewsfeed(AppUser dbUser)
        {
            try
            {
                var newUser = false;
                if (dbUser == null)
                    throw new NullReferenceException("Db User cannot be null");
                if (dbUser.TwitterDetail == null)
                    throw new NullReferenceException("Db User need TwitterDetails to be fed by Twitter");
                if (!dbUser.Activated)
                    newUser = true;

                dbUser.Activated = true;

                IAuthorizer auth = new SingleUserAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore()
                    {
                        ConsumerKey = GetPartiesIdentificationHelper.TwitterApiKey,
                        ConsumerSecret = GetPartiesIdentificationHelper.TwitterApiSecret,
                        OAuthToken = dbUser.TwitterDetail.TwitterAccessToken,
                        OAuthTokenSecret = dbUser.TwitterDetail.TwitterAccessTokenSecret
                    }
                };
                var twitterCtx = new TwitterContext(auth);
                try
                {
                    var verifyResponse = (from acct in twitterCtx.Account
                                          where acct.Type == AccountType.VerifyCredentials
                                          select acct)
                         .SingleOrDefault();

                    var timelineResponse = (from acct in twitterCtx.Status
                                            where acct.Type == StatusType.Home
                                            select acct)
                         .ToList();

                    if (verifyResponse != null && verifyResponse.User != null)
                    {
                        AppUser tUser = ConvertFullUserFromTwitterToAppUser(verifyResponse.User, timelineResponse);
                        dbUser = ParseFullUser(dbUser, tUser);
                    }
                }

                catch (TwitterQueryException tqe)
                {
                    return null;
                }









                /*if (newUser)
                {
                    foreach (var friend in dbUser.Friends)
                    {
                        if (friend.Activated)
                        {
                            var newNotification = new Notification()
                            {
                                IsRed = false,
                                AppUserId = friend.AppUserId,
                                ObjectType = NotificationObject.AppUser.ToString(),
                                ObjectId = dbUser.AppUserId,
                                Information = dbUser.FirstName + " " + dbUser.LastName + TranslationHelper.GetTranslation(friend.Locale, "NEW_FRIEND_NOTIFICATION"),
                                NotificationDate = DateTime.Now

                            };
                            _NotificationRepository.Insert(newNotification);
                        }
                    }
                }*/
                return dbUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private AppUser CleanDbUser(AppUser dbUser)
        {
            //To Do Check for obsolete post/friends/etc
            return dbUser;
        }


        #region ParseDataFromTwitterWithDataFromDB
        private List<Post> MapTweetWithDB(List<Post> postsFromTwitter, List<Post> PostsFromDb, IEnumerable<AppUser> existingUsers, IEnumerable<Post> existingPosts)
        {
            var Posts = new List<Post>();
            foreach (var post in postsFromTwitter)
            {
                var dbposts = PostsFromDb.Where(p => p.TwitterDetail.TwitterPostId == post.TwitterDetail.TwitterPostId);
                if (dbposts.Count() > 0)
                    Posts.Add(ParsePost(dbposts.First(), post, existingUsers, existingPosts));
                else
                    Posts.Add(post);
            }
            return Posts;

        }
        private List<Publication> MapNewsfeedWithDB(List<Publication> newsfeedFromTwitter, IEnumerable<AppUser> existingUsers, IEnumerable<Post> existingPosts)
        {
            var ParsedNewsfeed = new List<Publication>();

            

            foreach (var pair in newsfeedFromTwitter)
            {
                ParsedNewsfeed.Add(ParsePublicationRecursive(existingPosts, existingUsers, pair));
            }
            return ParsedNewsfeed;

        }

        private IEnumerable<string> GetTwitterPostIdRecursive(List<Publication> newsFeed)
        {
            var children = newsFeed.Where(p => p.Post.TwitterDetail.RetweetedPublication != null).Select(x => x.Post.TwitterDetail.RetweetedPublication).ToList();
            if (children.Any())
            {
                return newsFeed.Select(x => x.Post.TwitterDetail.TwitterPostId).Concat(GetTwitterPostIdRecursive(children));
            }
            return newsFeed.Select(x => x.Post.TwitterDetail.TwitterPostId);
        }
        private IEnumerable<string> GetTwitterUserIdRecursive(List<Publication> newsFeed)
        {
            var mentionedUserIds = new List<string>();
                newsFeed.ForEach(p => mentionedUserIds.AddRange(p.Post.TwitterDetail.MentionedUsers.Select(user => user.TwitterDetail.TwitterUserId)));

            var children = newsFeed.Where(p => p.Post.TwitterDetail.RetweetedPublication != null).Select(x => x.Post.TwitterDetail.RetweetedPublication).ToList();
            if (children.Any())
            {
                return newsFeed.Select(x => x.User.TwitterDetail.TwitterUserId).Concat(mentionedUserIds).Concat(GetTwitterPostIdRecursive(children));
            }
            return newsFeed.Select(x => x.User.TwitterDetail.TwitterUserId).Concat(mentionedUserIds);
        }

        private IEnumerable<string> GetTwitterFriendIdRecursive(List<Publication> newsFeed)
        {
            var mentionedUserIds = new List<string>();
            newsFeed.ForEach(p => mentionedUserIds.AddRange(p.Post.TwitterDetail.MentionedUsers.Select(user => user.TwitterDetail.TwitterUserId)));

            var children = newsFeed.Where(p => p.Post.TwitterDetail.RetweetedPublication != null).Select(x => x.Post.TwitterDetail.RetweetedPublication).ToList();
            if (children.Any())
            {
                return newsFeed.Select(x => x.User.TwitterDetail.TwitterUserId).Concat(mentionedUserIds).Concat(GetTwitterPostIdRecursive(children));
            }
            return newsFeed.Select(x => x.User.TwitterDetail.TwitterUserId).Concat(mentionedUserIds);
        }

        private Publication ParsePublicationRecursive(IEnumerable<Post> existingPosts, IEnumerable<AppUser> existingUsers, Publication publicationFromTwitter)
        {
            var parsedPost = publicationFromTwitter.Post;
            var parsedUser = publicationFromTwitter.User;

            var dbPost = existingPosts.Where(p => p.TwitterDetail.TwitterPostId == publicationFromTwitter.Post.TwitterDetail.TwitterPostId).FirstOrDefault();
            var dbUser = existingUsers.Where(p => p.TwitterDetail.TwitterUserId == publicationFromTwitter.User.TwitterDetail.TwitterUserId).FirstOrDefault();


            if (dbPost != null)
                parsedPost = ParsePost(dbPost, publicationFromTwitter.Post, existingUsers, existingPosts);
            if (dbUser != null)
                parsedUser = ParseUser(dbUser, publicationFromTwitter.User, existingUsers, existingPosts);
            if (publicationFromTwitter.Post.TwitterDetail.RetweetedPublication != null)
            {
                parsedPost.TwitterDetail.RetweetedPublication = ParsePublicationRecursive(existingPosts, existingUsers, publicationFromTwitter.Post.TwitterDetail.RetweetedPublication);
            }
            return new Publication(parsedPost, parsedUser);
        }



        private Post ParsePost(Post PostFromDb, Post PostFromTwitter, IEnumerable<AppUser> existingUsers, IEnumerable<Post> existingPosts)
        {
            Post parsedPost = PostFromDb.DeepClone();
            parsedPost.CreationDate = PostFromTwitter.CreationDate;
            parsedPost.TwitterDetail = PostFromDb.TwitterDetail.DeepClone();

            parsedPost.TwitterDetail.TwitterPostId = PostFromTwitter.TwitterDetail.TwitterPostId;
            parsedPost.TwitterDetail.Text = PostFromTwitter.TwitterDetail.Text;
            parsedPost.TwitterDetail.RetweetedPostId = PostFromTwitter.TwitterDetail.RetweetedPostId;
            parsedPost.TwitterDetail.CreationTime = PostFromTwitter.TwitterDetail.CreationTime;


            if (PostFromTwitter.TwitterDetail.RetweetedPublication != null)
            {
                var dbPost = existingPosts.Where(p => p.TwitterDetail.TwitterPostId == PostFromTwitter.TwitterDetail.RetweetedPublication.Post.TwitterDetail.TwitterPostId).FirstOrDefault();
                var dbUser = existingUsers.Where(p => p.TwitterDetail.TwitterUserId == PostFromTwitter.TwitterDetail.RetweetedPublication.User.TwitterDetail.TwitterUserId).FirstOrDefault();

                parsedPost.TwitterDetail.RetweetedPublication = new Publication
                    (
                    ParsePost(dbPost, PostFromTwitter.TwitterDetail.RetweetedPublication.Post, existingUsers, existingPosts),
                    ParseUser(dbUser, PostFromTwitter.TwitterDetail.RetweetedPublication.User, existingUsers, existingPosts)
                    );
            }
            foreach (var url in PostFromTwitter.TwitterDetail.Urls)
            {
                var existingUrls = parsedPost.TwitterDetail.Urls.Where(t => t.Url == url.Url).ToList();
                if (!existingUrls.Any())
                    parsedPost.TwitterDetail.Urls.Add(url);
            }
            foreach (var hashtag in PostFromTwitter.TwitterDetail.Hashtags)
            {
                var existingHashtags = parsedPost.TwitterDetail.Hashtags.Where(t => t.Text == hashtag.Text).ToList();
                if (!existingHashtags.Any())
                    parsedPost.TwitterDetail.Hashtags.Add(hashtag);
            }
            foreach (var media in PostFromTwitter.TwitterDetail.Medias)
            {
                var existingMedias = parsedPost.TwitterDetail.Medias.Where(t => t.Url == media.Url).ToList();
                if (!existingMedias.Any())
                    parsedPost.TwitterDetail.Medias.Add(media);
            }
            var tmpMentionedUsers = new List<AppUser>();
            foreach (var user in PostFromTwitter.TwitterDetail.MentionedUsers)
            {
                var existingUser = existingUsers.Where(p => p.TwitterDetail.TwitterUserId == user.TwitterDetail.TwitterUserId).FirstOrDefault();
                if (existingUser != null)
                    tmpMentionedUsers.Add(ParseUser(existingUser, user, existingUsers, existingPosts));
                else
                    tmpMentionedUsers.Add(user);
                
            }
            parsedPost.TwitterDetail.MentionedUsers.Clear();
            parsedPost.TwitterDetail.MentionedUsers= tmpMentionedUsers;
            return parsedPost;
        }

        private AppUser ParseUser(AppUser UserFromDb, AppUser UserFromTwitter, IEnumerable<AppUser> existingUsers, IEnumerable<Post> existingPosts) //Parse a User and its posts but not its friends 
        {
            var ParsedUser = UserFromDb.DeepClone();
            ParsedUser.FirstName = UserFromTwitter.FirstName;
            ParsedUser.LastName = UserFromTwitter.LastName;
            ParsedUser.EmailAddress = !string.IsNullOrWhiteSpace(UserFromTwitter.EmailAddress) ? UserFromTwitter.EmailAddress : UserFromDb.EmailAddress;
            ParsedUser.TwitterDetail.TwitterUserId = UserFromTwitter.TwitterDetail.TwitterUserId;
            ParsedUser.TwitterDetail.Description = UserFromTwitter.TwitterDetail.Description;
            ParsedUser.TwitterDetail.ScreenName = UserFromTwitter.TwitterDetail.ScreenName;
            ParsedUser.TwitterDetail.Url = UserFromTwitter.TwitterDetail.Url;
            ParsedUser.Gender = UserFromTwitter.Gender;
            ParsedUser.Birthday = UserFromTwitter.Birthday;
            ParsedUser.Locale = UserFromTwitter.Locale;

            var tempDBPosts = new List<Post>(ParsedUser.Posts.Select(x => x.DeepClone()));
            ParsedUser.Posts.Clear();
            ParsedUser.Posts = MapTweetWithDB(UserFromTwitter.Posts, tempDBPosts, existingUsers, existingPosts);

            ParsedUser.Friends.Clear();

            if (!ParsedUser.Footprint.Any())
                ParsedUser.Footprint = UserFromTwitter.Footprint;
            return ParsedUser;
        }
        private AppUser ParseFullUser(AppUser UserFromDb, AppUser UserFromTwitter)
        {
            
            var existingUsers = _UserRepository.Select(DephtLevel.UserProfile,
                new
                {
                    TwitterUserId = ImbricatedObjectFinder.GetImbricatedUsers(UserFromTwitter).Where(u=>u.IsProvidedBy(Provider.Twitter)).Select(u=>u.TwitterDetail.TwitterUserId).ToList()
                }); //on recupere l'ensemble des user dejà present dans la base pour les posts du fil et leurs enfants.
            var existingPosts = _PostRepository.Select(
                 new
                 {
                     TwitterPostId = ImbricatedObjectFinder.GetImbricatedPosts(UserFromTwitter).Where(u => u.IsProvidedBy(Provider.Twitter)).Select(u => u.TwitterDetail.TwitterPostId).ToList()
                 }); //on recupere l'ensemble des post dejà present dans la base pour les posts du fil et leurs enfants.
            
            AppUser parsedUser = ParseUser(UserFromDb, UserFromTwitter, existingUsers, existingPosts);
            
            parsedUser.Newsfeed = MapNewsfeedWithDB(UserFromTwitter.Newsfeed, existingUsers, existingPosts);

            return parsedUser;
        }
        #endregion



        #region ConvertDataFromTwitter
        private Hashtag ConvertHashtagFromTwitterToHashtag(HashTagEntity hashtag)
        {
            return new Hashtag()
            {
                Provider = Provider.Twitter.Value,
                Text = hashtag.Tag
            };
        }
        private PostMedia ConvertMediaFromTwitterToMedia(MediaEntity media)
        {
            return new PostMedia()
            {
                Provider = Provider.Twitter.Value,
                Type=media.Type,
                PostMediaTwitterId = media.ID.ToString(),
                Url= media.MediaUrl
            };
        }
        private PostUrl ConvertUrlFromTwitterToUrl(UrlEntity url)
        {
            return new PostUrl()
            {
                Provider = Provider.Twitter.Value,
                Url = url.ExpandedUrl
            };
        }
        private Post ConvertPostFromTwitterToAppPost(Status twitterPost)
        {
            var post = new Post()
            {
                CategoryId = 1,
                Activated = true,
                CreationDate = Convert.ToDateTime(twitterPost.CreatedAt.ToString())
            };
            post.TwitterDetail = new TwitterPostDetail()
            {
                TwitterPostId = twitterPost.StatusID.ToString(),
                Text = twitterPost.Text,
                CreationTime = Convert.ToDateTime(twitterPost.CreatedAt.ToString()),

            };
            

            if (twitterPost.RetweetedStatus != null && twitterPost.RetweetedStatus.User != null)
            {
                post.TwitterDetail.RetweetedPublication = new Publication(ConvertPostFromTwitterToAppPost(twitterPost.RetweetedStatus), ConvertUserFromTwitterToAppUser(twitterPost.RetweetedStatus.User));
            }
            if (twitterPost.Entities != null)
            {
                if (twitterPost.Entities.HashTagEntities != null)
                {
                    foreach (var hashtag in twitterPost.Entities.HashTagEntities)
                        post.TwitterDetail.Hashtags.Add(ConvertHashtagFromTwitterToHashtag(hashtag));
                }
                if (twitterPost.Entities.UrlEntities != null)
                {
                    foreach (var url in twitterPost.Entities.UrlEntities)
                    {
                        post.TwitterDetail.Urls.Add(ConvertUrlFromTwitterToUrl(url));
                        if(post.TwitterDetail.Text.Contains(url.Url))
                        {
                            post.TwitterDetail.Text = post.TwitterDetail.Text.Replace(url.Url, url.ExpandedUrl);
                        }
                    }
                }
                if (twitterPost.Entities.UserMentionEntities != null)
                {
                    foreach (var user in twitterPost.Entities.UserMentionEntities)
                        post.TwitterDetail.MentionedUsers.Add(ConvertUserFromTwitterMentionToAppUser(user));
                }
                if (twitterPost.Entities.MediaEntities != null)
                {
                    foreach (var media in twitterPost.Entities.MediaEntities)
                    {
                        post.TwitterDetail.Medias.Add(ConvertMediaFromTwitterToMedia(media));
                        if (post.TwitterDetail.Text.Contains(media.Url))
                        {
                            post.TwitterDetail.Text = post.TwitterDetail.Text.Replace(media.Url,"");
                        }
                    }
                }
            }
            return post;

        }
        private AppUser ConvertUserFromTwitterMentionToAppUser(UserMentionEntity twitterUser)
        {
            AppUser user = new AppUser();
            user.TwitterDetail = new TwitterUserDetail();
            user.FirstName = twitterUser.Name;
            user.Activated = false;
            user.SigningUpDate = DateTime.Now;
            user.LastLogInDate = DateTime.Now;
            user.TwitterDetail.TwitterUserId = twitterUser.Id.ToString();
            user.TwitterDetail.ScreenName = twitterUser.ScreenName;
            user.Footprint = FootprintHelper.GetNewFootprint(_Categories, _CategoryRepository);
            return user;


        }

        private AppUser ConvertUserFromTwitterToAppUser(User twitterUser)
        {
            AppUser user = new AppUser();
            user.TwitterDetail = new TwitterUserDetail();
            user.FirstName = twitterUser.Name;
            user.Activated = false;
            user.SigningUpDate = DateTime.Now;
            user.LastLogInDate = DateTime.Now;
            user.TwitterDetail.TwitterUserId = twitterUser.UserIDResponse;
            user.TwitterDetail.ScreenName = twitterUser.ScreenNameResponse;
            if(!string.IsNullOrWhiteSpace(twitterUser.Description))
                user.TwitterDetail.Description =  twitterUser.Description;
            user.Locale = twitterUser.LangResponse;
            user.Footprint = FootprintHelper.GetNewFootprint(_Categories, _CategoryRepository);
            user.TwitterDetail.Url = twitterUser.Url;

            return user;


        }

        private AppUser ConvertFullUserFromTwitterToAppUser(User twitterUser, List<Status> timeline)
        {
            AppUser user = ConvertUserFromTwitterToAppUser(twitterUser);
            user.Activated = true;

            if (timeline != null)
            {
                foreach (var tweet in timeline)
                {
                    user.Newsfeed.Add
                        (
                            new Publication
                            (
                                ConvertPostFromTwitterToAppPost(tweet),
                                ConvertUserFromTwitterToAppUser(tweet.User)
                            )
                        );
                }
            }
            return user;

        }


        #endregion

    }

}
