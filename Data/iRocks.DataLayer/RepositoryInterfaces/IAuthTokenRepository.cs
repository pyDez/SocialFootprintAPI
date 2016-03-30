using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IAuthTokenRepository 
    {
        IEnumerable<AuthToken> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(AuthToken obj);
        void Update(AuthToken obj);
        void Delete(AuthToken obj);
    }
}
