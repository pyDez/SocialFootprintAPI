using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web.Http;

namespace iRocks.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
               name: "User",
               routeTemplate: "api/user/{id}",
               defaults: new { controller = "AppUser", id = RouteParameter.Optional }
           );



            config.Routes.MapHttpRoute(
              name: "Posts",
              routeTemplate: "api/post/{id}",
              defaults: new { controller = "Post", id = RouteParameter.Optional }
          );
            config.Routes.MapHttpRoute(
              name: "Notifications",
              routeTemplate: "api/notification/",
              defaults: new { controller = "Notification"}
          );

            config.Routes.MapHttpRoute(
            name: "Categories",
            routeTemplate: "api/category",
            defaults: new { controller = "Category" }
        );
            config.Routes.MapHttpRoute(
           name: "NewsFeed",
           routeTemplate: "api/currentuser/NewsFeed/",
           defaults: new { controller = "NewsFeed" }
       );
            config.Routes.MapHttpRoute(
          name: "UpPostsFeed",
          routeTemplate: "api/currentuser/UpPostsFeed/",
          defaults: new { controller = "UpPostsFeed" }
      );
            config.Routes.MapHttpRoute(
          name: "DownPostsFeed",
          routeTemplate: "api/currentuser/DownPostsFeed/",
          defaults: new { controller = "DownPostsFeed" }
      );


            config.Routes.MapHttpRoute(
             name: "CurrentUser",
             routeTemplate: "api/currentuser/",
             defaults: new { controller = "currentuser" }
         );
            config.Routes.MapHttpRoute(
             name: "Votes",
             routeTemplate: "api/currentuser/vote/{id}",
             defaults: new { controller = "vote", id = RouteParameter.Optional }
         );

            config.Routes.MapHttpRoute(
             name: "Login",
             routeTemplate: "api/Login",
             defaults: new { controller = "Login" }
         );
            config.Routes.MapHttpRoute(
             name: "Register",
             routeTemplate: "api/Register",
             defaults: new { controller = "Register" }
         );
            config.Routes.MapHttpRoute(
            name: "ExternalLogin",
            routeTemplate: "api/ExternalLogin",
            defaults: new { controller = "ExternalLogin" }
        );
            config.Routes.MapHttpRoute(
            name: "ExternalRegister",
            routeTemplate: "api/ExternalRegister",
            defaults: new { controller = "ExternalRegister" }
        );
            config.Routes.MapHttpRoute(
            name: "LogOut",
            routeTemplate: "api/LogOut",
            defaults: new { controller = "LogOut" }
        );
            config.Routes.MapHttpRoute(
           name: "email",
           routeTemplate: "api/email",
           defaults: new { controller = "email" }
       );


            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            //serialization en JSON : les objets sont passé par valeurs. 1 objet peut donc être envoyé 2 fois
            //décommenter la ligne ci dessous pour activer
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //serialization en JSON : les objets sont passé par référence. un systeme de clé id/ref est mis en place pour renvoyer vers le bon objet en cas de redondance
            //décommenter les deux lignes ci dessous pour activer
            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;


            // Supprimez les commentaires de la ligne de code suivante pour activer la prise en charge des requêtes pour les actions ayant un type de retour IQueryable ou IQueryable<T>.
            // Pour éviter le traitement de requêtes inattendues ou malveillantes, utilisez les paramètres de validation définis sur QueryableAttribute pour valider les requêtes entrantes.
            // Pour plus d’informations, visitez http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // Pour désactiver le suivi dans votre application, supprimez le commentaire de la ligne de code suivante ou supprimez cette dernière
            //Pour plus d’informations, consultez la page : http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();

            //var corsAttr = new EnableCorsAttribute(origins: "http://localhost:15028", headers:"*", methods: "*");
            //config.EnableCors(corsAttr);
            //Configure Caching/ETag Support
            /*
             * la mise en cache des données avec cachecow considere par défaut que les données d'un controller ne sont modifiées que par un POST/PUT/DELETE sur le même controller
             * pour pouvoir l'utiliser, il faudra donc le configurer au taquet : cf http://stackoverflow.com/questions/24819537/caching-asp-net-web-api-with-cachecow/*/
            //var connString = ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString;
            //var etagStore = new SqlServerEntityTagStore(connString);
            //var cacheHandler = new CachingHandler(GlobalConfiguration.Configuration, etagStore);
            //config.MessageHandlers.Add(cacheHandler);

#if !DEBUG
            //Force Https on entire API
            //config.Filters.Add(new RequireHttpsAttribute());
#endif
        }
    }
}
