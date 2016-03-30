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
    public class PostMediaDapperRepository : DapperRepositoryBase, IPostMediaRepository
    {

        public PostMediaDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<PostMedia> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<PostMedia>(criteria, ConditionalKeyWord);
        }

        public void Insert(PostMedia obj)
        {
             base.Insert<PostMedia>(obj);
        }

        public void Update(PostMedia obj)
        {
            base.Update<PostMedia>(obj);
        }

        public void Delete(PostMedia obj)
        {
            base.Delete<PostMedia>(obj);
        }
    }
}
