using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IStoryTranslationRepository 
    {
        IEnumerable<StoryTranslation> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(StoryTranslation obj);
        void Update(StoryTranslation obj);
        void Delete(StoryTranslation obj);
    }
}
