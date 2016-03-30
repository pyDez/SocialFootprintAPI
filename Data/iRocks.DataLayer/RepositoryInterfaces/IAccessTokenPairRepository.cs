using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IAccessTokenPairRepository
    {
        IEnumerable<AccessTokenPair> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(AccessTokenPair obj);
        void Update(AccessTokenPair obj);
        void Delete(AccessTokenPair obj);
    }
}
