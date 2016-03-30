using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace iRocks.DataLayer
{

    public class WordProbability : DapperManagedObject<WordProbability>, IGetId, IDeeplyCloneable<WordProbability>
    {
        public WordProbability()
        {

        }

        public int WordProbabilityId { get; set; }
        public string Word { get; set; }
        public int CategoryId { get; set; }
        public long Matches { get; set; }
        public long NonMatches { get; set; }

        public bool IsNew
        {
            get
            {
                return this.WordProbabilityId == default(int);
            }
        }
        public WordProbability DeepClone()
        {
            var res = new WordProbability()
            {
                WordProbabilityId = this.WordProbabilityId,
                Word = this.Word,
                CategoryId = this.CategoryId,
                Matches = this.Matches,
                NonMatches = this.NonMatches,
                Snapshot = this.Snapshot
            };

            return res;
        }
        public int GetId()
        {
            return WordProbabilityId;
        }

    }
}
