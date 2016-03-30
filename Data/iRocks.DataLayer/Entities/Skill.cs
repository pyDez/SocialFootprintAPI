using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class Skill : DapperManagedObject<Skill>, IGetId, IDeeplyCloneable<Skill>
    {
        public Skill()
        {
            this.skillCategory = new Category();
        }
        private double skillLevel;
        private Category skillCategory;

        
        public int SkillId { get; set; }
        
        public int AppUserId { get; set; }
        
        public int CategoryId { get; set; }

        [DapperDoNotIgnore]
        public double MaxSkillLevel { get; private set; }

        public double SkillLevel
        {
            get { return this.skillLevel; }
            set
            {
                this.skillLevel = value;
                if (this.skillLevel > this.MaxSkillLevel)
                    this.MaxSkillLevel = this.skillLevel;
            }
        }
        
        public Category SkillCategory
        {
            get { return this.skillCategory; }
            set
            {
                this.skillCategory = value;
                this.CategoryId = value.CategoryId;
            }
        }
        
        public bool IsNew
        {
            get
            {
                return this.SkillId == default(int);
            }
        }
        
        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public Skill DeepClone()
        {
            var res = new Skill()
             {
                 SkillId = this.SkillId,
                 AppUserId = this.AppUserId,
                 CategoryId = this.CategoryId,
                 MaxSkillLevel = this.MaxSkillLevel,
                 SkillLevel = this.SkillLevel,
                 SkillCategory = this.SkillCategory.DeepClone()
             };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return SkillId;
        }
    }
}
