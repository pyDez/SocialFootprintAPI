using System.Collections.Generic;
using System.Configuration;

namespace iRocks.DataLayer
{
    public class PostRelationshipDapperRepository : DapperRepositoryBase, IPostRelationshipRepository
    {

        public PostRelationshipDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<PostRelationship> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<PostRelationship>(criteria, ConditionalKeyWord);
        }

        public void Insert(PostRelationship obj)
        {
            base.Insert<PostRelationship>(obj);
        }

        public void Update(PostRelationship obj)
        {
            base.Update<PostRelationship>(obj);
        }

        public void Delete(PostRelationship obj)
        {
            base.Delete<PostRelationship>(obj);
        }
    }
}
