using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IPostRelationshipRepository
    {
        IEnumerable<PostRelationship> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(PostRelationship obj);
        void Update(PostRelationship obj);
        void Delete(PostRelationship obj);
    }
}
