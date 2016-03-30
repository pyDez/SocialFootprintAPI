using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IDbSnapshot<T>
    {
        void SetSnapshot(T obj);
        DynamicParameters GetDifference(T obj);
    }
}
