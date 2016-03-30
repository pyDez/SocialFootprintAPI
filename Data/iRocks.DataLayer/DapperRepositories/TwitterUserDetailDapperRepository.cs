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
    public class TwitterUserDetailDapperRepository : DapperRepositoryBase, ITwitterUserDetailRepository
    {

        public TwitterUserDetailDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<TwitterUserDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<TwitterUserDetail>(criteria, ConditionalKeyWord);
        }

        public void Insert(TwitterUserDetail obj)
        {
             base.Insert<TwitterUserDetail>(obj);
        }

        public void Update(TwitterUserDetail obj)
        {
            base.Update<TwitterUserDetail>(obj);
        }

        public void Delete(TwitterUserDetail obj)
        {
            base.Delete<TwitterUserDetail>(obj);
        }
    }
}
