using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IDeeplyCloneable<T>  where T : IDeeplyCloneable<T>
    {
        T DeepClone();
    }
}
