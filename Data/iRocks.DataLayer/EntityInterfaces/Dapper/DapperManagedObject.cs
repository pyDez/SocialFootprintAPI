using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class DapperManagedObject<T> : IDbSnapshot<T>
    {
        public Snapshotter.Snapshot<T> Snapshot;
        public void SetSnapshot(T obj)
        {
            this.Snapshot = Snapshotter.Start(obj);
        }

        public Dapper.DynamicParameters GetDifference(T obj)
        {
            var t = typeof(T);
            Snapshot.trackedObject = obj;
            var diff = Snapshot.Diff();
            var result = new Dapper.DynamicParameters();
            foreach(var property in diff)
            {
                
                var pi = t.GetProperty(property.Key);
                if (!Attribute.IsDefined(pi, typeof(DapperIgnore)))
                    result.Add(property.Key, property.Value);
            }
            return result;

        }
    }
}
