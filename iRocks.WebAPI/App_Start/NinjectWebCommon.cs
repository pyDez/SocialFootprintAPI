[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(iRocks.WebAPI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(iRocks.WebAPI.App_Start.NinjectWebCommon), "Stop")]

namespace iRocks.WebAPI.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using iRocks.DataLayer;
    using System.Web.Http;
    using WebApiContrib.IoC.Ninject;
    using iRocks.WebAPI.Services;
    using iRocks.AI;
    using iRocks.AI.Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;
    using NClassifier;
    using NClassifier.Bayesian;
    using Helpers;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            //Support Web API
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IUserRepository>().To<UserDapperRepository>();
            kernel.Bind<IAuthenticationManager>().ToMethod(c => HttpContext.Current.GetOwinContext().Authentication).InRequestScope();
            kernel.Bind<IUserStore<AppUser>>().To<UserDapperRepository>();
            kernel.Bind<IAuthTokenRepository>().To<AuthTokenDapperRepository>();
            kernel.Bind<ICategoryRepository>().To<CategoryDapperRepository>();
            kernel.Bind<IFacebookPostDetailRepository>().To<FacebookPostDetailDapperRepository>();
            kernel.Bind<IFacebookUserDetailRepository>().To<FacebookUserDetailDapperRepository>();
            kernel.Bind<ITwitterPostDetailRepository>().To<TwitterPostDetailDapperRepository>();
            kernel.Bind<ITwitterUserDetailRepository>().To<TwitterUserDetailDapperRepository>();
            kernel.Bind<IExternalLoginRepository>().To<ExternalLoginDapperRepository>();
            kernel.Bind<IFriendshipRepository>().To<FriendshipDapperRepository>();
            kernel.Bind<IPostRepository>().To<PostDapperRepository>();
            kernel.Bind<ISkillRepository>().To<SkillDapperRepository>();
            kernel.Bind<IVoteRepository>().To<VoteDapperRepository>();
            kernel.Bind<INotificationRepository>().To<NotificationDapperRepository>();
            kernel.Bind<IWordProbabilityRepository>().To<WordProbabilityDapperRepository>();
            kernel.Bind<IBadgeCollectedRepository>().To<BadgeCollectedDapperRepository>();
            kernel.Bind<IAccessTokenPairRepository>().To<AccessTokenPairDapperRepository>();
            kernel.Bind<IBadgeRepository>().To<BadgeDapperRepository>();
            kernel.Bind<INewsFeedSorter>().To<NewsFeedSorter>();
            kernel.Bind<IRanking>().To<EloRanking>();
            kernel.Bind<ISocialNetworkDataFeed>().To<FacebookDataFeed>().WhenTargetHas<FacebookFeeder>();
            kernel.Bind<ISocialNetworkDataFeed>().To<TwitterDataFeed>().WhenTargetHas<TwitterFeeder>();
            kernel.Bind<ITrainableClassifier>().To<BayesianClassifier>();
            kernel.Bind<IAuthenticationHelper>().To<AuthenticationHelper>();
            kernel.Bind<INewsFeedHelper>().To<NewsFeedHelper>();
            kernel.Bind<iRocks.AI.IClassifier>().To<PredictiveClassifier>();
            
        }        
    }
}
