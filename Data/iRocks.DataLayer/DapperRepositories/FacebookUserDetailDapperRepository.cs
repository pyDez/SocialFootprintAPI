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
    public class FacebookUserDetailDapperRepository : DapperRepositoryBase, IFacebookUserDetailRepository
    {

        public FacebookUserDetailDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<FacebookUserDetail> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<FacebookUserDetail>(criteria, ConditionalKeyWord);
        }

        public void Insert(FacebookUserDetail obj)
        {
             base.Insert<FacebookUserDetail>(obj);
        }

        public void Update(FacebookUserDetail obj)
        {
            base.Update<FacebookUserDetail>(obj);
        }

        public void Delete(FacebookUserDetail obj)
        {
            base.Delete<FacebookUserDetail>(obj);
        }
    }
}
