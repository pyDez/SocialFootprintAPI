using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Helpers;
using iRocks.WebAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace iRocks.WebAPI.Controllers
{
    public class ExternalRegisterController : BaseApiController
    {
         private IAuthTokenRepository _tokenRepository;
        private ApplicationUserManager _userManager;
        private IAuthenticationHelper _authenticationHelper;

        public ExternalRegisterController(IAuthenticationHelper authenticationHelper, ApplicationUserManager userManager, IAuthTokenRepository authTokenRepository,
            IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            UserManager = userManager;
            _tokenRepository = authTokenRepository;
            _authenticationHelper = authenticationHelper;
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
        public async Task<IHttpActionResult> Post(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await _authenticationHelper.VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }
            //recherche si il existe déjà dans les utilisateurs ayant un "externalregister"
            AppUser user = await TheUserRepository.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));
            if (user == null)
            {
                switch (model.Provider)
                {
                    case "Facebook":
                        FacebookUserDetailDapperRepository FacebookUserDetailRepository = new FacebookUserDetailDapperRepository();
                        var FacebookDetails = FacebookUserDetailRepository.Select(new { FacebookUserId = verifiedAccessToken.user_id });
                        if (FacebookDetails.Any())
                            user = TheUserRepository.Select(DephtLevel.UserBasic, new { AppUserId = FacebookDetails.SingleOrDefault().AppUserId }).SingleOrDefault();
                        break;
                    case "Twitter":
                        TwitterUserDetailDapperRepository TwitterUserDetailRepository = new TwitterUserDetailDapperRepository();
                        var TwitterDetails = TwitterUserDetailRepository.Select(new { TwitterUserId = verifiedAccessToken.user_id });
                        if (TwitterDetails.Any())
                            user = TheUserRepository.Select(DephtLevel.UserBasic, new { AppUserId = TwitterDetails.SingleOrDefault().AppUserId }).SingleOrDefault();
                        break;
                    default:
                        break;
                }
            }

            if (user != null)
            {
                if (user.Activated)
                {
                    return BadRequest("External user is already registered");
                }
                switch (model.Provider)
                {
                    case "Facebook":
                        user.FacebookDetail.FacebookAccessToken = model.ExternalAccessToken;
                        user.FacebookDetail.FacebookUserId = verifiedAccessToken.user_id;
                        
                        break;
                    case "Twitter":
                        user.TwitterDetail.TwitterAccessToken = model.ExternalAccessToken;
                        user.TwitterDetail.TwitterAccessTokenSecret = verifiedAccessToken.user_secret;
                        user.TwitterDetail.TwitterUserId = verifiedAccessToken.user_id;
                        break;
                    default:
                        break;
                }
                user.UserName = model.Email;
            }
            else
            {
                user = new AppUser() { UserName = model.Email, FirstName = model.UserName, Activated = true };
                switch (model.Provider)
                {
                    case "Facebook":
                        user.FacebookDetail = new FacebookUserDetail() { FacebookAccessToken = model.ExternalAccessToken, FacebookUserId = verifiedAccessToken.user_id };
                        user.FacebookDetail.SetSnapshot(user.FacebookDetail);

                        break;
                    case "Twitter":
                        user.TwitterDetail = new TwitterUserDetail() { TwitterAccessToken = model.ExternalAccessToken, TwitterAccessTokenSecret = verifiedAccessToken.user_secret, TwitterUserId = verifiedAccessToken.user_id };
                         user.TwitterDetail.SetSnapshot(user.TwitterDetail);
                        break;
                    default:
                        break;
                }
               
            }
            user.SigningUpDate = DateTime.Now;

           
            var result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var info = new ExternalLoginInfo()
            {
                DefaultUserName = model.Email,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
            };

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //generate access token response
            var accessTokenResponse = _authenticationHelper.GenerateLocalAccessTokenResponse(user);

            return Ok(accessTokenResponse);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

    }
}
