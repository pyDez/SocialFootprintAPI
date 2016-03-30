using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IBadgeRepository 
    {
        IEnumerable<Badge> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Badge obj);
        void Update(Badge obj);
        void Delete(Badge obj);
    }
}
