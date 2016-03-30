using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class FacebookPostDetail : DapperManagedObject<FacebookPostDetail>, IGetId, IDeeplyCloneable<FacebookPostDetail>
    {
        public FacebookPostDetail()
        {
            this.UpdateTime = DateTime.Now;
            this.Target = new List<PostRelationship>();
            this.Stories = new List<StoryTranslation>();
        }

        public int FacebookPostDetailId { get; set; }

        public int PostId { get; set; }

        public string FacebookPostId { get; set; }

        public string Link { get; set; }
        public string Caption { get; set; }

        public string Message { get; set; }

        public string LinkName { get; set; }

        private string attachedObjectId;

        public string AttachedObjectId
        {
            get { return attachedObjectId; }
            set
            {
                attachedObjectId = value;
                AttachedObjectUrl = "https://graph.facebook.com/" + value + "/picture";
            }
        }
        [DapperIgnore]
        public string AttachedObjectUrl { get; set; }

        public string Picture { get; set; }

        public string Privacy { get; set; }

        public string VideoSource { get; set; }

        public string StatusType { get; set; }

        public List<StoryTranslation> Stories { get; set; }
        [DapperIgnore]
        public string  AnonymousStory { get; set; }

        public string GeneralStatusType { get; set; }

        public DateTime UpdateTime { get; set; }

        public int? ChildPostId { get; set; }
        [DapperIgnore]
        public Publication ChildPublication { get; set; }

        [DapperIgnore]
        public IEnumerable<PostRelationship> Target { get; set; }

        public bool IsNew
        {
            get
            {
                return this.FacebookPostDetailId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }
        public void setAccessTokenToPictureUrl(string accessToken) {
            setAccessTokenToPictureUrlRecursive(this, accessToken);
        }
        private void setAccessTokenToPictureUrlRecursive(FacebookPostDetail facebookDetail, string accessToken)
        {
            facebookDetail.AttachedObjectUrl = "https://graph.facebook.com/" + facebookDetail.AttachedObjectId + "/picture?access_token=" + accessToken;
            if (facebookDetail.ChildPublication != null)
            {
                if (facebookDetail.ChildPublication.Post != null)
                {
                    if (facebookDetail.ChildPublication.Post.FacebookDetail != null)
                    {
                        setAccessTokenToPictureUrlRecursive(facebookDetail.ChildPublication.Post.FacebookDetail, accessToken);
                    }
                }
            }
         }
        public FacebookPostDetail DeepClone()
        {
            var res = new FacebookPostDetail()
            {
                FacebookPostDetailId = this.FacebookPostDetailId,
                PostId = this.PostId,
                FacebookPostId = this.FacebookPostId,
                Link = this.Link,
                Caption = this.Caption,
                Message = this.Message,
                LinkName = this.LinkName,
                AttachedObjectId = this.AttachedObjectId,
                Picture = this.Picture,
                Privacy = this.Privacy,
                VideoSource = this.VideoSource,
                StatusType = this.StatusType,
                GeneralStatusType = this.GeneralStatusType,
                UpdateTime = this.UpdateTime,
                Stories= new List<StoryTranslation>(this.Stories.Select(x=>x.DeepClone())),
                Target = new List<PostRelationship>(this.Target.Select(x => x.DeepClone())),
                ChildPostId = this.ChildPostId,
                ChildPublication = this.ChildPublication != null ? this.ChildPublication.DeepClone() : null,
                Snapshot = this.Snapshot
            };
            return res;
        }
        public int GetId()
        {
            return FacebookPostDetailId;
        }
    }
}
