using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IBadgeCollectedRepository 
    {
        IEnumerable<BadgeCollected> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(BadgeCollected obj);
        void Update(BadgeCollected obj);
        void Delete(BadgeCollected obj);
    }
}
