using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class Post : DapperManagedObject<Post>, IGetId, IDeeplyCloneable<Post>
    {

        public Post()
        {
            this.CreationDate = DateTime.Now;
            this.UpVotes = new List<Vote>();
            this.DownVotes = new List<Vote>();
            this.PostCategory = new Category();
            this.FacebookDetail = new FacebookPostDetail();
            this.TwitterDetail = new TwitterPostDetail();
        }
        
        public int PostId { get; set; }
        
        public int AppUserId { get; set; }
        
        public int CategoryId { get; set; }


        
        public bool Activated { get; set; }
        
        public DateTime CreationDate { get; set; }
        
        public Category PostCategory { get; set; }
        
        public List<Vote> UpVotes { get; set; }
        
        public List<Vote> DownVotes { get; set; }
        
        public FacebookPostDetail FacebookDetail { get; set; }
        public TwitterPostDetail TwitterDetail { get; set; }

        public int Score
        {
            get
            {
                return UpVotes.Count - DownVotes.Count;
            }
        }

        
        public bool IsNew
        {
            get
            {
                return this.PostId == default(int);
            }
        }
        
        [DapperIgnore]
        public bool IsDeleted { get; set; }

        
        public bool IsProvidedBy(Provider provider)
        { 
            switch(provider.Value)
            {
                case "Facebook":
                    if (this.FacebookDetail != null)
                        if (!string.IsNullOrEmpty(this.FacebookDetail.FacebookPostId))
                            return true;
                    return false;
                case "Twitter":
                    if (this.TwitterDetail != null)
                        if (!string.IsNullOrEmpty(this.TwitterDetail.TwitterPostId))
                            return true;
                    return false;
                default:
                    return false;
            }
        }

        public Post DeepClone()
        {
            var res = new Post()
            {
                PostId = this.PostId,
                AppUserId = this.AppUserId,
                CategoryId = this.CategoryId,
                Activated = this.Activated,
                CreationDate = this.CreationDate,
                PostCategory = this.PostCategory.DeepClone(),
                UpVotes = new List<Vote>(this.UpVotes.Select(x => x.DeepClone())),
                DownVotes = new List<Vote>(this.DownVotes.Select(x => x.DeepClone())),
                FacebookDetail = IsProvidedBy(Provider.Facebook)? this.FacebookDetail.DeepClone():null,
                TwitterDetail = IsProvidedBy(Provider.Twitter) ? this.TwitterDetail.DeepClone() : null,
                Snapshot = this.Snapshot
            };
            return res;
        }


        public int GetId()
        {
            return PostId;
        }
    }

}
