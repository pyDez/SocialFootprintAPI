using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class TwitterPostDetail : DapperManagedObject<TwitterPostDetail>, IGetId, IDeeplyCloneable<TwitterPostDetail>
    {
        public TwitterPostDetail()
        {
            this.CreationTime = DateTime.Now;
            this.Urls = new List<PostUrl>();
            this.Hashtags = new List<Hashtag>();
            this.Medias = new List<PostMedia>();
            this.MentionedUsers = new List<AppUser>();
        }

        public int TwitterPostDetailId { get; set; }

        public int PostId { get; set; }

        public string TwitterPostId { get; set; }

        public List<PostUrl> Urls { get; set; }
        public List<Hashtag> Hashtags { get; set; }
        public List<PostMedia> Medias { get; set; }
        public string Text { get; set; }

        public List<AppUser> MentionedUsers { get; set; }
        
        public DateTime CreationTime { get; set; }

        public int? RetweetedPostId { get; set; }
        [DapperIgnore]
        public Publication RetweetedPublication { get; set; }
        public bool IsNew
        {
            get
            {
                return this.TwitterPostDetailId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }
        public TwitterPostDetail DeepClone()
        {
            var res = new TwitterPostDetail()
            {
                TwitterPostDetailId = this.TwitterPostDetailId,
                PostId = this.PostId,
                TwitterPostId = this.TwitterPostId,
                Urls = new List<PostUrl>(this.Urls.Select(x => x.DeepClone())),
                Hashtags = new List<Hashtag>(this.Hashtags.Select(x => x.DeepClone())),
                Medias = new List<PostMedia>(this.Medias.Select(x => x.DeepClone())),
                Text = this.Text,
                CreationTime = this.CreationTime,
                RetweetedPostId = this.RetweetedPostId,
                MentionedUsers = new List<AppUser>(this.MentionedUsers.Select(x => x.DeepClone())),
                RetweetedPublication = this.RetweetedPublication != null ? this.RetweetedPublication.DeepClone() : null,
                Snapshot = this.Snapshot
            };
            return res;
        }
        public int GetId()
        {
            return TwitterPostDetailId;
        }
    }
}
