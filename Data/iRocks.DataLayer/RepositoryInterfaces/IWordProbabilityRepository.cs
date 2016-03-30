using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IWordProbabilityRepository 
    {
        IEnumerable<WordProbability> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(WordProbability obj);
        void Update(WordProbability obj);
        void Delete(WordProbability obj);
    }
}
