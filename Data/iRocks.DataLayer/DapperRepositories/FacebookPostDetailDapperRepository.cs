using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace iRocks.DataLayer
{
    public class FacebookPostDetailDapperRepository : DapperRepositoryBase, IFacebookPostDetailRepository
    {

        public FacebookPostDetailDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<FacebookPostDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            //return base.Select<FacebookPostDetail>(criteria, ConditionalKeyWord);
            IPostRepository PostRepository = new PostDapperRepository();
            IUserRepository UserRepository = new UserDapperRepository();
            //var FacebookPostDetails = base.Select<FacebookPostDetail>(criteria, ConditionalKeyWord);
            var lookup = new Dictionary<int, FacebookPostDetail>();
            var FacebookPostDetails = base.Select<FacebookPostDetail, StoryTranslation, FacebookPostDetail>(
                           @"SELECT * FROM FacebookPostDetail LEFT OUTER JOIN StoryTranslation  ON FacebookPostDetail.FacebookPostDetailId = StoryTranslation.FacebookPostDetailId ",

                         (facebookDetail, storyTranslation) =>
                         {
                             FacebookPostDetail aFacebookDetail;
                             if (!lookup.TryGetValue(facebookDetail.FacebookPostDetailId, out aFacebookDetail))
                             {
                                 lookup.Add(facebookDetail.FacebookPostDetailId, aFacebookDetail = facebookDetail);
                             }
                             if (storyTranslation != null)
                             {
                                 storyTranslation.SetSnapshot(storyTranslation);
                                 aFacebookDetail.Stories.Add(storyTranslation);
                             }
                             //aPost.SetSnapshot(aPost);
                             return aFacebookDetail;
                         },
                         criteria,
                         ConditionalKeyWord,
                         splitOn: "StoryTranslationId"
                         );


            var childIds = FacebookPostDetails.Select(p => p.ChildPostId).Where(id => id.HasValue).ToList();
            if (childIds.Count() > 0)
            {
                var childPosts = PostRepository.Select(new { PostId = childIds });
                var childPostsUsers = UserRepository.Select(DephtLevel.UserFootprint, new { AppUserId = childPosts.Select(p => p.AppUserId).ToList() });

                var Publications = new List<Publication>();
                childPosts.ToList().ForEach(c => Publications.Add(new Publication(c, childPostsUsers.Where(u => u.AppUserId == c.AppUserId).FirstOrDefault())));
                FacebookPostDetails.ToList().ForEach(fpd => fpd.ChildPublication = Publications.Where(p => p.Post.PostId == fpd.ChildPostId).FirstOrDefault());
            }
            return FacebookPostDetails;

        }

        public void Insert(FacebookPostDetail obj)
        {

            SaveChild(obj);
            base.Insert<FacebookPostDetail>(obj);
            SaveStories(obj);
        }

        

        public void Update(FacebookPostDetail obj)
        {
            SaveChild(obj);
            base.Update<FacebookPostDetail>(obj);
            SaveStories(obj);
        }

        public void Delete(FacebookPostDetail obj)
        {
            base.Delete<FacebookPostDetail>(obj);
        }
        private void SaveChild(FacebookPostDetail obj)
        {
            IUserRepository UserRepository = new UserDapperRepository();
            IPostRepository PostRepository = new PostDapperRepository();
            if (obj.ChildPublication != null)
            {
                UserRepository.Save(DephtLevel.UserFootprint, obj.ChildPublication.User);
                obj.ChildPublication.Post.AppUserId = obj.ChildPublication.User.AppUserId;
                PostRepository.Update(obj.ChildPublication.Post);
                obj.ChildPostId = obj.ChildPublication.Post.PostId;
            }
        }
        private void SaveStories(FacebookPostDetail obj)
        {
            IStoryTranslationRepository storyRepository = new StoryTranslationDapperRepository();
            foreach (var story in obj.Stories)
            {
                if (story.IsNew)
                {
                    story.FacebookPostDetailId = obj.FacebookPostDetailId;
                    storyRepository.Insert(story);
                }
                //else if (vote.IsDeleted)
                //    VoteRepository.Delete(vote);

                else
                    storyRepository.Update(story);
            }
        }
    }
}
