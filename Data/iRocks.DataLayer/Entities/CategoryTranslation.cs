using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRocks.DataLayer
{
    public class CategoryTranslation : DapperManagedObject<CategoryTranslation>, IGetId, IDeeplyCloneable<CategoryTranslation>
    {
        public int CategoryTranslationId { get; set;} 
        public int CategoryId { get; set;} 
        public string Label{get; set;} 
        public string Locale{get; set;}
        public bool IsNew
        {
            get
            {
                return this.CategoryTranslationId == default(int);
            }
        }
        public int GetId()
        {
            return this.CategoryTranslationId;
        }

        public CategoryTranslation DeepClone()
        {
            var res = new CategoryTranslation()
            {
                CategoryTranslationId = this.CategoryTranslationId,
                CategoryId = this.CategoryId,
                Label = this.Label,
                Locale = this.Locale,
                Snapshot = this.Snapshot
            };
            return res;
        }
    }
}
