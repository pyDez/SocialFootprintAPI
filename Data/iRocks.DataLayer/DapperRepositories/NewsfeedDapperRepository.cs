using System.Collections.Generic;
using System.Configuration;

namespace iRocks.DataLayer
{
    public class NewsfeedDapperRepository : DapperRepositoryBase, INewsfeedRepository
    {

        public NewsfeedDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<Newsfeed> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<Newsfeed>(criteria, ConditionalKeyWord);
        }

        public void Insert(Newsfeed obj)
        {
            base.Insert<Newsfeed>(obj);
        }

        public void Update(Newsfeed obj)
        {
            base.Update<Newsfeed>(obj);
        }

        public void Delete(Newsfeed obj)
        {
            base.Delete<Newsfeed>(obj);
        }
    }
}
