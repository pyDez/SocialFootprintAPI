using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IPostRepository 
    {
        IEnumerable<Post> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Update(Post obj);
        void Insert(Post obj);
        void Delete(Post obj);
    }

    public sealed class Provider
    {
        public Provider(string value)
        {
            Value = value;
        }
        public static readonly Provider Twitter = new Provider("Twitter");
        public static readonly Provider Facebook = new Provider("Facebook");
        public string Value { get; private set; }
        //public static implicit operator string(Provider provider) { return provider.Value; }


    }
}
