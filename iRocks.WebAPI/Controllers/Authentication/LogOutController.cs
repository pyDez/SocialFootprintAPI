using iRocks.DataLayer;
using Microsoft.Owin.Security;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;

namespace iRocks.WebAPI.Controllers
{
    public class LogOutController : BaseApiController
    {
        public LogOutController(IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {

        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
        public HttpResponseMessage Post()
        {
            AuthenticationManager.SignOut();
            Thread.CurrentPrincipal = null;
            HttpContext.Current.User = null;
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
