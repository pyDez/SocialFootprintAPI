using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IBadgeTranslationRepository 
    {
        IEnumerable<BadgeTranslation> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(BadgeTranslation obj);
        void Update(BadgeTranslation obj);
        void Delete(BadgeTranslation obj);
    }
}
