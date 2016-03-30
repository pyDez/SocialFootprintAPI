using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public interface IApplicationRepository
    {
        IEnumerable<AuthToken> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<AuthToken> Select(object criteria = null);
        void Insert(AuthToken obj);
        void Update(AuthToken obj);
        void Delete(AuthToken obj);
        IEnumerable<Category> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<Category> Select(object criteria = null);
        void Insert(Category obj);
        void Update(Category obj);
        void Delete(Category obj);
        IEnumerable<FacebookPostDetail> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<FacebookPostDetail> Select(object criteria = null);
        void Insert(FacebookPostDetail obj);
        void Update(FacebookPostDetail obj);
        void Delete(FacebookPostDetail obj);
        IEnumerable<FacebookUserDetail> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<FacebookUserDetail> Select(object criteria = null);
        void Insert(FacebookUserDetail obj);
        void Update(FacebookUserDetail obj);
        void Delete(FacebookUserDetail obj);
        IEnumerable<Friendship> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<Friendship> Select(object criteria = null);
        void Insert(Friendship obj);
        void Update(Friendship obj);
        void Delete(Friendship obj);
        IEnumerable<Post> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<Post> Select(object criteria = null);
        void Save(Post obj);
        void Delete(Post obj);
        IEnumerable<Skill> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<Skill> Select(object criteria = null);
        void Insert(Skill obj);
        void Update(Skill obj);
        void Delete(Skill obj);
        IEnumerable<AppUser> GetItems(bool getItFully, CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<AppUser> Select(bool getItFully, object criteria = null);
        void Save(AppUser obj);
        void Delete(AppUser obj);
        IEnumerable<Vote> GetItems(CommandType commandType, string sql, dynamic parameters = null);
        IEnumerable<Vote> Select(object criteria = null);
        void Insert(Vote obj);
        void Update(Vote obj);
        void Delete(Vote obj);
    }
}
