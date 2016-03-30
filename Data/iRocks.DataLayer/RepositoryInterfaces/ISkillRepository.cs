using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface ISkillRepository 
    {
        IEnumerable<Skill> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Skill obj);
        void Update(Skill obj);
        void Delete(Skill obj);
    }
}
