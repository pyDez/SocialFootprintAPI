using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IPostMentionedUserRepository
    {
        IEnumerable<PostMentionedUser> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(PostMentionedUser obj);
        void Update(PostMentionedUser obj);
        void Delete(PostMentionedUser obj);
    }
}
