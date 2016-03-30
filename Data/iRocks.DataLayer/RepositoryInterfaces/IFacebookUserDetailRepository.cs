using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IFacebookUserDetailRepository 
    {
        IEnumerable<FacebookUserDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(FacebookUserDetail obj);
        void Update(FacebookUserDetail obj);
        void Delete(FacebookUserDetail obj);
    }
}
