using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class Category : DapperManagedObject<Category>, IGetId, IDeeplyCloneable<Category>
    {
        public Category()
        {
            this.Labels = new List<CategoryTranslation>();
        }
        
        public int CategoryId { get; set; }
        
        public string Label { get; set; }
        public List<CategoryTranslation> Labels { get; set; }

        public bool IsNew
        {
            get
            {
                return this.CategoryId == default(int);
            }
        }
        public Category DeepClone()
        {
            var res = new Category()
            {
                CategoryId = this.CategoryId,
                Label = this.Label,
                Labels = new List<CategoryTranslation>(this.Labels.Select(x => x.DeepClone()))
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return CategoryId;
        }

    }
}
