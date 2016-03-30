using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IPostMediaRepository
    {
        IEnumerable<PostMedia> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(PostMedia obj);
        void Update(PostMedia obj);
        void Delete(PostMedia obj);
    }
}
