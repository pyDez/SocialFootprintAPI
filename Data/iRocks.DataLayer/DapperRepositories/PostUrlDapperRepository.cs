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
    public class PostUrlDapperRepository : DapperRepositoryBase, IPostUrlRepository
    {

        public PostUrlDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<PostUrl> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<PostUrl>(criteria, ConditionalKeyWord);
        }

        public void Insert(PostUrl obj)
        {
             base.Insert<PostUrl>(obj);
        }

        public void Update(PostUrl obj)
        {
            base.Update<PostUrl>(obj);
        }

        public void Delete(PostUrl obj)
        {
            base.Delete<PostUrl>(obj);
        }
    }
}
