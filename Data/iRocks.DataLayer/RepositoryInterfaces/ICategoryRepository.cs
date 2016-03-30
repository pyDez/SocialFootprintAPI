using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface ICategoryRepository 
    {
        IEnumerable<Category> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Category obj);
        void Update(Category obj);
        void Delete(Category obj);
    }
}
