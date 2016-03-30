using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface INotificationRepository 
    {
        IEnumerable<Notification> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Insert(Notification obj);
        void Update(Notification obj);
        void Delete(Notification obj);
    }
}
