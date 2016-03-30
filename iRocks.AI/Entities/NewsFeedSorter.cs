using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRocks.DataLayer;

namespace iRocks.AI.Entities
{
    public class NewsFeedSorter: INewsFeedSorter
    {

        public IEnumerable<Duel> GetNewsFeed(List<Publication> posts, List<Vote> votes, int take)
        {
            var usefullPosts = posts.OrderByDescending(p =>p.Post.IsProvidedBy(Provider.Facebook)? p.Post.FacebookDetail.UpdateTime: p.Post.IsProvidedBy(Provider.Twitter) ? p.Post.TwitterDetail.CreationTime:p.Post.CreationDate).ToList();
            List<Duel> res = new List<Duel>();
            while (usefullPosts.Count != 0 && res.Count < take)
            {
                var post = usefullPosts.First();//.DeepClone();
                usefullPosts.RemoveAt(0);
                var associatedPost = GetMatchedPost(usefullPosts, post);//.DeepClone();
                if (associatedPost != null)
                {
                    usefullPosts.RemoveAll((p => p.Post.PostId == associatedPost.Post.PostId));
                    var result = votes.Where(v => (v.DownPostId == post.Post.PostId && v.UpPostId == associatedPost.Post.PostId) || (v.UpPostId == post.Post.PostId && v.DownPostId == associatedPost.Post.PostId)).FirstOrDefault();
                    res.Add(new Duel(post, associatedPost, result));
                }
            }
            //res.OrderBy(p => p.FirstPublication.Post.FacebookDetail.UpdateTime);
            return res;
        }

        public Publication GetMatchedPost(List<Publication> posts, Publication postToMatch)
        {
            var potentialPosts = posts.Where(p => p.Post.AppUserId != postToMatch.Post.AppUserId).Where(p => p.Post.CategoryId == postToMatch.Post.CategoryId);
            //if(potentialPosts.Count() == 0)
            //    potentialPosts = posts.Where(p => p.Item2.AppUserId != postToMatch.Item2.AppUserId).Where(p => p.Item2.CategoryId == PostCategory.Other);
            if (potentialPosts.Count() == 0)
                return null;
            Dictionary<Publication, double> postDistance = new Dictionary<Publication, double>();
            foreach (Publication p in potentialPosts)
                postDistance.Add(p, Math.Pow((postToMatch.Post.CreationDate - p.Post.CreationDate).TotalDays, 2) + Math.Pow(postToMatch.Post.Score - p.Post.Score, 2));

            return postDistance.OrderBy(p => p.Value).First().Key;
        }
       
    }
}
