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
    public class BadgeTranslationDapperRepository : DapperRepositoryBase, IBadgeTranslationRepository
    {

        public BadgeTranslationDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<BadgeTranslation> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            return base.Select<BadgeTranslation>(criteria, ConditionalKeyWord);
        }

        public void Insert(BadgeTranslation obj)
        {
            base.Insert<BadgeTranslation>(obj);
        }

        public void Update(BadgeTranslation obj)
        {
            base.Update<BadgeTranslation>(obj);
        }

        public void Delete(BadgeTranslation obj)
        {
            base.Delete<BadgeTranslation>(obj);
        }
    }
}
