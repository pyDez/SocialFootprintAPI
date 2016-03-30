using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Helpers;
using iRocks.WebAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace iRocks.WebAPI.Controllers
{
    public class LoginController : BaseApiController
    {

        private IAuthTokenRepository _tokenRepository;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAuthenticationHelper _authenticationHelper;

        public LoginController(IAuthenticationHelper authenticationHelper, ApplicationUserManager userManager, ApplicationSignInManager signInManager, IAuthTokenRepository authTokenRepository,
            IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {

            UserManager = userManager;
            SignInManager = signInManager;
            _tokenRepository = authTokenRepository;
            _authenticationHelper = authenticationHelper;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [AllowAnonymous]
        public async Task<HttpResponseMessage> Post(TokenRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid || model == null || model.Email == null || model.Password == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var user = TheUserRepository.Select(DephtLevel.UserBasic, new { UserName = model.Email }).FirstOrDefault(); //var user = TheUserRepository.Select(true).Where(u => u.AppId == model.ApiKey).FirstOrDefault();
                if (user != null)
                {
                    /*
                    var secret = user.SecurityStamp;
                    //simplistic implementation DO NOT USE
                    var key = Convert.FromBase64String(secret);
                    var provider = new System.Security.Cryptography.HMACSHA256(key);
                    //Compute Hash from API key (NOT SECURE)
                    var hash = provider.ComputeHash(Encoding.UTF8.GetBytes(user.Id.ToString()));
                    var signature = Convert.ToBase64String(hash);
                     */
                    /*
                   if (signature == model.Signature)
                   {
                       var rawTokenInfo = string.Concat(user.Id.ToString() + DateTime.UtcNow.ToString("d"));
                       var rawTokenByte = Encoding.UTF8.GetBytes(rawTokenInfo);
                       var token = provider.ComputeHash(rawTokenByte);
                       var authToken = new AuthToken()
                       {
                           Token = Convert.ToBase64String(token),
                           Expiration = DateTime.UtcNow.AddDays(1),
                           AppUserId = user.AppUserId
                       };
                       _tokenRepository.Insert(authToken);
                       return Request.CreateResponse(HttpStatusCode.Created, new { authToken.Expiration, authToken.Token });

                   }*/
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:

                            return Request.CreateResponse(HttpStatusCode.Created, _authenticationHelper.GenerateLocalAccessTokenResponse(user));
                        case SignInStatus.LockedOut:
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, "User is locked out");
                        case SignInStatus.RequiresVerification:
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, "Verification needed");
                        //return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid login attempt.");
                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }


       
    }
}
