using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iRocks.AI;
using iRocks.DataLayer;
using iRocks.WebAPI.Services;
using iRocks.WebAPI.Filters;
using iRocks.AI.Helpers.Loging;

namespace iRocks.WebAPI.Controllers
{
    public class NotificationController : BaseApiController
    {

        private INotificationRepository _notificationRepository;

        public NotificationController(IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository, INotificationRepository notificationRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
            _notificationRepository = notificationRepository;
        }
        
        [HttpPatch]
        [HttpPut]
        [iRocksAuthorizeAttribute(true)]
        [LoggingAspect]
        public HttpResponseMessage Patch([FromBody]IEnumerable<DataLayer.Notification> Notifications)
        {
            try
            {
                if (Notifications == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                var currentUser = TheUserRepository.Select(DephtLevel.NewsFeed, new { UserName = User.Identity.Name }).SingleOrDefault();

                if (currentUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                var dbNotifications = _notificationRepository.Select(new { NotificationId = Notifications.Select(n => n.NotificationId).ToList() });
                foreach(var notification in Notifications)
                {
                    var dbNotification = dbNotifications.Where(n => n.NotificationId == notification.NotificationId).FirstOrDefault();
                    if (dbNotification != null)
                    {
                        if (notification.AppUserId == currentUser.AppUserId)
                        {
                            notification.SetSnapshot(dbNotification);
                            _notificationRepository.Update(notification);
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
                 
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
