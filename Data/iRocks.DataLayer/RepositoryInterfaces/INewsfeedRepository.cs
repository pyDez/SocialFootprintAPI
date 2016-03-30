using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface INewsfeedRepository
    {
        IEnumerable<Newsfeed> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Newsfeed obj);
        void Update(Newsfeed obj);
        void Delete(Newsfeed obj);
    }
}
