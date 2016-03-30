using iRocks.DataLayer;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace iRocks.WebAPI.Filters
{
    public class iRocksAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private IUserRepository _userRepository;
        private IAuthTokenRepository _authTokenRepository;
        private bool _basicAuth;
        public iRocksAuthorizeAttribute(bool basicAuth = false)
        {
            this._userRepository = (IUserRepository)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IUserRepository));
            this._authTokenRepository = (IAuthTokenRepository)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IAuthTokenRepository));
            this._basicAuth = basicAuth;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(authHeader.Parameter))
                {

                    var authToken = _authTokenRepository.Select(new { Token = authHeader.Parameter }).FirstOrDefault();
                    if (authToken != null)
                    {
                        var user = _userRepository.Select(DephtLevel.UserBasic, new { AppUserId = authToken.AppUserId }).FirstOrDefault();
                        if (user != null)
                        {
                            user.LastLogInDate = DateTime.Now;
                            _userRepository.Save(DephtLevel.UserBasic, user);
                            SetPrincipal(user.UserName);
                            return;
                        }
                    }

                }

            }



            /*
            const string APIKEYNAME = "apikey";
            const string TOKENNAME = "token";

            var query = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
            if (!string.IsNullOrWhiteSpace(query[APIKEYNAME]) &&
                !string.IsNullOrWhiteSpace(query[TOKENNAME]))
            {

                var apikey = query[APIKEYNAME];
                var token = query[TOKENNAME];

                var authToken = _authTokenRepository.Select(new { Token = token }).FirstOrDefault();
                if (authToken != null && authToken.AppUserId.ToString() == apikey && authToken.Expiration > DateTime.UtcNow)
                {

                    if (_basicAuth)
                    {
                        if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                        {
                            return;
                        }
                        var authHeader = actionContext.Request.Headers.Authorization;
                        if (authHeader != null)
                        {
                            if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                                !string.IsNullOrWhiteSpace(authHeader.Parameter))
                            {
                                var rawCredentials = authHeader.Parameter;
                                var encoding = Encoding.GetEncoding("iso-8859-1");
                                var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
                                var split = credentials.Split(':');
                                var userName = split[0];
                                var password = split[1];


                                var task = _userManager.FindAsync(userName, password); //await _userRepository.Fin();
                                AppUser user = task.Result;
                                if (user != null)
                                {
                                    var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                                    Thread.CurrentPrincipal = principal;
                                    return;
                                }

                            }

                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
             */
            if (actionContext.RequestContext.RouteData.Values.Values.Contains("AppUser"))
            {
                if (actionContext.RequestContext.RouteData.Values.Values.Count == 2)
                {
                    return;
                }
            }
            HandleUnauthorized(actionContext);
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            //il vaut virer ce truc en dur !!!
            //actionContext.Response.Headers.Add("WWW.Authenticate", "Basic Scheme='iRocks' location='http://localhost:55302/auth/login'");
        }

        private void SetPrincipal(string userName)
        {

            var principal = new GenericPrincipal(new GenericIdentity(userName), null);
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;
        }

    }
}