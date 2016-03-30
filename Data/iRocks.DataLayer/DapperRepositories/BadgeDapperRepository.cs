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
    public class BadgeDapperRepository : DapperRepositoryBase, IBadgeRepository
    {

        public BadgeDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<Badge> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            var categoryRepository = new CategoryDapperRepository();
            var lookup = new Dictionary<int, Badge>();
            var badges = base.Select<Badge, BadgeTranslation, Badge>(
                           @"SELECT * FROM Badge LEFT OUTER JOIN BadgeTranslation  ON Badge.BadgeId = BadgeTranslation.BadgeId ",

                         (Badge, BadgeTranslation) =>
                         {
                             Badge aBadge;
                             if (!lookup.TryGetValue(Badge.BadgeId, out aBadge))
                             {
                                 lookup.Add(Badge.BadgeId, aBadge = Badge);
                             }
                             if (BadgeTranslation != null)
                             {
                                 BadgeTranslation.SetSnapshot(BadgeTranslation);
                                 aBadge.Labels.Add(BadgeTranslation);
                             }
                             //aPost.SetSnapshot(aPost);
                             return aBadge;
                         },
                         criteria,
                         ConditionalKeyWord,
                         splitOn: "BadgeTranslationId"
                         );


            var categoryIds = lookup.Values.Select(u => u.CategoryId).ToList();

            var categories = categoryRepository.Select(new { CategoryId = categoryIds });
            foreach (var badge in lookup.Values)
            {
                badge.Category = categories.Where(n => n.CategoryId == badge.CategoryId).FirstOrDefault();
            }

            return lookup.Values;
            //return base.Select<Badge>(criteria, ConditionalKeyWord);
        }

        public void Insert(Badge obj)
        {
            base.Insert<Badge>(obj);
            SaveTranslations(obj);
        }

        public void Update(Badge obj)
        {
            base.Update<Badge>(obj);
            SaveTranslations(obj);
        }

        public void Delete(Badge obj)
        {
            base.Delete<Badge>(obj);
        }
        private void SaveTranslations(Badge obj)
        {
            IBadgeTranslationRepository BadgeTranslationRepository = new BadgeTranslationDapperRepository();
            foreach (var label in obj.Labels)
            {
                if (label.IsNew)
                {
                    label.BadgeId = obj.BadgeId;
                    BadgeTranslationRepository.Insert(label);
                }
                //else if (vote.IsDeleted)
                //    VoteRepository.Delete(vote);

                else
                    BadgeTranslationRepository.Update(label);
            }
        }
    }
}
