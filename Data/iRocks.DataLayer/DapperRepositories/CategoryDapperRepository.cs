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
    public class CategoryDapperRepository : DapperRepositoryBase, ICategoryRepository
    {

        public CategoryDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<Category> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            var lookup = new Dictionary<int, Category>();
            var categories = base.Select<Category, CategoryTranslation, Category>(
                           @"SELECT * FROM Category LEFT OUTER JOIN CategoryTranslation  ON Category.CategoryId = CategoryTranslation.CategoryId ",

                         (category, categoryTranslation) =>
                         {
                             Category aCategory;
                             if (!lookup.TryGetValue(category.CategoryId, out aCategory))
                             {
                                 lookup.Add(category.CategoryId, aCategory = category);
                             }
                             if (categoryTranslation != null)
                             {
                                 categoryTranslation.SetSnapshot(categoryTranslation);
                                 aCategory.Labels.Add(categoryTranslation);
                             }
                             //aPost.SetSnapshot(aPost);
                             return aCategory;
                         },
                         criteria,
                         ConditionalKeyWord,
                         splitOn: "CategoryTranslationId"
                         );
            return lookup.Values;
            //return base.Select<Category>(criteria, ConditionalKeyWord);
        }

        public void Insert(Category obj)
        {
             base.Insert<Category>(obj);
            SaveTranslations(obj);
        }

        public void Update(Category obj)
        {
            base.Update<Category>(obj);
            SaveTranslations(obj);
        }

        public void Delete(Category obj)
        {
            base.Delete<Category>(obj);
        }
        private void SaveTranslations(Category obj)
        {
            ICategoryTranslationRepository categoryTranslationRepository = new CategoryTranslationDapperRepository();
            foreach (var label in obj.Labels)
            {
                if (label.IsNew)
                {
                    label.CategoryId = obj.CategoryId;
                    categoryTranslationRepository.Insert(label);
                }
                //else if (vote.IsDeleted)
                //    VoteRepository.Delete(vote);

                else
                    categoryTranslationRepository.Update(label);
            }
        }
    }
}
