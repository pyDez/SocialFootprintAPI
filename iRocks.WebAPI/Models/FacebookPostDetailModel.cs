using iRocks.DataLayer;
using System;
using System.Collections.Generic;

namespace iRocks.WebAPI.Models
{
    public class FacebookPostDetailModel
    {
        public int FacebookPostDetailId { get; set; }

        public int PostId { get; set; }

        public string FacebookPostId { get; set; }

        public string Link { get; set; }
        public string Caption { get; set; }

        public string Message { get; set; }

        public string LinkName { get; set; }

        public string AttachedObjectId { get; set; }

        public string AttachedObjectUrl { get; set; }

        public string Picture { get; set; }

        public string Privacy { get; set; }

        public string VideoSource { get; set; }

        public string StatusType { get; set; }

        public List<StoryTranslation> Stories { get; set; }

        public string AnonymousStory { get; set; }

        public string GeneralStatusType { get; set; }

        public DateTime UpdateTime { get; set; }

        public int? ChildPostId { get; set; }
        public PublicationModel ChildPublication { get; set; }

        public IEnumerable<PostRelationship> Target { get; set; }

    }
}