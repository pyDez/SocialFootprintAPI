using Facebook;
using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iRocks.AI
{
    public static class FacebookVideoHelper
    {
        public static string GetVideoSource( string oldSource, string accessToken, string postId)
        {
            
            try {
                var client = new FacebookClient(accessToken);
                dynamic localeFB = client.Get(postId, new { fields = "source" });
                return localeFB.source;
            }
            catch(Exception ex)
            {
                return "";
            }
            
        }
    }



}
