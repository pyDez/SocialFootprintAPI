using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IHashtagRepository
    {
        IEnumerable<Hashtag> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Hashtag obj);
        void Update(Hashtag obj);
        void Delete(Hashtag obj);
    }
}
