using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IFriendshipRepository 
    {
        IEnumerable<Friendship> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Friendship obj);
        void Update(Friendship obj);
        void Delete(Friendship obj);
    }
}
