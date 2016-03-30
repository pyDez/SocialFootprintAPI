using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.DataLayer
{
    public class ApplicationDapperRepository : IApplicationRepository
    {
        private IAuthTokenRepository _AuthTokenRepository;
        private ICategoryRepository _CategoryRepository;
        private IFacebookPostDetailRepository _FacebookPostDetailRepository;
        private IFacebookUserDetailRepository _FacebookUserDetailRepository;
        private IFriendshipRepository _FriendshipRepository;
        private IPostRepository _PostRepository;
        private ISkillRepository _SkillRepository;
        private IVoteRepository _VoteRepository;
        private IUserRepository _UserRepository;
        public ApplicationDapperRepository()
        {
            this._AuthTokenRepository = new AuthTokenDapperRepository();
            this._CategoryRepository = new CategoryDapperRepository();
            this._FacebookPostDetailRepository = new FacebookPostDetailDapperRepository();
            this._FacebookUserDetailRepository = new FacebookUserDetailDapperRepository();
            this._FriendshipRepository = new FriendshipDapperRepository();
            this._PostRepository = new PostDapperRepository();
            this._SkillRepository = new SkillDapperRepository();
            this._VoteRepository = new VoteDapperRepository();
            this._UserRepository = new UserDapperRepository();
        }
        public IEnumerable<AuthToken> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._AuthTokenRepository.GetItems(commandType, sql, parameters);
        }

        public IEnumerable<AuthToken> Select(object criteria = null)
        {
            return this._AuthTokenRepository.Select(criteria);
        }

        public void Insert(AuthToken obj)
        {
            this._AuthTokenRepository.Insert(obj);
        }

        public void Update(AuthToken obj)
        {
            this._AuthTokenRepository.Update(obj);
        }

        public void Delete(AuthToken obj)
        {
            this._AuthTokenRepository.Delete(obj);
        }

        IEnumerable<Category> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._CategoryRepository.GetItems(commandType, sql, parameters);
        }

        public IEnumerable<Category> Select(object criteria = null)
        {
            return this._CategoryRepository.Select(criteria);
        }

        public void Insert(Category obj)
        {
            this._CategoryRepository.Insert(obj);
        }

        public void Update(Category obj)
        {
            this._CategoryRepository.Update(obj);
        }

        public void Delete(Category obj)
        {
            this._CategoryRepository.Delete(obj);
        }

        IEnumerable<FacebookPostDetail> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._FacebookPostDetailRepository.GetItems(commandType, sql, parameters);
        }

        IEnumerable<FacebookPostDetail> Select(object criteria = null)
        {
            return this._FacebookPostDetailRepository.Select(criteria);
        }

        public void Insert(FacebookPostDetail obj)
        {
            this._FacebookPostDetailRepository.Insert(obj);
        }

        public void Update(FacebookPostDetail obj)
        {
            this._FacebookPostDetailRepository.Update(obj);
        }

        public void Delete(FacebookPostDetail obj)
        {
            this._FacebookPostDetailRepository.Delete(obj);
        }

        IEnumerable<FacebookUserDetail> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._FacebookUserDetailRepository.GetItems(commandType, sql, parameters);
        }

        IEnumerable<FacebookUserDetail> Select(object criteria = null)
        {
            return this._FacebookUserDetailRepository.Select(criteria);
        }

        public void Insert(FacebookUserDetail obj)
        {
            this._FacebookUserDetailRepository.Insert(obj);
        }

        public void Update(FacebookUserDetail obj)
        {
            this._FacebookUserDetailRepository.Update(obj);
        }

        public void Delete(FacebookUserDetail obj)
        {
            this._FacebookUserDetailRepository.Delete(obj);
        }

        IEnumerable<Friendship> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._FriendshipRepository.GetItems(commandType, sql, parameters);
        }

        IEnumerable<Friendship> Select(object criteria = null)
        {
            return this._FriendshipRepository.Select(criteria);
        }

        public void Insert(Friendship obj)
        {
            this._FriendshipRepository.Insert(obj);
        }

        public void Update(Friendship obj)
        {
            this._FriendshipRepository.Update(obj);
        }

        public void Delete(Friendship obj)
        {
            this._FriendshipRepository.Delete(obj);
        }

        IEnumerable<Post> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._PostRepository.GetItems(commandType, sql, parameters);
        }

        IEnumerable<Post> Select(object criteria = null)
        {
            return this._PostRepository.Select(criteria);
        }

        public void Save(Post obj)
        {
            this._PostRepository.Save(obj);
        }

        public void Delete(Post obj)
        {
            this._PostRepository.Delete(obj);
        }

        IEnumerable<Skill> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._SkillRepository.GetItems(commandType, sql, parameters);
        }

        IEnumerable<Skill> Select(object criteria = null)
        {
            return this._SkillRepository.Select(criteria);
        }

        public void Insert(Skill obj)
        {
            this._SkillRepository.Insert(obj);
        }

        public void Update(Skill obj)
        {
            this._SkillRepository.Update(obj);
        }

        public void Delete(Skill obj)
        {
            this._SkillRepository.Delete(obj);
        }

        public IEnumerable<AppUser> GetItems(bool getItFully, System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._UserRepository.GetItems(getItFully, commandType, sql, parameters);
        }

        public IEnumerable<AppUser> Select(bool getItFully, object criteria = null)
        {
            return this._UserRepository.Select(getItFully, criteria);
        }

        public void Save(AppUser obj)
        {
            this._UserRepository.Save(obj);
        }

        public void Delete(AppUser obj)
        {
            this._UserRepository.Delete(obj);
        }

        IEnumerable<Vote> GetItems(System.Data.CommandType commandType, string sql, dynamic parameters = null)
        {
            return this._VoteRepository.GetItems(commandType, sql, parameters);
        }

        IEnumerable<Vote> Select(object criteria = null)
        {
            return this._VoteRepository.Select(criteria);
        }

        public void Insert(Vote obj)
        {
            this._VoteRepository.Insert(obj);
        }

        public void Update(Vote obj)
        {
            this._VoteRepository.Update(obj);
        }

        public void Delete(Vote obj)
        {
            this._VoteRepository.Delete(obj);
        }
    }
}
