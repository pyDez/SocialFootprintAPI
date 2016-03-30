using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

namespace iRocks.DataLayer
{
    public class PostDapperRepository : DapperRepositoryBase, IPostRepository
    {

        public PostDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        /*public IEnumerable<Post> GetItems(CommandType commandType, string sql, dynamic parameters = null)
        {
            IEnumerable<Post> Posts = base.GetItems<Post>(commandType, sql, (DynamicParameters)parameters);
            return CategoriesAndVotesAffectation(Posts);
        }*/

        //1s \o/
        public IEnumerable<Post> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {

            IFacebookPostDetailRepository FacebookDetailRepositiory = new FacebookPostDetailDapperRepository();
            ITwitterPostDetailRepository TwitterDetailRepositiory = new TwitterPostDetailDapperRepository();
            ICategoryRepository CategoryRepositiory = new CategoryDapperRepository();

            var lookup = new Dictionary<int, Post>();
            var FacebookPostDetailIds = new List<int>();
            var TwitterPostDetailIds = new List<int>();
            var CategoryIds = new List<int>();
            var posts = base.Select<Post, Vote, FacebookPostDetail, TwitterPostDetail, int?, Post>(
                            @"SELECT Post.*, Vote.*, FacebookPostDetail.*, TwitterPostDetail.*, Category.CategoryId FROM Post LEFT OUTER JOIN Vote  ON Vote.UpPostId = Post.PostId OR Vote.DownPostId = Post.PostId
                                                 LEFT OUTER JOIN FacebookPostDetail  ON FacebookPostDetail.PostId = Post.PostId 
                                                 LEFT OUTER JOIN TwitterPostDetail  ON TwitterPostDetail.PostId = Post.PostId 
                                                 LEFT OUTER JOIN Category  ON Category.CategoryId = Post.CategoryId ",

                          (post, vote, facebookPostDetail, twitterPostDetail, categoryId) =>
                          {
                              Post aPost;
                              if (!lookup.TryGetValue(post.PostId, out aPost))
                              {
                                  lookup.Add(post.PostId, aPost = post);
                                  if (facebookPostDetail != null)
                                      FacebookPostDetailIds.Add(facebookPostDetail.FacebookPostDetailId);

                                  if (twitterPostDetail != null)
                                      TwitterPostDetailIds.Add(twitterPostDetail.TwitterPostDetailId);

                                  if (categoryId.HasValue)
                                      CategoryIds.Add(categoryId.Value);
                                  //aPost.PostCategory = category;
                              }
                              if (vote != null)
                              {
                                  vote.SetSnapshot(vote);
                                  if (vote.UpPostId == aPost.PostId && !aPost.UpVotes.Where(v => v.VoteId == vote.VoteId).Any())
                                      aPost.UpVotes.Add(vote);
                                  if (vote.DownPostId == aPost.PostId && !aPost.DownVotes.Where(v => v.VoteId == vote.VoteId).Any())
                                      aPost.DownVotes.Add(vote);
                              }
                              //aPost.SetSnapshot(aPost);
                              return aPost;
                          },
                          criteria,
                          ConditionalKeyWord,
                          splitOn: "VoteId, FacebookPostDetailId, TwitterPostDetailId, CategoryId"
                          );
            if (FacebookPostDetailIds.Any())
            {
                var FacebookDetails = FacebookDetailRepositiory.Select(new { FacebookPostDetailId = FacebookPostDetailIds }).ToList();
                lookup.Values.ToList().ForEach(p => p.FacebookDetail = FacebookDetails.Where(d => d.PostId == p.PostId).FirstOrDefault());
            }
            if (TwitterPostDetailIds.Any())
            {
                var TwitterDetails = TwitterDetailRepositiory.Select(new { TwitterPostDetailId = TwitterPostDetailIds }).ToList();
                lookup.Values.ToList().ForEach(p => p.TwitterDetail = TwitterDetails.Where(d => d.PostId == p.PostId).FirstOrDefault());
            }
            var Categories = CategoryRepositiory.Select(new { CategoryId = CategoryIds }).ToList();
            lookup.Values.ToList().ForEach(p => p.PostCategory = Categories.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault());


            return lookup.Values;

        }


        public void Insert(Post obj)
        {
            using (var transaction = new TransactionScope())
            {
                IFacebookPostDetailRepository FacebookPostDetailRepository = new FacebookPostDetailDapperRepository();
                ITwitterPostDetailRepository TwitterPostDetailRepository = new TwitterPostDetailDapperRepository();

                base.Insert<Post>(obj);
                if (obj.IsProvidedBy(Provider.Facebook))
                {
                    obj.FacebookDetail.PostId = obj.PostId;
                    FacebookPostDetailRepository.Insert(obj.FacebookDetail);

                }
                if (obj.IsProvidedBy(Provider.Twitter))
                {
                    obj.TwitterDetail.PostId = obj.PostId;
                    TwitterPostDetailRepository.Insert(obj.TwitterDetail);

                }
                transaction.Complete();
            }
        }


        public void Update(Post obj)
        {
            using (var transaction = new TransactionScope())
            {
                IFacebookPostDetailRepository FacebookPostDetailRepository = new FacebookPostDetailDapperRepository();
                ITwitterPostDetailRepository TwitterPostDetailRepository = new TwitterPostDetailDapperRepository();

                if (obj.IsNew)
                    base.Insert<Post>(obj);
                else
                    base.Update<Post>(obj);

                if (obj.IsProvidedBy(Provider.Facebook))
                {
                    obj.FacebookDetail.PostId = obj.PostId;
                    if (obj.FacebookDetail.IsNew)
                        FacebookPostDetailRepository.Insert(obj.FacebookDetail);
                    else if (obj.FacebookDetail.IsDeleted)
                        FacebookPostDetailRepository.Delete(obj.FacebookDetail);
                    else
                        FacebookPostDetailRepository.Update(obj.FacebookDetail);
                }
                if (obj.IsProvidedBy(Provider.Twitter))
                {
                    obj.TwitterDetail.PostId = obj.PostId;
                    if (obj.TwitterDetail.IsNew)
                        TwitterPostDetailRepository.Insert(obj.TwitterDetail);
                    else if (obj.TwitterDetail.IsDeleted)
                        TwitterPostDetailRepository.Delete(obj.TwitterDetail);
                    else
                        TwitterPostDetailRepository.Update(obj.TwitterDetail);
                }
                transaction.Complete();
            }
        }

        public void Delete(Post obj)
        {
            using (var transaction = new TransactionScope())
            {
                IVoteRepository VoteRepository = new VoteDapperRepository();
                IFacebookPostDetailRepository FacebookPostDetailRepository = new FacebookPostDetailDapperRepository();
                ITwitterPostDetailRepository TwitterPostDetailRepository = new TwitterPostDetailDapperRepository();

                if (obj.FacebookDetail != null)
                    FacebookPostDetailRepository.Delete(obj.FacebookDetail);

                if (obj.TwitterDetail != null)
                    TwitterPostDetailRepository.Delete(obj.TwitterDetail);

                var Parameters = new DynamicParameters();
                Parameters.Add("@PostId", obj.PostId);
                Execute(CommandType.Text, "DELETE FROM Vote WHERE UpPostId = @PostId OR DownPostId = @PostId ", Parameters);
                base.Delete<Post>(obj);
                transaction.Complete();
            }
        }

    }
}
