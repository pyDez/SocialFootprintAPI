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
    public class SkillDapperRepository : DapperRepositoryBase, ISkillRepository
    {

        public SkillDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }
        /// Automatic generation of SELECT statement, BUT only for simple equality criterias!
        /// Example: Select<LogItem>(new {Class = "Client"})
        /// For more complex criteria it is necessary to call GetItems method with custom SQL statement.
        /// </summary>
        public IEnumerable<Skill> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {

            ICategoryRepository categoryRepositiory = new CategoryDapperRepository();

            var lookup = new Dictionary<int, Skill>();
            var categoryIds = new List<int>();
            var skills = base.Select<Skill,  int?, Skill>(
                            @"SELECT Skill.*, Category.CategoryId FROM Skill LEFT OUTER JOIN Category  ON Category.CategoryId = Skill.CategoryId",

                          (skill, categoryId) =>
                          {
                              Skill aSkill;
                              if (!lookup.TryGetValue(skill.SkillId, out aSkill))
                              {
                                  lookup.Add(skill.SkillId, aSkill = skill);
                                  if (categoryId.HasValue)
                                      categoryIds.Add(categoryId.Value);
                              }
                              return aSkill;
                          },
                          criteria,
                          ConditionalKeyWord,
                          splitOn: "CategoryId"
                          );

            var categories = categoryRepositiory.Select(new { CategoryId = categoryIds }).ToList();
            lookup.Values.ToList().ForEach(s => s.SkillCategory = categories.Where(c => c.CategoryId == s.CategoryId).FirstOrDefault());

            return lookup.Values;





        }

        public void Insert(Skill obj)
        {
             base.Insert<Skill>(obj);
        }

        public void Update(Skill obj)
        {
            base.Update<Skill>(obj);
        }

        public void Delete(Skill obj)
        {
            base.Delete<Skill>(obj);
        }
    }
}
