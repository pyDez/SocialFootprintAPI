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
using Microsoft.AspNet.Identity;

namespace iRocks.DataLayer
{
    public class ExternalLoginDapperRepository : DapperRepositoryBase, IExternalLoginRepository
    {

        public ExternalLoginDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<ExternalLogin> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<ExternalLogin>(criteria, ConditionalKeyWord);
        }

        public void Insert(ExternalLogin obj)
        {
            base.Insert<ExternalLogin>(obj);
        }

        public void Update(ExternalLogin obj)
        {
            base.Update<ExternalLogin>(obj);
        }

        public void Delete(ExternalLogin obj)
        {
            base.Delete<ExternalLogin>(obj);
        }
       
    }
}
