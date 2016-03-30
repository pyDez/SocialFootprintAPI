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
    public class PostMentionedUserDapperRepository : DapperRepositoryBase, IPostMentionedUserRepository
    {

        public PostMentionedUserDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        public IEnumerable<PostMentionedUser> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<PostMentionedUser>(criteria, ConditionalKeyWord);
        }

        public void Insert(PostMentionedUser obj)
        {
             base.Insert<PostMentionedUser>(obj);
        }

        public void Update(PostMentionedUser obj)
        {
            base.Update<PostMentionedUser>(obj);
        }

        public void Delete(PostMentionedUser obj)
        {
            base.Delete<PostMentionedUser>(obj);
        }
    }
}
