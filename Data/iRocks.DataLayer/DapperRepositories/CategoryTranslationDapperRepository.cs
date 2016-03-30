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
    public class CategoryTranslationDapperRepository : DapperRepositoryBase, ICategoryTranslationRepository
    {

        public CategoryTranslationDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<CategoryTranslation> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<CategoryTranslation>(criteria, ConditionalKeyWord);
        }

        public void Insert(CategoryTranslation obj)
        {
            base.Insert<CategoryTranslation>(obj);
        }

        public void Update(CategoryTranslation obj)
        {
            base.Update<CategoryTranslation>(obj);
        }

        public void Delete(CategoryTranslation obj)
        {
            base.Delete<CategoryTranslation>(obj);
        }
    }
}
