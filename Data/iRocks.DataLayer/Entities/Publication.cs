using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRocks.DataLayer
{
    public class Publication
    {
        public Publication(Post aPost, AppUser aUser)
        {
            this.Post = aPost;
            this.User = aUser;
            SetAnonymousStory();
        }
        public Publication(Post aPost, AppUser aUser, string locale)
        {
            this.Post = aPost;
            this.User = aUser;
            filterStories(locale);
            SetAnonymousStory();
        }
        public Post Post {get; set;}
        public AppUser User { get; set; }

        public void filterStories(string locale)
        {
            filterStoriesRecursive(this.Post.FacebookDetail, locale);
        }
        private void filterStoriesRecursive(FacebookPostDetail facebookDetail,  string locale)
        {
            if (Post.IsProvidedBy(Provider.Facebook))
            {
                if (!string.IsNullOrWhiteSpace(locale))
                {
                    var goodTranslation = facebookDetail.Stories.Where(s => s.Locale == locale).FirstOrDefault();
                    if (goodTranslation != null)
                    {
                        var temp = goodTranslation.DeepClone();
                        facebookDetail.Stories.Clear();
                        facebookDetail.Stories.Add(temp);

                    }
                    if (facebookDetail.ChildPublication != null)
                    {
                        filterStoriesRecursive(facebookDetail.ChildPublication.Post.FacebookDetail, locale);
                    }
                }
            }
        }
        private void SetAnonymousStory()
        {
            if (Post.IsProvidedBy(Provider.Facebook))
            {
                if (Post.FacebookDetail.Stories.Count > 0)
                {
                    var name = User.FirstName;
                    if (!String.IsNullOrWhiteSpace(User.LastName))
                        name = User.LastName;
                    var index = Post.FacebookDetail.Stories.First().Story.IndexOf(name);
                    Post.FacebookDetail.AnonymousStory = Post.FacebookDetail.Stories.First().Story.Substring(index + name.Length);
                    if (Post.FacebookDetail.Stories.First().Locale.ToUpper().Contains("fr".ToUpper()))
                        Post.FacebookDetail.AnonymousStory = "Un(e) ami(e) " + Post.FacebookDetail.AnonymousStory;
                    else
                        Post.FacebookDetail.AnonymousStory = "A friend " + Post.FacebookDetail.AnonymousStory;
                }
            }
        }

        public Publication DeepClone()
        {
            return new Publication(this.Post.DeepClone(), this.User.DeepClone());
        }
    }
   
}