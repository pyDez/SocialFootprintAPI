using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Helpers;
using iRocks.WebAPI.Models;
using iRocks.WebAPI.Results;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace iRocks.WebAPI.Controllers
{
    public class ExternalLoginController : BaseApiController
    {
        private IAccessTokenPairRepository _AccessTokenRepository;
        private IAuthenticationHelper _authenticationHelper;

        public ExternalLoginController(IAuthenticationHelper authenticationHelper,IAccessTokenPairRepository accessTokenRepository, IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            this._AccessTokenRepository = accessTokenRepository;
            this._authenticationHelper = authenticationHelper;
        }
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        // GET api/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Get(string provider,string returnUrl, string error = null, string error_description = null)
        {

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref returnUrl);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }
            
            AppUser user = await TheUserRepository.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));
            bool hasRegistered = false;
            if (user != null)
            {
                hasRegistered = user.Activated;
            }
            if (!hasRegistered)
            {
                if (externalLogin.LoginProvider == "Twitter")
                {
                    _AccessTokenRepository.Insert(new AccessTokenPair { AccessToken = externalLogin.ExternalAccessToken, AccessTokenSecret = externalLogin.ExternalAccessTokenSecret });
                }
            }
            returnUrl = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&external_email={5}",
                                            returnUrl,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.UserName,
                                            externalLogin.Email);

            return Redirect(returnUrl);

        }

        // POST api/ExternalLogin
        [AllowAnonymous]
        public async Task<IHttpActionResult> Post(ExternalLoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Provider or external access token is not sent");
            }
            //if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            //{
            //    return BadRequest("Provider or external access token is not sent");
            //}

            var verifiedAccessToken = await _authenticationHelper.VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            AppUser user = await TheUserRepository.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            //generate access token response
            var accessTokenResponse = _authenticationHelper.GenerateLocalAccessTokenResponse(user);
            return Ok(accessTokenResponse);

        }


        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriString)
        {

            Uri redirectUri;

            //redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }
            /*
            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _repo.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }*/

            redirectUriString = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }
            public string Email { get; set; }
            public string ExternalAccessTokenSecret { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue(GetAccessTokenKey(providerKeyClaim.Issuer)),
                    Email = identity.FindFirstValue(GetEmail(providerKeyClaim.Issuer)),
                    ExternalAccessTokenSecret= identity.FindFirstValue(GetAccessTokenSecret(providerKeyClaim.Issuer))
                };
            }
            private static string GetAccessTokenKey(string provider)
            {
                switch(provider)
                {
                    case "Facebook":
                        return "urn:facebook:access_token";
                    case "Twitter":
                        return "urn:twitter:access_token";
                    default:
                        return "ExternalAccessToken";
                }
            }
            private static string GetAccessTokenSecret(string provider)
            {
                switch (provider)
                {
                    case "Facebook":
                        return "ExternalAccessToken";
                    case "Twitter":
                        return "urn:twitter:access_token_secret";
                    default:
                        return "ExternalAccessToken";
                }
            }
            private static string GetEmail(string provider)
            {
                switch (provider)
                {
                    case "Facebook":
                        return "urn:facebook:email";
                    case "Twitter":
                        return "urn:twitter:email";
                    default:
                        return "ExternalEmail";
                }
            }
        }

    }
}
