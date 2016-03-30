using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IPostUrlRepository
    {
        IEnumerable<PostUrl> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(PostUrl obj);
        void Update(PostUrl obj);
        void Delete(PostUrl obj);
    }
}
