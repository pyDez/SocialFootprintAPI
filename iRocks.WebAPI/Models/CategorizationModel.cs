using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRocks.WebAPI.Models
{
    public class CategorizationModel
    {
        public int PostId { get; set; }
        public int CategoryId { get; set; }
        public List<int> PostsBlackList { get; set; }
    }
}