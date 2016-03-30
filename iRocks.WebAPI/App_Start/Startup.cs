using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Services;
using log4net.Config;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(iRocks.WebAPI.Startup))]
namespace iRocks.WebAPI
{
    
    public class Startup
    {
        const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        //public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            //ConfigureOAuth(app);
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //WebApiConfig.Register(config);


            //config Log4Net
            var configFile = new FileInfo(HttpContext.Current.Server.MapPath("log4net.config"));
            XmlConfigurator.Configure(configFile);


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                //LoginPath = new PathString("/auth/login")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            ConfigureOAuth(app);

            if (GetPartiesIdentificationHelper.FacebookAppId.Length > 0)
            {
                var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
                {
                    AppId = GetPartiesIdentificationHelper.FacebookAppId,
                    AppSecret = GetPartiesIdentificationHelper.FacebookAppSecret,
                    Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                    {
                        OnAuthenticated = (context) =>
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token", context.AccessToken, XmlSchemaString, "Facebook"));
                            foreach (var x in context.User)
                            {
                                var claimType = string.Format("urn:facebook:{0}", x.Key);
                                string claimValue = x.Value.ToString();
                                if (!context.Identity.HasClaim(claimType, claimValue))
                                    context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, XmlSchemaString, "Facebook"));

                            }
                            return Task.FromResult(0);
                        }
                    }

                };
                facebookOptions.Scope.Add("email");
                facebookOptions.Scope.Add("user_birthday");
                facebookOptions.Scope.Add("read_stream");
                facebookOptions.Scope.Add("user_relationships");
                facebookOptions.Scope.Add("user_about_me");
                facebookOptions.Scope.Add("user_photos");
                app.UseFacebookAuthentication(facebookOptions);


            }

            if (GetPartiesIdentificationHelper.TwitterApiKey.Length > 0)
            {
                var twitterOptions = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions()
                {
                    ConsumerKey = GetPartiesIdentificationHelper.TwitterApiKey,
                    ConsumerSecret = GetPartiesIdentificationHelper.TwitterApiSecret,
                    Provider = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationProvider()
                    {
                        OnAuthenticated = (context) =>
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_token", context.AccessToken, XmlSchemaString, "Twitter"));
                            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_token_secret", context.AccessTokenSecret, XmlSchemaString, "Twitter"));
                            return Task.FromResult(0);
                        }
                    },
                    BackchannelCertificateValidator = new Microsoft.Owin.Security.CertificateSubjectKeyIdentifierValidator(new[]
                        {
                            "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                            "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                            "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                            "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                            "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
                            "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
                            "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
                        })
                };
                app.UseTwitterAuthentication(twitterOptions);


            }
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                ApplicationCanDisplayErrors = true
            };
        }

    }
}