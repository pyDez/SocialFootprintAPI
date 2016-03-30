using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRocks.DataLayer
{
    public class StoryTranslation : DapperManagedObject<StoryTranslation>, IGetId, IDeeplyCloneable<StoryTranslation>
    {
        public int StoryTranslationId{get; set;} 
        public int FacebookPostDetailId{get; set;} 
        public string Story{get; set;} 
        public string Locale{get; set;}
        public bool IsNew
        {
            get
            {
                return this.StoryTranslationId == default(int);
            }
        }
        public int GetId()
        {
            return this.StoryTranslationId;
        }

        public StoryTranslation DeepClone()
        {
            var res = new StoryTranslation()
            {
                StoryTranslationId = this.StoryTranslationId,
                FacebookPostDetailId = this.FacebookPostDetailId,
                Story = this.Story,
                Locale = this.Locale,
                Snapshot = this.Snapshot
            };
            return res;
        }
    }
}
