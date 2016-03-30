namespace iRocks.DataLayer
{
    public class PostMentionedUser : DapperManagedObject<PostMentionedUser>, IGetId, IDeeplyCloneable<PostMentionedUser>
    {
        public PostMentionedUser()
        {
        }
        public int PostMentionedUserId { get; set; }
        public int AppUserId { get; set; }
        public int PostId { get; set; }
        public string Provider { get; set; }
        public bool IsNew
        {
            get
            {
                return this.PostMentionedUserId == default(int);
            }
        }

        [DapperIgnore]
        public bool IsDeleted { get; set; }

        public PostMentionedUser DeepClone()
        {
            var res = new PostMentionedUser()
            {
                PostMentionedUserId = this.PostMentionedUserId,
                AppUserId = this.AppUserId,
                PostId = this.PostId,
                Provider = this.Provider
            };
            res.Snapshot = this.Snapshot;
            return res;
        }
        public int GetId()
        {
            return PostMentionedUserId;
        }
    }
}
