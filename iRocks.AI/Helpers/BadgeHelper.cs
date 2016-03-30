using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iRocks.AI
{
    public static class BadgeHelper
    {
        public static Tuple<BadgeCollected, Notification> AddCurrentUserBadge(AppUser currentUser, Vote newVote, List<Badge> badges, IBadgeCollectedRepository badgeRepository, INotificationRepository notificationRepository)
        {
            Badge badge = null;
            if (newVote.AppUserId == currentUser.AppUserId)
            {
                if (currentUser.Votes.Count == 10)
                {
                    badge = badges.Where(b => b.BadgeId == 21).FirstOrDefault();
                }
                if (currentUser.Votes.Count == 50)
                {
                    badge = badges.Where(b => b.BadgeId == 22).FirstOrDefault();
                }
                if (currentUser.Votes.Count == 500)
                {
                    badge = badges.Where(b => b.BadgeId == 23).FirstOrDefault();
                }
                if (currentUser.Votes.Count == 5000)
                {
                    badge = badges.Where(b => b.BadgeId == 24).FirstOrDefault();
                }
            }
            if (badge != null)
            {
                if (!currentUser.Badges.Where(b => b.BadgeId == badge.BadgeId).Any())
                {
                    var collected = new BadgeCollected()
                    {
                        AppUserId = currentUser.AppUserId,
                        BadgeId = badge.BadgeId,
                        CollectDate = DateTime.Now,
                        Badge = badge
                    };
                    badgeRepository.Insert(collected);
                    var notification = new Notification()
                    {
                        AppUserId = currentUser.AppUserId,
                        IsRed = false,
                        ObjectType = NotificationObject.Badge.ToString(),
                        ObjectId = badge.BadgeId,
                        Information = TranslationHelper.GetTranslation(currentUser.Locale, "NEW_BADGE_NOTIFICATION_1") + badge.Label+ TranslationHelper.GetTranslation(currentUser.Locale, "NEW_BADGE_NOTIFICATION_2"),
                        NotificationDate = DateTime.Now
                    };
                    notificationRepository.Insert(notification);
                    
                    return new Tuple<BadgeCollected, Notification>(collected, notification);
                }


            }
            return null;
        }
        public static async Task AddCategoryBadge(AppUser user, List<Badge> badges, List<Category> categories, IBadgeCollectedRepository badgeRepository, INotificationRepository notificationRepository)
        {
            await Task.Run(() =>
            {
                Badge badge = null;
                Dictionary<int, BadgeIds> badgeIdByCategory = new Dictionary<int, BadgeIds>();
                badgeIdByCategory.Add(1, new BadgeIds() { Good = 13, Best = 14 });//Général
                badgeIdByCategory.Add(2, new BadgeIds() { Good = 3, Best = 4 });//RH
                badgeIdByCategory.Add(3, new BadgeIds() { Good = 5, Best = 6 });//Societé
                badgeIdByCategory.Add(4, new BadgeIds() { Good = 11, Best = 12 });//Art
                badgeIdByCategory.Add(5, new BadgeIds() { Good = 9, Best = 10 });//Sport
                badgeIdByCategory.Add(6, new BadgeIds() { Good = 1, Best = 2 });//Humoour
                badgeIdByCategory.Add(7, new BadgeIds() { Good = 7, Best = 8 });//Santé
                badgeIdByCategory.Add(8, new BadgeIds() { Good = 15, Best = 16 });//Business


                foreach (Category category in categories)
                {
                    if (badgeIdByCategory.ContainsKey(category.CategoryId))
                    {
                        var badgeIds = badgeIdByCategory[category.CategoryId];
                        if (user.GetSkillLevel(category.CategoryId) >= 75)
                        {
                            if (!user.Badges.Where(b => b.BadgeId == badgeIds.Good).Any())
                            {
                                badge = badges.Where(b => b.BadgeId == badgeIds.Good).FirstOrDefault();
                            }
                        }
                        if (user.GetSkillLevel(category.CategoryId) >= 90)
                        {
                            if (!user.Badges.Where(b => b.BadgeId == badgeIds.Best).Any())
                            {
                                badge = badges.Where(b => b.BadgeId == badgeIds.Best).FirstOrDefault();
                            }
                        }

                    }

                }

                if (badge != null)
                {
                    var collected = new BadgeCollected()
                    {
                        AppUserId = user.AppUserId,
                        BadgeId = badge.BadgeId,
                        CollectDate = DateTime.Today
                    };
                    badgeRepository.Insert(collected);

                    var notification = new Notification()
                    {
                        AppUserId = user.AppUserId,
                        IsRed = false,
                        ObjectType = NotificationObject.Badge.ToString(),
                        ObjectId = badge.BadgeId,
                        Information = TranslationHelper.GetTranslation(user.Locale, "NEW_BADGE_NOTIFICATION_1") + badge.Label + TranslationHelper.GetTranslation(user.Locale, "NEW_BADGE_NOTIFICATION_2"),
                        NotificationDate = DateTime.Now
                    };
                    notificationRepository.Insert(notification);
                }

            });
        }
        public static async Task AddPostBadge(AppUser user, Post post, List<Badge> badges, IBadgeCollectedRepository badgeRepository, INotificationRepository notificationRepository)
        {
            if (user.AppUserId != post.AppUserId)
                throw new ArgumentException("the post should be from the user");

            await Task.Run(() =>
            {
                Badge badge = null;

                if (post.UpVotes.Count() > 10)
                {
                    badge = badges.Where(b => b.BadgeId == 17).FirstOrDefault();
                }
                if (post.UpVotes.Count() > 30)
                {
                    badge = badges.Where(b => b.BadgeId == 18).FirstOrDefault();
                }
                if (post.UpVotes.Count() > 100)
                {
                    badge = badges.Where(b => b.BadgeId == 19).FirstOrDefault();
                }
                if (post.UpVotes.Count() > 1000)
                {
                    badge = badges.Where(b => b.BadgeId == 20).FirstOrDefault();
                }


                if (badge != null)
                {
                    if (!user.Badges.Where(b => b.BadgeId == badge.BadgeId).Any())
                    {
                        var collected = new BadgeCollected()
                        {
                            AppUserId = post.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today,
                            PostId = post.PostId
                        };
                        badgeRepository.Insert(collected);

                        var notification = new Notification()
                        {
                            AppUserId = post.AppUserId,
                            IsRed = false,
                            ObjectType = NotificationObject.Badge.ToString(),
                            ObjectId = badge.BadgeId,
                            Information = TranslationHelper.GetTranslation(user.Locale, "NEW_BADGE_NOTIFICATION_1") + badge.Label + TranslationHelper.GetTranslation(user.Locale, "NEW_BADGE_NOTIFICATION_2"),
                            NotificationDate = DateTime.Now
                        };
                        notificationRepository.Insert(notification);

                    }
                }

            });
        }

    }

    public class BadgeIds
    {
        //public int Worst { get; set; }
        //public int Bad { get; set; }
        public int Good { get; set; }
        public int Best { get; set; }
    }



}
