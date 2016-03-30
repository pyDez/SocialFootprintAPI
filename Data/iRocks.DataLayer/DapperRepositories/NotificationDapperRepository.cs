using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

namespace iRocks.DataLayer
{
    public class NotificationDapperRepository : DapperRepositoryBase, INotificationRepository
    {

        public NotificationDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {

        }

        public IEnumerable<Notification> Select(object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            var postRepository = new PostDapperRepository();
            var userRepository = new UserDapperRepository();

            var Notifications = base.Select<Notification>(criteria, ConditionalKeyWord);


            //var postIds = Notifications.Where(n => n.ObjectType == NotificationObject.Post.ToString()).Select(n => n.ObjectId).ToList();
            //var posts = postRepository.Select(new { PostId = postIds });

            //var userIds = Notifications.Where(n => n.ObjectType == NotificationObject.AppUser.ToString()).Select(n => n.ObjectId).Concat(posts.Select(p=>p.AppUserId)).ToList();
            //var users = userRepository.Select(DephtLevel.UserBasic, new { AppUserId = userIds });

            //foreach(var notification in Notifications)
            //{
            //    if (notification.ObjectType == NotificationObject.Post.ToString())
            //    {
            //         var post = posts.Where(p => p.PostId == notification.ObjectId).Single();
            //        var user = users.Where(u => u.AppUserId == post.AppUserId).Single();
            //        notification.ObjectEntity = new Publication(post, user);
            //    }

            //    if (notification.ObjectType == NotificationObject.AppUser.ToString())
            //        notification.ObjectEntity = users.Where(u => u.AppUserId == notification.ObjectId).Single();
            //}

            return Notifications;
        }

        public void Insert(Notification obj)
        {
            base.Insert<Notification>(obj);
        }

        public void Update(Notification obj)
        {
            base.Update<Notification>(obj);
        }

        public void Delete(Notification obj)
        {
            base.Delete<Notification>(obj);
        }
    }
}
