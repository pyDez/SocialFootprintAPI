using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IUserRepository : IUserStore<AppUser>, IUserPasswordStore<AppUser>, IUserSecurityStampStore<AppUser>, IUserEmailStore<AppUser>, IUserLockoutStore<AppUser, string>, IUserTwoFactorStore<AppUser, string>, IUserLoginStore<AppUser>
    {
        IEnumerable<AppUser> Select(DephtLevel dephtLevel, object criteria = null, SQLKeyWord ConditionalKeyWord = null);
        void Save(DephtLevel dephtLevel, AppUser obj);
        void Save(DephtLevel dephtLevel, List<AppUser> users);
        void Delete(AppUser obj);
    }


    public enum DephtLevel
    {
        UserBasic = 1, //no votes, no friends, no posts
        UserFootprint = 2, //no votes, no friends, no posts but footprint
        UserProfile = 3, //no votes, no friends, no posts but notifications and footprint and badges
        Friends = 4,//Votes, posts and friends for the user
        NewsFeed = 5//Votes, posts, friends and NewsFeed for the user

    }
}
