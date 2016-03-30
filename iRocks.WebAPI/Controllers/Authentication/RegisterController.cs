using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Helpers;
using iRocks.WebAPI.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace iRocks.WebAPI.Controllers
{
    [AllowAnonymous]
    public class RegisterController : BaseApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAuthenticationHelper _AuthenticationHelper;

        public RegisterController(IAuthenticationHelper authenticationHelper, ApplicationUserManager userManager, ApplicationSignInManager signInManager,
            IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _AuthenticationHelper = authenticationHelper;
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
      
        //
        // POST: /Auth/Register
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public async Task<HttpResponseMessage> Post([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, EmailAddress = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return Request.CreateResponse(HttpStatusCode.Created, _AuthenticationHelper.GenerateLocalAccessTokenResponse(user));
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

    }
}