using System;
using System.Collections.Generic;
using System.Linq;

namespace iRocks.DataLayer.Helpers
{
    static public class ImbricatedObjectFinder
    {

        static public List<AppUser> GetImbricatedUsers(AppUser user)
        {
            var allUsers = GetUsersFromUserRecursive(user);

            return GetDistinctUsers(allUsers);
        }
        static public List<AppUser> GetImbricatedUsers(Post post)
        {
            var allUsers = GetUsersFromPostRecursive(post);
            return GetDistinctUsers(allUsers);

        }
        static public List<Post> GetImbricatedPosts(AppUser user)
        {
            var allPosts = GetPostsFromUserRecursive(user);
            return GetDistinctPosts(allPosts);

        }
        static public List<AppUser> GetUsersToBeSaved(IEnumerable<AppUser> implicatedUsers, IEnumerable<AppUser> alreadySavedUsers)
        {
            var usersToBeSaved = new List<AppUser>();
            var alreadySavedFbUsers = alreadySavedUsers.Where(u => u.IsProvidedBy(Provider.Facebook));
            var alreadySavedTwitterUsers = alreadySavedUsers.Where(u => u.IsProvidedBy(Provider.Twitter));
            foreach (var user in implicatedUsers)
            {
                if (user.IsProvidedBy(Provider.Facebook))
                {
                    if (!alreadySavedFbUsers.Where(u => u.FacebookDetail.FacebookUserId == user.FacebookDetail.FacebookUserId).Any())
                        usersToBeSaved.Add(user);
                }
                if (user.IsProvidedBy(Provider.Twitter))
                {
                    if (!alreadySavedTwitterUsers.Where(u => u.TwitterDetail.TwitterUserId == user.TwitterDetail.TwitterUserId).Any())
                        usersToBeSaved.Add(user);
                }
            }
            return usersToBeSaved;
        }
        static private List<Tuple<Provider, string, AppUser>> GetUsersFromPostRecursive(Post post)
        {
            var allUsers = new List<Tuple<Provider, string, AppUser>>();
            if (post.IsProvidedBy(Provider.Facebook))
            {
                if (post.FacebookDetail.ChildPublication != null)
                {
                    allUsers.AddRange(GetUsersFromPostRecursive(post.FacebookDetail.ChildPublication.Post));
                    allUsers.AddRange(GetUsersFromUserRecursive(post.FacebookDetail.ChildPublication.User));
                }
            }
            if (post.IsProvidedBy(Provider.Twitter))
            {
                foreach (var user in post.TwitterDetail.MentionedUsers)
                {
                    allUsers.AddRange(GetUsersFromUserRecursive(user));
                }
                if (post.TwitterDetail.RetweetedPublication != null)
                {
                    allUsers.AddRange(GetUsersFromPostRecursive(post.TwitterDetail.RetweetedPublication.Post));
                    allUsers.AddRange(GetUsersFromUserRecursive(post.TwitterDetail.RetweetedPublication.User));
                }
            }


            return allUsers;
        }
        static private List<Tuple<Provider, string, AppUser>> GetUsersFromUserRecursive(AppUser user)
        {
            var allUsers = new List<Tuple<Provider, string, AppUser>>();
            if (user.IsProvidedBy(Provider.Facebook))
            {
                allUsers.Add(new Tuple<Provider, string, AppUser>(Provider.Facebook, user.FacebookDetail.FacebookUserId, user));
            }
            if (user.IsProvidedBy(Provider.Twitter))
            {
                allUsers.Add(new Tuple<Provider, string, AppUser>(Provider.Twitter, user.TwitterDetail.TwitterUserId, user));
            }
            foreach(var friend in user.Friends)
            {
                allUsers.AddRange(GetUsersFromUserRecursive(friend));
            }
            foreach (var post in user.Posts)
            {
                allUsers.AddRange(GetUsersFromPostRecursive(post));
            }
            foreach (var publication in user.Newsfeed)
            {
                allUsers.AddRange(GetUsersFromPostRecursive(publication.Post));
                allUsers.AddRange(GetUsersFromUserRecursive(publication.User));
            }

            return allUsers;
        }

        static private List<Tuple<Provider, string, Post>> GetPostsFromPostRecursive(Post post)
        {
            var allPosts = new List<Tuple<Provider, string, Post>>();
            if (post.IsProvidedBy(Provider.Facebook))
            {
                allPosts.Add(new Tuple<Provider, string, Post>(Provider.Facebook, post.FacebookDetail.FacebookPostId, post));
                if (post.FacebookDetail.ChildPublication != null)
                {
                    allPosts.AddRange(GetPostsFromPostRecursive(post.FacebookDetail.ChildPublication.Post));
                    allPosts.AddRange(GetPostsFromUserRecursive(post.FacebookDetail.ChildPublication.User));
                }
            }
            if (post.IsProvidedBy(Provider.Twitter))
            {
                allPosts.Add(new Tuple<Provider, string, Post>(Provider.Twitter, post.TwitterDetail.TwitterPostId, post));
                foreach (var user in post.TwitterDetail.MentionedUsers)
                {
                    allPosts.AddRange(GetPostsFromUserRecursive(user));
                }
                if (post.TwitterDetail.RetweetedPublication != null)
                {
                    allPosts.AddRange(GetPostsFromPostRecursive(post.TwitterDetail.RetweetedPublication.Post));
                    allPosts.AddRange(GetPostsFromUserRecursive(post.TwitterDetail.RetweetedPublication.User));
                }
            }


            return allPosts;
        }

        static private List<Tuple<Provider, string, Post>> GetPostsFromUserRecursive(AppUser user)
        {
            var allPosts = new List<Tuple<Provider, string, Post>>();
            foreach (var friend in user.Friends)
            {
                allPosts.AddRange(GetPostsFromUserRecursive(friend));
            }
            foreach (var post in user.Posts)
            {
                allPosts.AddRange(GetPostsFromPostRecursive(post));
            }
            foreach (var publication in user.Newsfeed)
            {
                allPosts.AddRange(GetPostsFromPostRecursive(publication.Post));
                allPosts.AddRange(GetPostsFromUserRecursive(publication.User));
            }

            return allPosts;
        }
        static private List<AppUser> GetDistinctUsers(List<Tuple<Provider, string, AppUser>> users)
        {
            var fbDistinctUsers = users.Where(t => t.Item1 == Provider.Facebook).GroupBy(t => t.Item2).Select
               (
                   group => group.First().Item3
               );
            var tDistinctUsers = users.Where(t => t.Item1 == Provider.Twitter).GroupBy(t => t.Item2).Select
                (
                    group => group.Where(u => u.Item3.TwitterDetail.Description != null).Any() ?
                        group.Where(u => u.Item3.TwitterDetail.Description != null).First().Item3 :
                        group.First().Item3
                );

            var distinctUsers = new List<AppUser>();
            foreach (var distinctUser in fbDistinctUsers.Concat(tDistinctUsers))
            {
                if (distinctUser.IsNew)
                    distinctUsers.Add(distinctUser);
                else
                {
                    var existing = distinctUsers.Where(u => u.AppUserId == distinctUser.AppUserId).SingleOrDefault();
                    if (existing == null)
                    {
                        distinctUsers.Add(distinctUser);
                    }
                    else
                    {
                        if (existing.IsProvidedBy(Provider.Twitter) && distinctUser.IsProvidedBy(Provider.Twitter))
                        {
                            if (string.IsNullOrWhiteSpace(existing.TwitterDetail.Description))
                            {
                                existing.TwitterDetail.Description = distinctUser.TwitterDetail.Description;
                            }
                        }
                    }
                }
            }
            return distinctUsers;
        }
        static private List<Post> GetDistinctPosts(List<Tuple<Provider, string, Post>> posts)
        {
            var fbDistinctPosts = posts.Where(t => t.Item1 == Provider.Facebook).GroupBy(t => t.Item2).Select
              (
                  group => group.First().Item3
              );
            var tDistinctPosts = posts.Where(t => t.Item1 == Provider.Twitter).GroupBy(t => t.Item2).Select
                (
                    group => group.First().Item3
                );

            var distinctPosts = new List<Post>();
            foreach (var distinctPost in fbDistinctPosts.Concat(tDistinctPosts))
            {
                if (distinctPost.IsNew)
                    distinctPosts.Add(distinctPost);
                else
                {
                    var existing = distinctPosts.Where(u => u.PostId == distinctPost.PostId).SingleOrDefault();
                    if (existing == null)
                    {
                        distinctPosts.Add(distinctPost);
                    }
                }
            }
            return distinctPosts;
        }

    }
}
