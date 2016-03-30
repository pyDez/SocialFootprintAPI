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
using System.Reflection;

namespace iRocks.DataLayer
{
    public class FriendshipDapperRepository : DapperRepositoryBase, IFriendshipRepository
    {

        public FriendshipDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<Friendship> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<Friendship>(criteria, ConditionalKeyWord);
        }

        public void Insert(Friendship obj)
        {
             base.Insert<Friendship>(obj);
        }

        public void Update(Friendship obj)
        {
            base.Update<Friendship>(obj);
        }

        public void Delete(Friendship obj)
        {
            base.Delete<Friendship>(obj);
        }
    }
}
