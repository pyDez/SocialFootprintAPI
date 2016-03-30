using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface ITwitterUserDetailRepository
    {
        IEnumerable<TwitterUserDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(TwitterUserDetail obj);
        void Update(TwitterUserDetail obj);
        void Delete(TwitterUserDetail obj);
    }
}
