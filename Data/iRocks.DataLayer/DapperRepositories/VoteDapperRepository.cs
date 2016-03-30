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
    public class VoteDapperRepository : DapperRepositoryBase, IVoteRepository
    {

        public VoteDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<Vote> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<Vote>(criteria, ConditionalKeyWord);
        }

        public void Insert(Vote obj)
        {
             base.Insert<Vote>(obj);
        }

        public void Update(Vote obj)
        {
            base.Update<Vote>(obj);
        }

        public void Delete(Vote obj)
        {
            base.Delete<Vote>(obj);
        }
    }
}
