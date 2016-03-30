using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IVoteRepository 
    {
        IEnumerable<Vote> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Vote obj);
        void Update(Vote obj);
        void Delete(Vote obj);
    }
}
