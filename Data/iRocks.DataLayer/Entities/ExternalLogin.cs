using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class ExternalLogin : DapperManagedObject<ExternalLogin>, IGetId, IDeeplyCloneable<ExternalLogin>
    {
        public ExternalLogin()
        {

        }
        public int ExternalLoginId { get; set; }
        public int AppUserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public bool IsNew
        {
            get
            {
                return this.ExternalLoginId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public ExternalLogin DeepClone()
        {
            var res = new ExternalLogin()
            {
                ExternalLoginId = this.ExternalLoginId,
                AppUserId = this.AppUserId,
                LoginProvider = this.LoginProvider,
                ProviderKey = this.ProviderKey
                
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return ExternalLoginId;
        }
    }
}
