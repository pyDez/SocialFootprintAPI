using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IExternalLoginRepository 
    {
        IEnumerable<ExternalLogin> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(ExternalLogin obj);
        void Update(ExternalLogin obj);
        void Delete(ExternalLogin obj);
    }
}
