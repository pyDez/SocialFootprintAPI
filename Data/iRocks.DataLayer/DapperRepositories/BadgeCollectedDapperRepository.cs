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
    public class BadgeCollectedDapperRepository : DapperRepositoryBase, IBadgeCollectedRepository
    {

        public BadgeCollectedDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<BadgeCollected> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            //return base.Select<BadgeCollected>(criteria, ConditionalKeyWord);
            IBadgeRepository badgeTranslationRepository = new BadgeDapperRepository();
            var badgesCollected = base.Select<BadgeCollected>(criteria, ConditionalKeyWord);
            var badgeIds = badgesCollected.Select(b => b.BadgeId);
            var badges = badgeTranslationRepository.Select(new { BadgeId = badgeIds.ToList() });
            badgesCollected.ToList().ForEach(bc => bc.Badge = badges.Where(b => b.BadgeId == bc.BadgeId).FirstOrDefault());
            return badgesCollected;
        }

        public void Insert(BadgeCollected obj)
        {
            //SaveBadge(obj);
            base.Insert<BadgeCollected>(obj);
        }

        public void Update(BadgeCollected obj)
        {
            //SaveBadge(obj);
            base.Update<BadgeCollected>(obj);
        }

        public void Delete(BadgeCollected obj)
        {
            base.Delete<BadgeCollected>(obj);
        }

        private void SaveBadge(BadgeCollected obj)
        {
            IBadgeRepository badgeTranslationRepository = new BadgeDapperRepository();
            if (obj.Badge != null)
            {
                if (obj.Badge.IsNew)
                {
                    badgeTranslationRepository.Insert(obj.Badge);
                }
                else
                    badgeTranslationRepository.Update(obj.Badge);
            }

        }
    }
}
