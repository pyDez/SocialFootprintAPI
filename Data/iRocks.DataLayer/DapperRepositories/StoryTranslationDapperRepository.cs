using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

namespace iRocks.DataLayer
{
    public class StoryTranslationDapperRepository : DapperRepositoryBase, IStoryTranslationRepository
    {

        public StoryTranslationDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<StoryTranslation> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<StoryTranslation>(criteria, ConditionalKeyWord);
        }

        public void Insert(StoryTranslation obj)
        {
            base.Insert<StoryTranslation>(obj);
        }

        public void Update(StoryTranslation obj)
        {
            base.Update<StoryTranslation>(obj);
        }

        public void Delete(StoryTranslation obj)
        {
            base.Delete<StoryTranslation>(obj);
        }
    }
}
