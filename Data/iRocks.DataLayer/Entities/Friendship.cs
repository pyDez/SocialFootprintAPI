using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iRocks.DataLayer
{

    public class Friendship : DapperManagedObject<Friendship>, IGetId
    {
        public int FriendshipId { get; set; }
        public int AppUserAId { get; set; }
        public int AppUserBId { get; set; }

        public int GetId()
        {
            return FriendshipId;
        }
    }
}
