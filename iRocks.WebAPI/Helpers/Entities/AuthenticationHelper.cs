using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Models;
using LinqToTwitter;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.WebAPI.Helpers
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private IAccessTokenPairRepository _accessTokenRepo;
        public AuthenticationHelper(IAccessTokenPairRepository accessTokenRepo)
        {
            this._accessTokenRepo = accessTokenRepo;
        }

        public async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";
            switch (provider)
            {
                case "Facebook":

                    //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                    //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
                    var appToken = GetPartiesIdentificationHelper.FacebookAppToken;
                    verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
                    break;
                case "Twitter":
                    var accessTokenPair = _accessTokenRepo.Select(new { AccessToken = accessToken }).FirstOrDefault();
                    if (accessTokenPair != null)
                    {
                        IAuthorizer auth = new SingleUserAuthorizer
                        {
                            CredentialStore = new InMemoryCredentialStore()
                            {
                                ConsumerKey = GetPartiesIdentificationHelper.TwitterApiKey,
                                ConsumerSecret = GetPartiesIdentificationHelper.TwitterApiSecret,
                                OAuthToken = accessToken,
                                OAuthTokenSecret = accessTokenPair.AccessTokenSecret
                            }
                        };
                        var twitterCtx = new TwitterContext(auth);
                        try
                        {
                            var verifyResponse = await
                                (from acct in twitterCtx.Account
                                 where acct.Type == AccountType.VerifyCredentials
                                 select acct)
                                 .SingleOrDefaultAsync();

                            if (verifyResponse != null && verifyResponse.User != null)
                            {
                                parsedToken = new ParsedExternalAccessToken();
                                parsedToken.user_id = verifyResponse.User.UserIDResponse;
                                parsedToken.app_id = GetPartiesIdentificationHelper.TwitterApiKey;
                                parsedToken.user_secret = accessTokenPair.AccessTokenSecret;
                                return parsedToken;
                            }
                        }

                        catch (TwitterQueryException tqe)
                        {
                            return null;
                        }
                    }
                    else
                        return null;
                    break;

                default:
                    return null;
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(verifyTokenEndPoint),
                Method = HttpMethod.Get
            };

            
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                switch (provider)
                {
                    case "Facebook":
                        parsedToken.user_id = jObj["data"]["user_id"];
                        parsedToken.app_id = jObj["data"]["app_id"];

                        if (!string.Equals(GetPartiesIdentificationHelper.FacebookAppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                        {
                            return null;
                        }
                        break;
                    default:
                        return null;
                }


            }

            return parsedToken;
        }
        public JObject GenerateLocalAccessTokenResponse(AppUser user)
        {
            JObject tokenResponse;
            IAuthTokenRepository repo = (IAuthTokenRepository)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IAuthTokenRepository));
            var existingToken = repo.Select(new { AppUserId = user.AppUserId }).FirstOrDefault();
            if (existingToken != null && existingToken.Expiration > DateTime.UtcNow.AddHours(2))
            {
                tokenResponse = new JObject(
                                        new JProperty("userName", user.UserName),
                                        new JProperty("access_token", existingToken.Token),
                                        new JProperty("expiration", existingToken.Expiration.ToString()));

            }
            else
            {
                var secret = user.SecurityStamp.Replace("_", "/").Replace("-", "+");
                //simplistic implementation DO NOT USE
                var key = Convert.FromBase64String(secret);
                var provider = new System.Security.Cryptography.HMACSHA256(key);
                var rawTokenInfo = string.Concat(user.Id.ToString() + DateTime.UtcNow.ToString("d"));
                var rawTokenByte = Encoding.UTF8.GetBytes(rawTokenInfo);
                var token = provider.ComputeHash(rawTokenByte);
                AuthToken authToken;
                if (existingToken != null)
                {
                    existingToken.Token = Convert.ToBase64String(token);
                    existingToken.Expiration = DateTime.UtcNow.AddDays(1);
                    repo.Update(existingToken);
                    authToken = existingToken;
                }
                else
                {
                    authToken = new AuthToken()
                    {
                        Token = Convert.ToBase64String(token),
                        Expiration = DateTime.UtcNow.AddDays(1),
                        AppUserId = user.AppUserId
                    };
                    repo.Insert(authToken);
                }
                tokenResponse = new JObject(
                                        new JProperty("userName", user.UserName),
                                        new JProperty("access_token", authToken.Token),
                                        new JProperty("expiration", authToken.Expiration.ToString()));
            }
            return tokenResponse;


        }
        /*
        public static JObject GenerateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }
        */
    }
}