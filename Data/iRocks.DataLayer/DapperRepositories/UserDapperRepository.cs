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
using Microsoft.AspNet.Identity;
using iRocks.DataLayer.Helpers;

namespace iRocks.DataLayer
{
    public class UserDapperRepository : DapperRepositoryBase, IUserRepository
    {
        private bool disposed = false;
        public UserDapperRepository()
            : base(ConfigurationManager.ConnectionStrings["iRocksDB"].ConnectionString)
        {
        }


        /*public IEnumerable<AppUser> GetItems(DephtLevel dephtLevel, CommandType commandType, string sql, dynamic parameters = null)
        {
            IEnumerable<AppUser> AppUsers = base.GetItems<AppUser>(commandType, sql, (DynamicParameters)parameters);
            return DependenciesAffectation(dephtLevel, AppUsers);
        }*/



        public IEnumerable<AppUser> Select(DephtLevel dephtLevel, object criteria = null, SQLKeyWord ConditionalKeyWord = null)
        {
            var postRepository = new PostDapperRepository();
            var skillRepository = new SkillDapperRepository();
            var newsfeedRepository = new NewsfeedDapperRepository();
            var notificationRepository = new NotificationDapperRepository();
            var badgeRepository = new BadgeDapperRepository();
            var badgeCollectedRepository = new BadgeCollectedDapperRepository();
            var voteRepository = new VoteDapperRepository();
            var friendshipRepository = new FriendshipDapperRepository();

            var lookup = new Dictionary<int, AppUser>();

            //var friendships = new Dictionary<int, Friendship>();
            //var postIds = new List<int>();
            //var skillIds = new List<int>();

            var NewsfeedMapping = new List<Newsfeed>();
            var NewsfeedPosts = new List<Post>();
            var NewsfeedPosters = new List<AppUser>();
            var Newsfeeds = new List<Publication>();



            var users = base.Select<AppUser, FacebookUserDetail, TwitterUserDetail, ExternalLogin, AppUser>(
                           @"SELECT * FROM AppUser LEFT OUTER JOIN FacebookUserDetail  ON AppUser.AppUserId = FacebookUserDetail.AppUserId
                                                    LEFT OUTER JOIN TwitterUserDetail  ON AppUser.AppUserId = TwitterUserDetail.AppUserId
                                                    LEFT OUTER JOIN ExternalLogin  ON AppUser.AppUserId = ExternalLogin.AppUserId ",
                         (user, facebookDetail, twitterDetail, externalLogin) =>
                         {
                             return MatchBasics(user, facebookDetail, twitterDetail, externalLogin, lookup);
                         },
                         criteria,
                         ConditionalKeyWord,
                         splitOn: "FacebookUserDetailId, TwitterUserDetailId, ExternalLoginId"
                         );

            var userIds = lookup.Values.Select(u => u.AppUserId).ToList();
            if (dephtLevel >= DephtLevel.UserFootprint)
            {
                var skills = skillRepository.Select(new { AppUserId = userIds });
                foreach (var user in lookup.Values)
                {
                    user.Footprint.AddRange(skills.Where(s => s.AppUserId == user.AppUserId));
                }
            }
            if (dephtLevel >= DephtLevel.UserProfile)
            {
                var notifications = notificationRepository.Select(new { AppUserId = userIds });
                var badgesCollected = badgeCollectedRepository.Select(new { AppUserId = userIds });
                foreach (var user in lookup.Values)
                {
                    user.Badges = badgesCollected.Where(b => b.AppUserId == user.AppUserId).OrderBy(b => b.Badge.Level).ToList();
                    user.Notifications = notifications.Where(n => n.AppUserId == user.AppUserId).ToList();
                }
            }
            if (dephtLevel >= DephtLevel.Friends)
            {
                List<AppUser> allUsers = new List<AppUser>();

                var missingUserIds = new List<int>();
                var friendsByUser = new Dictionary<int, List<int>>();

                var friendships = friendshipRepository.Select(new { AppUserAId = userIds, AppUserBId = userIds }, SQLKeyWord.Or);

                foreach (var frienship in friendships)
                {
                    if (!userIds.Contains(frienship.AppUserAId) && !missingUserIds.Contains(frienship.AppUserAId))
                        missingUserIds.Add(frienship.AppUserAId);
                    if (!userIds.Contains(frienship.AppUserBId) && !missingUserIds.Contains(frienship.AppUserBId))
                        missingUserIds.Add(frienship.AppUserBId);

                    if (friendsByUser.Keys.Contains(frienship.AppUserAId))
                        friendsByUser[frienship.AppUserAId].Add(frienship.AppUserBId);
                    else
                        friendsByUser.Add(frienship.AppUserAId, new List<int>() { frienship.AppUserBId });

                    if (friendsByUser.Keys.Contains(frienship.AppUserBId))
                        friendsByUser[frienship.AppUserBId].Add(frienship.AppUserAId);
                    else
                        friendsByUser.Add(frienship.AppUserBId, new List<int>() { frienship.AppUserAId });

                }
                allUsers.AddRange(lookup.Values.Select(x => x.DeepClone()));
                allUsers.AddRange(Select(DephtLevel.UserBasic, new { AppUserId = missingUserIds }));

                var posts = postRepository.Select(new { AppUserId = userIds });

                var votes = voteRepository.Select(new { AppUserId = userIds });



                foreach (var user in lookup.Values)
                {
                    user.Posts.AddRange(posts.Where(p => p.AppUserId == user.AppUserId));
                    user.Votes.AddRange(votes.Where(s => s.AppUserId == user.AppUserId));

                    if (friendsByUser.ContainsKey(user.AppUserId))
                        user.Friends.AddRange(allUsers.Where(u => friendsByUser[user.AppUserId].Contains(u.AppUserId)));


                }
            }

            if (dephtLevel >= DephtLevel.NewsFeed)
            {
                NewsfeedMapping = newsfeedRepository.Select(new { AppUserId = userIds.ToList() }).ToList();
                NewsfeedPosts = postRepository.Select(new { PostId = NewsfeedMapping.Select(n => n.PostId).ToList() }).ToList();
                NewsfeedPosters = Select(DephtLevel.UserProfile, new { AppUserId = NewsfeedPosts.Select(p => p.AppUserId).ToList() }).ToList();
                NewsfeedPosts.ForEach(
                    p => Newsfeeds.Add(
                        new Publication(
                            p,
                            NewsfeedPosters.Where(user => user.AppUserId == p.AppUserId).First()
                            )));


                foreach (var user in lookup.Values)
                {
                    var newsfeedPostIds = NewsfeedMapping.Where(n => n.AppUserId == user.AppUserId).Select(n => n.PostId);
                    user.Newsfeed.AddRange(
                                            Newsfeeds.Where(p => newsfeedPostIds.Contains(p.Post.PostId))
                                          );
                }
            }
            return lookup.Values;
        }

        public void Save(DephtLevel dephtLevel, AppUser user)
        {
            List<AppUser> users = new List<AppUser>();
            users.Add(user);
            Save(dephtLevel, users);

        }



        // /!\DELETE  Operations have been disabled since the CleanDbUser is not implemented in FacebookDataFeed.

        public void Save(DephtLevel dephtLevel, List<AppUser> users)
        {

            ISkillRepository SkillRepository = new SkillDapperRepository();
            IVoteRepository VoteRepository = new VoteDapperRepository();
            IPostRepository PostRepository = new PostDapperRepository();
            IFriendshipRepository FriendshipRepository = new FriendshipDapperRepository();
            IFacebookUserDetailRepository FacebookUserDetailRepository = new FacebookUserDetailDapperRepository();
            ITwitterUserDetailRepository TwitterUserDetailRepository = new TwitterUserDetailDapperRepository();
            IExternalLoginRepository ExternalLoginRepository = new ExternalLoginDapperRepository();
            INewsfeedRepository NewsfeedRepository = new NewsfeedDapperRepository();
            INotificationRepository notificationRepository = new NotificationDapperRepository();
            IBadgeCollectedRepository badgeCollectedRepository = new BadgeCollectedDapperRepository();

            var alreadySavedUsers = new List<AppUser>();

            var WholeFriendships = FriendshipRepository.Select(new { AppUserAId = users.Select(c => c.AppUserId).Distinct().ToList(), AppUserBId = users.Select(c => c.AppUserId).Distinct().ToList() }, SQLKeyWord.Or);

            IEnumerable<Newsfeed> WholeNewsfeedMatching = new List<Newsfeed>();
            if (dephtLevel == DephtLevel.NewsFeed)
            {
                WholeNewsfeedMatching = NewsfeedRepository.Select(new { AppUserId = users.Select(c => c.AppUserId).ToList() });
            }





            foreach (var user in users)
            {
                using (var transaction = new TransactionScope())
                {
                    //Should not create new Skill to existing user !
                    if (!user.IsNew && user.Footprint.Where(s => s.IsNew).Any())
                    {
                        var dbFootprint = SkillRepository.Select(new { AppUserId = user.AppUserId });
                        foreach (var skill in user.Footprint.Where(s => s.IsNew))
                        {
                            var existingSkill = dbFootprint.Where(s => s.CategoryId == skill.CategoryId).SingleOrDefault();
                            if (existingSkill != null)
                                skill.SkillId = existingSkill.SkillId;
                        }
                    }
                    if (user.IsNew)
                        base.Insert<AppUser>(user);
                    else
                        base.Update<AppUser>(user);
                    alreadySavedUsers.Add(user);
                    if (user.IsProvidedBy(Provider.Facebook))
                    {
                        if (user.FacebookDetail.IsNew)
                        {
                            user.FacebookDetail.AppUserId = user.AppUserId;
                            FacebookUserDetailRepository.Insert(user.FacebookDetail);
                        }
                        //else if (user.FacebookDetail.IsDeleted)
                        //    FacebookUserDetailRepository.Delete(user.FacebookDetail);

                        else
                            FacebookUserDetailRepository.Update(user.FacebookDetail);
                    }
                    if (user.IsProvidedBy(Provider.Twitter))
                    {
                        if (user.TwitterDetail.IsNew)
                        {
                            user.TwitterDetail.AppUserId = user.AppUserId;
                            TwitterUserDetailRepository.Insert(user.TwitterDetail);
                        }
                        else
                            TwitterUserDetailRepository.Update(user.TwitterDetail);
                    }

                    foreach (var externalLogin in user.ExternalLogins)
                    {
                        if (externalLogin.IsNew)
                        {
                            externalLogin.AppUserId = user.AppUserId;
                            ExternalLoginRepository.Insert(externalLogin);
                        }
                        //else if (externalLogin.IsDeleted)
                        //    ExternalLoginRepository.Delete(externalLogin);

                        else
                            ExternalLoginRepository.Update(externalLogin);
                    }
                    if (dephtLevel >= DephtLevel.UserFootprint)
                    {
                        foreach (var skill in user.Footprint)
                        {
                            if (skill.IsNew)
                            {
                                skill.AppUserId = user.AppUserId;
                                SkillRepository.Insert(skill);
                            }
                            //else if (skill.IsDeleted)
                            //    SkillRepository.Delete(skill);

                            else
                                SkillRepository.Update(skill);
                        }
                    }
                    if (dephtLevel >= DephtLevel.UserProfile)
                    {
                        foreach (var notification in user.Notifications)
                        {
                            if (notification.IsNew)
                            {
                                notification.AppUserId = user.AppUserId;
                                notificationRepository.Insert(notification);
                            }
                            //else if (vote.IsDeleted)
                            //    VoteRepository.Delete(vote);

                            else
                                notificationRepository.Update(notification);
                        }

                        foreach (var badge in user.Badges)
                        {
                            if (badge.IsNew)
                            {
                                badge.AppUserId = user.AppUserId;
                                badgeCollectedRepository.Insert(badge);
                            }
                            //else if (vote.IsDeleted)
                            //    VoteRepository.Delete(vote);

                            else
                                badgeCollectedRepository.Update(badge);
                        }
                    }
                    if (dephtLevel >= DephtLevel.Friends)
                    {
                        foreach (var post in user.Posts)
                        {
                            if (post.IsNew)
                            {
                                post.AppUserId = user.AppUserId;
                                PostRepository.Insert(post);
                            }
                            else
                                PostRepository.Update(post);
                            alreadySavedUsers.AddRange(ImbricatedObjectFinder.GetImbricatedUsers(post));
                        }




                        foreach (var vote in user.Votes)
                        {
                            if (vote.IsNew)
                            {
                                vote.AppUserId = user.AppUserId;
                                VoteRepository.Insert(vote);
                            }
                            //else if (vote.IsDeleted)
                            //    VoteRepository.Delete(vote);

                            else
                                VoteRepository.Update(vote);
                        }




                        var friendships = WholeFriendships.Where(f => f.AppUserAId == user.AppUserId || f.AppUserBId == user.AppUserId);

                        foreach (var friend in user.Friends)
                        {
                            if (friend.IsNew)
                                base.Insert<AppUser>(friend);
                            else
                                Save(DephtLevel.UserBasic, friend);
                            alreadySavedUsers.Add(friend);

                            if (!friendships.Where(f => f.AppUserAId == friend.AppUserId || f.AppUserBId == friend.AppUserId).Any())
                            {
                                FriendshipRepository.Insert(new Friendship() { AppUserAId = user.AppUserId, AppUserBId = friend.AppUserId });

                            }
                        }

                        foreach (var friendship in friendships)
                        {
                            if (!user.Friends.Where(f => f.AppUserId == friendship.AppUserAId || f.AppUserId == friendship.AppUserBId).Any())
                            {
                                //FriendshipRepository.Delete(friendship);
                            }

                        }


                    }
                    transaction.Complete();
                }
            }


            foreach (var user in users)//On rentre le newsfeed en dernier afin de s'assurer que l'ensemble des AppUsers ait été entré auparavant et ne pas faire de doublons
            {
                using (var transaction = new TransactionScope())
                {
                    if (dephtLevel == DephtLevel.NewsFeed)
                    {

                        var implicatedUsers = ImbricatedObjectFinder.GetImbricatedUsers(user); // get all users to be saved for newsfeed

                        //parse them
                        var usersToBeSaved = ImbricatedObjectFinder.GetUsersToBeSaved(implicatedUsers, alreadySavedUsers);


                        var newsfeedMatchings = WholeNewsfeedMatching.Where(f => f.AppUserId == user.AppUserId);
                        Save(DephtLevel.UserProfile, usersToBeSaved);

                        foreach (var pair in user.Newsfeed)
                        {
                            var post = pair.Post;
                            if (post.IsNew)
                            {
                                post.AppUserId = pair.User.AppUserId;
                                if (pair.User.AppUserId == 0)
                                {
                                    if (pair.User.IsProvidedBy(Provider.Facebook))
                                    {
                                        post.AppUserId = user.Newsfeed.Select(p => p.User).Where(u => u.FacebookDetail.FacebookUserId == pair.User.FacebookDetail.FacebookUserId && u.AppUserId != 0).First().AppUserId;
                                    }
                                    if (pair.User.IsProvidedBy(Provider.Twitter))
                                    {
                                        post.AppUserId = user.Newsfeed.Select(p => p.User).Where(u => u.TwitterDetail.TwitterUserId == pair.User.TwitterDetail.TwitterUserId && u.AppUserId != 0).First().AppUserId;
                                    }
                                }
                                PostRepository.Insert(post);
                            }

                            else
                                PostRepository.Update(post);

                            if (!newsfeedMatchings.Where(f => f.PostId == post.PostId).Any())
                            {
                                NewsfeedRepository.Insert(new Newsfeed() { AppUserId = user.AppUserId, PostId = post.PostId });

                            }
                        }



                    }
                    transaction.Complete();
                }
            }

        }

       
        private AppUser MatchBasics(AppUser user, FacebookUserDetail facebookDetail, TwitterUserDetail twitterDetail, ExternalLogin externalLogin, Dictionary<int, AppUser> lookup)
        {
            AppUser aUser;
            if (!lookup.TryGetValue(user.AppUserId, out aUser))
            {
                lookup.Add(user.AppUserId, aUser = user);
                if (facebookDetail != null)
                {
                    facebookDetail.SetSnapshot(facebookDetail);
                    aUser.FacebookDetail = facebookDetail;
                }

                if (twitterDetail != null)
                {
                    twitterDetail.SetSnapshot(twitterDetail);
                    aUser.TwitterDetail = twitterDetail;
                }
            }
            if (externalLogin != null && !aUser.ExternalLogins.Where(el => el.ExternalLoginId == externalLogin.ExternalLoginId).Any())
            {
                externalLogin.SetSnapshot(externalLogin);
                aUser.ExternalLogins.Add(externalLogin);
            }
            return aUser;

        }

        public void Delete(AppUser obj)
        {
            using (var transaction = new TransactionScope())
            {
                ISkillRepository SkillRepository = new SkillDapperRepository();
                IVoteRepository VoteRepository = new VoteDapperRepository();
                IPostRepository PostRepository = new PostDapperRepository();
                IFriendshipRepository FriendshipRepository = new FriendshipDapperRepository();
                IFacebookUserDetailRepository FacebookUserDetailRepository = new FacebookUserDetailDapperRepository();
                ITwitterUserDetailRepository TwitterUserDetailRepository = new TwitterUserDetailDapperRepository();
                IExternalLoginRepository ExternalLoginRepository = new ExternalLoginDapperRepository();

                if (obj.FacebookDetail != null)
                    FacebookUserDetailRepository.Delete(obj.FacebookDetail);

                if (obj.TwitterDetail != null)
                    TwitterUserDetailRepository.Delete(obj.TwitterDetail);

                foreach (Post aPost in obj.Posts)
                {
                    PostRepository.Delete(aPost);
                }
                foreach (Vote aVote in obj.Votes)
                {
                    VoteRepository.Delete(aVote);
                }
                foreach (ExternalLogin anExternalLogin in obj.ExternalLogins)
                {
                    ExternalLoginRepository.Delete(anExternalLogin);
                }

                foreach (Skill aSkill in obj.Footprint)
                {
                    SkillRepository.Delete(aSkill);
                }

                var Parameters = new DynamicParameters();
                Parameters.Add("@AppUserId", obj.AppUserId);

                base.Execute(CommandType.Text, "DELETE FROM Friendship WHERE AppUserAId = @AppUserId OR AppUserBId = @AppUserId");

                base.Delete<AppUser>(obj);
                transaction.Complete();
            }
        }



        #region IUserStore Implementation
        public Task CreateAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                var existingUser = Select(DephtLevel.Friends, new { UserName = user.UserName }).FirstOrDefault();
                if (existingUser != null)
                {
                    user.ExternalLogins.ForEach(c => c.AppUserId = existingUser.AppUserId);
                    existingUser.ExternalLogins.AddRange(user.ExternalLogins);
                    user = existingUser;
                }
                else
                {
                    if (user.IsProvidedBy(Provider.Facebook))
                    {
                        FacebookUserDetailDapperRepository FacebookUserDetailRepository = new FacebookUserDetailDapperRepository();
                        var FacebookDetails = FacebookUserDetailRepository.Select(new { FacebookUserId = user.FacebookDetail.FacebookUserId });
                        if (FacebookDetails.Any())
                        {
                            var tempUser = Select(DephtLevel.Friends, new { AppUserId = FacebookDetails.SingleOrDefault().AppUserId }).SingleOrDefault();
                            tempUser.ExternalLogins.AddRange(user.ExternalLogins);
                            if (tempUser.FacebookDetail != null)
                            {
                                user.FacebookDetail.FacebookUserDetailId = tempUser.FacebookDetail.FacebookUserDetailId;
                                user.FacebookDetail.AppUserId = tempUser.FacebookDetail.AppUserId;
                                tempUser.FacebookDetail = user.FacebookDetail;
                            }
                            else
                            {
                                tempUser.FacebookDetail = user.FacebookDetail;
                                tempUser.FacebookDetail.AppUserId = user.AppUserId;
                            }
                            tempUser.UserName = user.UserName;
                            tempUser.ReturnUrl = user.ReturnUrl;
                            tempUser.Activated = user.Activated;
                            user = tempUser;
                            //user.AppUserId = tempUser.AppUserId;
                        }
                    }
                    if (user.IsProvidedBy(Provider.Twitter))
                    {
                        TwitterUserDetailDapperRepository TwitterUserDetailRepository = new TwitterUserDetailDapperRepository();
                        var TwitterDetails = TwitterUserDetailRepository.Select(new { TwitterUserId = user.TwitterDetail.TwitterUserId });
                        if (TwitterDetails.Any())
                        {
                            var tempUser = Select(DephtLevel.Friends, new { AppUserId = TwitterDetails.SingleOrDefault().AppUserId }).SingleOrDefault();
                            tempUser.ExternalLogins.AddRange(user.ExternalLogins);
                            if (tempUser.TwitterDetail != null)
                            {
                                user.TwitterDetail.TwitterUserDetailId = tempUser.TwitterDetail.TwitterUserDetailId;
                                user.TwitterDetail.AppUserId = tempUser.TwitterDetail.AppUserId;
                                tempUser.TwitterDetail = user.TwitterDetail;
                            }
                            else
                            {
                                tempUser.TwitterDetail = user.TwitterDetail;
                                tempUser.TwitterDetail.AppUserId = user.AppUserId;
                            }
                            tempUser.UserName = user.UserName;
                            tempUser.ReturnUrl = user.ReturnUrl;
                            tempUser.Activated = user.Activated;
                            user = tempUser;
                            //user.AppUserId = tempUser.AppUserId;
                        }
                    }

                }

                Save(DephtLevel.Friends, user);
            });
        }

        public Task DeleteAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                Delete(user);
            });
        }

        public Task<AppUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException("userId");

            return Task.Factory.StartNew(() =>
            {
                return Select(DephtLevel.UserBasic, new { AppUserId = userId }).SingleOrDefault();
            });
        }

        public Task<AppUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("userName");

            return Task.Factory.StartNew(() =>
            {

                return Select(DephtLevel.UserBasic, new { UserName = userName }).SingleOrDefault();
            });
        }

        public Task UpdateAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                Save(DephtLevel.Friends, user);
            });
        }


        #endregion
        #region IUserLoginStore Implementation
        public Task AddLoginAsync(AppUser user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");
            return Task.Factory.StartNew(() =>
            {
                ExternalLoginDapperRepository ExternalLoginRepository = new ExternalLoginDapperRepository();
                var loginInfo = new ExternalLogin
                {
                    AppUserId = user.AppUserId,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey
                };

                ExternalLoginRepository.Insert(loginInfo);

                if (user.IsProvidedBy(Provider.Facebook))
                {
                    IFacebookUserDetailRepository FacebookDetailRepository = new FacebookUserDetailDapperRepository();

                    FacebookDetailRepository.Insert(user.FacebookDetail);
                }
                if (user.IsProvidedBy(Provider.Twitter))
                {
                    ITwitterUserDetailRepository TwitterDetailRepository = new TwitterUserDetailDapperRepository();

                    TwitterDetailRepository.Insert(user.TwitterDetail);
                }
            });
        }

        public Task<AppUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                ExternalLoginDapperRepository ExternalLoginRepository = new ExternalLoginDapperRepository();
                var loginInfos = ExternalLoginRepository.Select(new { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey });
                if (loginInfos.Any())
                    return Select(DephtLevel.UserBasic, new { AppUserId = loginInfos.SingleOrDefault().AppUserId }).SingleOrDefault();
                return null;


            });
        }


        public Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                ExternalLoginDapperRepository ExternalLoginRepository = new ExternalLoginDapperRepository();
                return (IList<UserLoginInfo>)ExternalLoginRepository.Select(new { AppUserId = user.AppUserId }).Select(p => new UserLoginInfo(p.LoginProvider, p.ProviderKey));
            });
        }

        public Task RemoveLoginAsync(AppUser user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                ExternalLoginDapperRepository ExternalLoginRepository = new ExternalLoginDapperRepository();
                ExternalLoginRepository.Delete(ExternalLoginRepository.Select(new { AppUserId = user.AppUserId, LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey }).SingleOrDefault());
            });
        }
        #endregion
        #region  IUserPasswordStore
        public Task<string> GetPasswordHashAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(AppUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }
        #endregion
        #region IUserSecurityStampStore

        public Task<string> GetSecurityStampAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(AppUser user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }
        #endregion

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;

        }
        ~UserDapperRepository()
        {
            Dispose(false);
        }
        #region IUserEmailStore implementation

        public Task<AppUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("email");

            return Task.Factory.StartNew(() =>
            {
                return Select(DephtLevel.UserBasic, new { EmailAddress = email }).SingleOrDefault();
            });
        }

        public Task<string> GetEmailAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.EmailAddress);
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            //not implemented yet
            return Task.FromResult(true);
        }

        public Task SetEmailAsync(AppUser user, string email)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.EmailAddress = email;

            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed)
        {
            //not implemented yet
            return Task.FromResult(0);
        }
        #endregion
        #region IUserLockoutStore implementation
        public Task<int> GetAccessFailedCountAsync(AppUser user)
        {
            //not implemented yet
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(AppUser user)
        {
            //not implemented yet
            return Task.FromResult(false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(AppUser user)
        {
            //not implemented yet
            return Task.FromResult(DateTimeOffset.Now);
        }

        public Task<int> IncrementAccessFailedCountAsync(AppUser user)
        {
            //not implemented yet
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(AppUser user)
        {
            //not implemented yet
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(AppUser user, bool enabled)
        {
            //not implemented yet
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset lockoutEnd)
        {
            //not implemented yet
            return Task.FromResult(0);
        }
        #endregion



        #region IUserTwoFactorStore implementation
        public Task<bool> GetTwoFactorEnabledAsync(AppUser user)
        {
            //not implemented yet
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled)
        {
            //not implemented yet
            return Task.FromResult(0);
        }
        #endregion
    }
}
