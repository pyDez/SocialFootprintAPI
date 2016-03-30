using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IFacebookPostDetailRepository 
    {
        IEnumerable<FacebookPostDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(FacebookPostDetail obj);
        void Update(FacebookPostDetail obj);
        void Delete(FacebookPostDetail obj);
    }
}
