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
    public class AccessTokenPairDapperRepository : DapperRepositoryBase, IAccessTokenPairRepository
    {

        public AccessTokenPairDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<AccessTokenPair> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<AccessTokenPair>(criteria, ConditionalKeyWord);
        }

        public void Insert(AccessTokenPair obj)
        {
             base.Insert<AccessTokenPair>(obj);
        }

        public void Update(AccessTokenPair obj)
        {
            base.Update<AccessTokenPair>(obj);
        }

        public void Delete(AccessTokenPair obj)
        {
            base.Delete<AccessTokenPair>(obj);
        }
    }
}
