using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface ITwitterPostDetailRepository
    {
        IEnumerable<TwitterPostDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(TwitterPostDetail obj);
        void Update(TwitterPostDetail obj);
        void Delete(TwitterPostDetail obj);
    }
}
