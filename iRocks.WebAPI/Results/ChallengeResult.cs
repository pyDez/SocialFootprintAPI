using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace iRocks.WebAPI.Results
{

    public class ChallengeResult : IHttpActionResult
    {
        private const string XsrfKey = "XsrfId";

        public ChallengeResult(string provider, ApiController controller)
        {
            this.LoginProvider = provider;
            this.Request = controller.Request;
        }
        public string LoginProvider { get; set; }
        public HttpRequestMessage Request { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Request.GetOwinContext().Authentication.Challenge(LoginProvider);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            return Task.FromResult(response);
        }
    }
}