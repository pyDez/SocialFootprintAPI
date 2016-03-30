using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.AI
{
    public interface IRanking
    {
        Tuple<double, double> getNewRankings(double scoreA, double scoreB, bool AWin);
    }
}
