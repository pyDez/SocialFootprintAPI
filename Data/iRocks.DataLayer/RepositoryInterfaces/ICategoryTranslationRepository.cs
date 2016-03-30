using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface ICategoryTranslationRepository
    {
        IEnumerable<CategoryTranslation> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(CategoryTranslation obj);
        void Update(CategoryTranslation obj);
        void Delete(CategoryTranslation obj);
    }
}
