using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace iRocks.DataLayer
{
    public class TwitterPostDetailDapperRepository : DapperRepositoryBase, ITwitterPostDetailRepository
    {

        public TwitterPostDetailDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<TwitterPostDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {

            IPostRepository PostRepository = new PostDapperRepository();
            IUserRepository UserRepository = new UserDapperRepository();

            var mentionedUsermapping = new List<Tuple<int, int>>();

            var lookup = new Dictionary<int, TwitterPostDetail>();
            var TwitterPostDetails = base.Select<TwitterPostDetail, Hashtag, PostMentionedUser, PostUrl, PostMedia, TwitterPostDetail>(
                           @"SELECT * FROM TwitterPostDetail LEFT OUTER JOIN Hashtag  ON TwitterPostDetail.PostId = Hashtag.PostId AND Hashtag.Provider = '" + Provider.Twitter.Value + "' " +
                                                @"LEFT OUTER JOIN PostMentionedUser  ON TwitterPostDetail.PostId = PostMentionedUser.PostId AND PostMentionedUser.Provider = '" + Provider.Twitter.Value + "' " +
                                                @"LEFT OUTER JOIN PostUrl  ON TwitterPostDetail.PostId = PostUrl.PostId AND PostUrl.Provider = '" + Provider.Twitter.Value + "' "+
                                                @"LEFT OUTER JOIN PostMedia  ON TwitterPostDetail.PostId = PostMedia.PostId AND PostMedia.Provider = '" + Provider.Twitter.Value + "' " ,

                         (twitterDetail, hashtag, postMentionedUser, postUrl, postMedia) =>
                         {
                             TwitterPostDetail aTwitterDetail;
                             if (!lookup.TryGetValue(twitterDetail.TwitterPostDetailId, out aTwitterDetail))
                             {
                                 lookup.Add(twitterDetail.TwitterPostDetailId, aTwitterDetail = twitterDetail);
                             }
                             if (hashtag != null && !aTwitterDetail.Hashtags.Where(h => h.HashtagId == hashtag.HashtagId).Any())
                             {
                                 hashtag.SetSnapshot(hashtag);
                                 aTwitterDetail.Hashtags.Add(hashtag);
                             }
                             if (postMentionedUser != null && !mentionedUsermapping.Where(pair => pair.Item1 == aTwitterDetail.TwitterPostDetailId && pair.Item2 == postMentionedUser.AppUserId).Any())
                             {
                                 mentionedUsermapping.Add(new Tuple<int, int>(aTwitterDetail.TwitterPostDetailId, postMentionedUser.AppUserId));
                             }
                             if (postUrl != null && !aTwitterDetail.Urls.Where(u => u.PostUrlId == postUrl.PostUrlId).Any())
                             {
                                 postUrl.SetSnapshot(postUrl);
                                 aTwitterDetail.Urls.Add(postUrl);
                             }
                             if (postMedia != null && !aTwitterDetail.Medias.Where(m => m.PostMediaId == postMedia.PostMediaId).Any())
                             {
                                 postMedia.SetSnapshot(postMedia);
                                 aTwitterDetail.Medias.Add(postMedia);
                             }
                             return aTwitterDetail;
                         },
                         criteria,
                         ConditionalKeyWord,
                         splitOn: "HashtagId, PostMentionedUserId, PostUrlId, PostMediaId"
                         );


            var retweetedIds = TwitterPostDetails.Select(p => p.RetweetedPostId).Where(id => id.HasValue).ToList();
            IEnumerable<Post> retweetedPosts = new List<Post>();
            if (retweetedIds.Count() > 0)
            {
                retweetedPosts = PostRepository.Select(new { PostId = retweetedIds });
               // var retweetedPostsUsers = UserRepository.Select(DephtLevel.UserFootprint, new { AppUserId = retweetedPosts.Select(p => p.AppUserId).ToList() });
            }

            var implicatedUsers = UserRepository.Select(DephtLevel.UserFootprint, new { AppUserId = mentionedUsermapping.Select(d => d.Item2).Concat(retweetedPosts.Select(p => p.AppUserId)).ToList() });

            var Publications = new List<Publication>();
            retweetedPosts.ToList().ForEach(c => Publications.Add(new Publication(c, implicatedUsers.Where(u => u.AppUserId == c.AppUserId).FirstOrDefault())));
            TwitterPostDetails.ToList().ForEach(tpd => tpd.RetweetedPublication = Publications.Where(p => p.Post.PostId == tpd.RetweetedPostId).FirstOrDefault());

            foreach (var map in mentionedUsermapping)
            {
                foreach(var detail in TwitterPostDetails)                    
                {
                    if (detail.TwitterPostDetailId == map.Item1)
                    {
                        foreach (var user in implicatedUsers)
                        {
                            if(user.AppUserId == map.Item2)
                                detail.MentionedUsers.Add(user);
                        }
                    }
                }
               
            }

            
            return TwitterPostDetails;

        }

        public void Insert(TwitterPostDetail obj)
        {

            SaveChild(obj);
            base.Insert<TwitterPostDetail>(obj);
            SaveDependencies(obj);
        }

        

        public void Update(TwitterPostDetail obj)
        {
            SaveChild(obj);
            base.Update<TwitterPostDetail>(obj);
            SaveDependencies(obj);
        }

       

        public void Delete(TwitterPostDetail obj)
        {
            base.Delete<TwitterPostDetail>(obj);
        }
        private void SaveChild(TwitterPostDetail obj)
        {
            IUserRepository UserRepository = new UserDapperRepository();
            IPostRepository PostRepository = new PostDapperRepository();

            if (obj.RetweetedPublication != null)
            {
                UserRepository.Save(DephtLevel.UserFootprint, obj.RetweetedPublication.User);
                obj.RetweetedPublication.Post.AppUserId = obj.RetweetedPublication.User.AppUserId;
                PostRepository.Update(obj.RetweetedPublication.Post);
                obj.RetweetedPostId = obj.RetweetedPublication.Post.PostId;
            }
        }
        private void SaveDependencies(TwitterPostDetail obj)
        {
            IHashtagRepository hashtagRepository = new HashtagDapperRepository();
            IPostMediaRepository postMediaRepository = new PostMediaDapperRepository();
            IPostMentionedUserRepository postMentionedUserRepository = new PostMentionedUserDapperRepository();
            IPostUrlRepository postUrlRepository = new PostUrlDapperRepository();
            IUserRepository UserRepository = new UserDapperRepository();

            foreach (var hashtag in obj.Hashtags)
            {
                if (hashtag.IsNew)
                {
                    hashtag.PostId = obj.PostId;
                    hashtagRepository.Insert(hashtag);
                }
                else
                    hashtagRepository.Update(hashtag);
            }
            foreach (var media in obj.Medias)
            {
                if (media.IsNew)
                {
                    media.PostId = obj.PostId;
                    postMediaRepository.Insert(media);
                }
                else
                    postMediaRepository.Update(media);
            }
            foreach (var url in obj.Urls)
            {
                if (url.IsNew)
                {
                    url.PostId = obj.PostId;
                    postUrlRepository.Insert(url);
                }
                else
                    postUrlRepository.Update(url);
            }
            if(obj.MentionedUsers.Any())
            {
                UserRepository.Save(DephtLevel.UserFootprint, obj.MentionedUsers);
            }

            var mentionedUsers = postMentionedUserRepository.Select(new { PostId = obj.PostId, Provider = Provider.Twitter.Value }, SQLKeyWord.And);

            foreach (var user in obj.MentionedUsers)
            {
                if (!mentionedUsers.Where(mu=>mu.AppUserId==user.AppUserId).Any())
                {
                    postMentionedUserRepository.Insert(new PostMentionedUser { AppUserId = user.AppUserId, PostId = obj.PostId, Provider= Provider.Twitter.Value});
                }
                   
            }
        }

    }
}
