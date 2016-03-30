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
    public class AuthTokenDapperRepository : DapperRepositoryBase, IAuthTokenRepository
    {

        public AuthTokenDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<AuthToken> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<AuthToken>(criteria, ConditionalKeyWord);
        }

        public void Insert(AuthToken obj)
        {
             base.Insert<AuthToken>(obj);
        }

        public void Update(AuthToken obj)
        {
            base.Update<AuthToken>(obj);
        }

        public void Delete(AuthToken obj)
        {
            base.Delete<AuthToken>(obj);
        }
    }
}
