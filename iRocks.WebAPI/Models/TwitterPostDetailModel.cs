using iRocks.DataLayer;
using System;
using System.Collections.Generic;

namespace iRocks.WebAPI.Models
{
    public class TwitterPostDetailModel
    {
        public TwitterPostDetailModel()
        {
            this.Urls = new List<string>();
            this.Hashtags = new List<string>();
            this.Medias = new List<string>();
            this.MentionedUsers = new Dictionary<string, int>();
        }
        public int TwitterPostDetailId { get; set; }

        public int PostId { get; set; }

        public string TwitterPostId { get; set; }

        public List<string> Urls { get; set; }
        public List<string> Hashtags { get; set; }
        public List<string> Medias { get; set; }
        public string Text { get; set; }

        public Dictionary<string,int> MentionedUsers { get; set; }

        public DateTime CreationTime { get; set; }

        public int? RetweetedPostId { get; set; }
        
        public PublicationModel RetweetedPublication { get; set; }
       

    }
}