using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.AI.Helpers
{
    public static class FootprintHelper
    {
        public static List<Skill> GetNewFootprint(List<Category> categories, ICategoryRepository categoryRepository)
        {
            var footprint = new List<Skill>();
            if (!categories.Any())
                categories = categoryRepository.Select().ToList();
            foreach (var category in categories)
            {
                footprint.Add(new Skill()
                {
                    CategoryId = category.CategoryId,
                    SkillCategory = category,
                    SkillLevel = 50
                });
            }
            return footprint;
        }
    }
}
