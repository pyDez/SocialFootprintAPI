using iRocks.DataLayer;
using System;
using System.Collections.Generic;

namespace iRocks.WebAPI.Models
{
    public class PostModel
    {
        public int PostId { get; set; }

        public int AppUserId { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreationDate { get; set; }

        public CategoryModel PostCategory { get; set; }

        public List<Vote> UpVotes { get; set; }

        public List<Vote> DownVotes { get; set; }

        public FacebookPostDetailModel FacebookDetail { get; set; }
        public TwitterPostDetailModel TwitterDetail { get; set; }

        public int Score { get; set; }
        public bool IsFacebookProvided { get; set; }
        public bool IsTwitterProvided { get; set; }
    }
}