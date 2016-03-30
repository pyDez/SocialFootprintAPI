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
    public class WordProbabilityDapperRepository : DapperRepositoryBase, IWordProbabilityRepository
    {

        public WordProbabilityDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<WordProbability> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<WordProbability>(criteria, ConditionalKeyWord);
        }

        public void Insert(WordProbability obj)
        {
            base.Insert<WordProbability>(obj);
        }

        public void Update(WordProbability obj)
        {
            base.Update<WordProbability>(obj);
        }

        public void Delete(WordProbability obj)
        {
            base.Delete<WordProbability>(obj);
        }
    }
}
