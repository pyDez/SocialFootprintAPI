using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class Duel
    {
        public Duel(Publication theFirstPublication, Publication theSecondPublication)
        {
            this.FirstPublication = theFirstPublication;
            this.SecondPublication = theSecondPublication;
        }
        public Duel(Publication theFirstPublication, Publication theSecondPublication, Vote duelResult)
        {
            this.FirstPublication = theFirstPublication;
            this.SecondPublication = theSecondPublication;
            this.DuelResult = duelResult;
        }
        public Publication FirstPublication { get; set; }
        public Publication SecondPublication { get; set; }
        public Vote DuelResult { get; set; }
    }
}
