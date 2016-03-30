using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iRocks.DataLayer
{
    public class AuthToken : DapperManagedObject<AuthToken>, IGetId
    {
        public AuthToken()
        {
            this.Expiration = DateTime.Now.AddHours(5);

        }
        public int AuthTokenId { get; set; }
        public string Token { get; set; }

        public int AppUserId { get; set; }
        public DateTime Expiration { get; set; }

        public int GetId()
        {
            return AuthTokenId;
        }
    }
}
