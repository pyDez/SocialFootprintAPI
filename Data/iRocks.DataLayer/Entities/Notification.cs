
using System;
namespace iRocks.DataLayer
{
    public class Notification : DapperManagedObject<Notification>, IGetId, IDeeplyCloneable<Notification>
    {
        public Notification()
        {
            this.NotificationDate = DateTime.Now;
        }
        public int NotificationId { get; set; }
        public int AppUserId { get; set; }
        public string Information { get; set; }
        public bool IsRed { get; set; }
        public DateTime NotificationDate { get; set; }
        private string objectType;

        public string ObjectType
        {
            get { return objectType; }
            set
            {
                var ok = false;
                foreach (var type in Enum.GetNames(typeof(NotificationObject)))
                {
                    if (value == type.ToString())
                        ok = true;
                }
                if (ok)
                    objectType = value;
                else
                    throw new NotSupportedException();
            }
        }
        public int ObjectId { get; set; }

        // [DapperIgnore]
        //public object ObjectEntity { get; set; }
       
        public bool IsNew
        {
            get
            {
                return this.NotificationId == default(int);
            }
        }
        public int GetId()
        {
            return NotificationId;
        }

        public Notification DeepClone()
        {
            var res = new Notification()
            {
                NotificationId = this.NotificationId,
                AppUserId = this.AppUserId,
                Information = this.Information,
                IsRed = this.IsRed,
                ObjectType = this.ObjectType,
                ObjectId = this.ObjectId,
                //ObjectEntity = this.ObjectEntity,
                NotificationDate = this.NotificationDate,
                Snapshot = this.Snapshot
            };
            return res;
        }
    }
    public enum NotificationObject
    {
        Post,
        AppUser,
        Badge
    }
}
