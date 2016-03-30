using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iRocks.DataLayer
{

    public class Badge : DapperManagedObject<Badge>, IGetId, IDeeplyCloneable<Badge>
    {
        public Badge()
        {
            this.Labels = new List<BadgeTranslation>();
        }
        public int BadgeId { get; set; }
        public string Label { get; set; }
        public int Level { get; set; }
        public List<BadgeTranslation> Labels { get; set; }
        public string Explanation { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public bool IsNew
        {
            get
            {
                return this.BadgeId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public Badge DeepClone()
        {
            var res = new Badge()
            {
                BadgeId = this.BadgeId,
                Label = this.Label,
                Explanation = this.Explanation,
                Level = this.Level,
                CategoryId = this.CategoryId,
                Category= this.Category!=null? this.Category.DeepClone():null,
                Labels = new List<BadgeTranslation>(this.Labels.Select(x => x.DeepClone()))
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return BadgeId;
        }
    }
}
