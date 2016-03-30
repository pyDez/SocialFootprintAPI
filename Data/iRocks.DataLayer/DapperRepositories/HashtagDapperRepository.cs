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
    public class HashtagDapperRepository : DapperRepositoryBase, IHashtagRepository
    {

        public HashtagDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<Hashtag> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<Hashtag>(criteria, ConditionalKeyWord);
        }

        public void Insert(Hashtag obj)
        {
             base.Insert<Hashtag>(obj);
        }

        public void Update(Hashtag obj)
        {
            base.Update<Hashtag>(obj);
        }

        public void Delete(Hashtag obj)
        {
            base.Delete<Hashtag>(obj);
        }
    }
}
