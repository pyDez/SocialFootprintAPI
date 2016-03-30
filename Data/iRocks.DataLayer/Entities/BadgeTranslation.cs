using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRocks.DataLayer
{
    public class BadgeTranslation : DapperManagedObject<BadgeTranslation>, IGetId, IDeeplyCloneable<BadgeTranslation>
    {
        public int BadgeTranslationId { get; set;} 
        public int BadgeId { get; set;} 
        public string Label{get; set;} 
        public string Locale{get; set;}
        public string Explanation { get; set; }
        public bool IsNew
        {
            get
            {
                return this.BadgeTranslationId == default(int);
            }
        }
        public int GetId()
        {
            return this.BadgeTranslationId;
        }

        public BadgeTranslation DeepClone()
        {
            var res = new BadgeTranslation()
            {
                BadgeTranslationId = this.BadgeTranslationId,
                BadgeId = this.BadgeId,
                Label = this.Label,
                Locale = this.Locale,
                Snapshot = this.Snapshot,
                Explanation = this.Explanation

            };
            return res;
        }
    }
}
