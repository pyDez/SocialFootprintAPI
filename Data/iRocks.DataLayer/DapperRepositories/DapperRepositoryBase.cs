using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace iRocks.DataLayer
{
    //The normal conventions of naming the Id field are supported: Id, TypeNameId or TypeName_Id . 
    //If you want to use some other name, you need to decorate the property with [DapperKey] attribute.
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperKey : Attribute
    {
    }
    //Properties without public setters are automatically skipped. 
    //If you want to skip other properties, you need to decorate them with [DapperIgnore] attribute.
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperIgnore : Attribute
    {
    }
    //Properties without public setters are automatically skipped. 
    //If you do not want to skip one of these properties, you need to decorate them with [DapperDoNotIgnore] attribute.
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperDoNotIgnore : Attribute
    {
    }

    public abstract class DapperRepositoryBase
    {
        private readonly string _connectionString;

        #region Constructor

        protected DapperRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Standard Dapper functionality
        protected IEnumerable<T> GetUnmanagedItems<T>(CommandType commandType, string sql, DynamicParameters parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Query<T>(sql, parameters, commandType: commandType);
            }
        }
        // Example: GetBySql<Activity>( "SELECT * 
        //FROM Activities WHERE Id = @activityId", new {activityId = 15} ).FirstOrDefault();
        protected IEnumerable<T> GetItems<T>(CommandType commandType, string sql, DynamicParameters parameters = null) where T : DapperManagedObject<T>
        {
            using (var connection = GetOpenConnection())
            {
                var objects = connection.Query<T>(sql, parameters, commandType: commandType);
                objects.ToList().ForEach(o => o.SetSnapshot(o));
                return objects;
            }
        }

        private IEnumerable<TReturn> GetItems<TFirst, TSecond, TReturn>(CommandType commandType, string sql, DynamicParameters parameters, Func<TFirst, TSecond, TReturn> map, string splitOn) where TReturn : DapperManagedObject<TReturn>
        {
            using (var connection = GetOpenConnection())
            {
                var objects = connection.Query<TFirst, TSecond, TReturn>(sql, map, param: parameters, splitOn: splitOn, commandType: commandType);
                objects.ToList().ForEach(o => o.SetSnapshot(o));
                return objects;
            }
        }

        private IEnumerable<TReturn> GetItems<TFirst, TSecond, TThird, TReturn>(CommandType commandType, string sql, DynamicParameters parameters, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn) where TReturn : DapperManagedObject<TReturn>
        {
            using (var connection = GetOpenConnection())
            {
                var objects = connection.Query<TFirst, TSecond, TThird, TReturn>(sql, map, param: parameters, splitOn: splitOn, commandType: commandType);
                objects.ToList().ForEach(o => o.SetSnapshot(o));
                return objects;
            }
        }

        private IEnumerable<TReturn> GetItems<TFirst, TSecond, TThird, TFourth, TReturn>(CommandType commandType, string sql, DynamicParameters parameters, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn) where TReturn : DapperManagedObject<TReturn>
        {
            using (var connection = GetOpenConnection())
            {
                var objects = connection.Query<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param: parameters, splitOn: splitOn, commandType: commandType);
                objects.ToList().ForEach(o => o.SetSnapshot(o));
                return objects;
            }
        }
        private IEnumerable<TReturn> GetItems<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandType commandType, string sql, DynamicParameters parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn) where TReturn : DapperManagedObject<TReturn>
        {
            using (var connection = GetOpenConnection())
            {
                var objects = connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(sql, map, param: parameters, splitOn: splitOn, commandType: commandType);
                objects.ToList().ForEach(o => o.SetSnapshot(o));
                return objects;
            }
        }
        private IEnumerable<TReturn> GetItems<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandType commandType, string sql, DynamicParameters parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn) where TReturn : DapperManagedObject<TReturn>
        {
            using (var connection = GetOpenConnection())
            {
                var objects = connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(sql, map, param: parameters, splitOn: splitOn, commandType: commandType);
                objects.ToList().ForEach(o => o.SetSnapshot(o));
                return objects;
            }
        }
        private IEnumerable<TReturn> GetItems<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandType commandType, string sql, DynamicParameters parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn) where TReturn : DapperManagedObject<TReturn>
        {
            using (var connection = GetOpenConnection())
            {
                var objects = connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param: parameters, splitOn: splitOn, commandType: commandType);
                objects.ToList().ForEach(o => o.SetSnapshot(o));
                return objects;
            }
        }




        protected int Execute(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Execute(sql, parameters, commandType: commandType);
            }
        }

        protected SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        #endregion

        #region Automated methods for: Insert, Update, Delete


        // For simple objects these methods will work fine, 
        // but they will not cover more complex scenarios!
        // Id column is assumed to be of type int IDENTITY.
        // Reflection is used to create appropriate SQL statements.
        // Even if reflection is costly in itself, the average gain 
        // compared to Entity Framework is approximately a factor 10!
        // Key property is determined by convention 
        // (Id, TypeNameId or TypeName_Id) or by custom attribute [DapperKey].
        // All properties with public setters are included. 
        // Exclusion can be manually made with custom attribute [DapperIgnore].
        // If key property is mapped to single database Identity column, 
        // then it is automatically reflected back to object.

        //
        /// <summary>
        /// Automatic generation of SELECT statement, BUT only for simple equality criterias!
        /// Example: Select<LogItem>(new {Class = "Client"})
        /// For more complex criteria it is necessary to call GetItems method with custom SQL statement.
        /// </summary>
        /// 

        protected IEnumerable<T> Select<T>(object criteria = null, SQLKeyWord ConditionalKeyWord = null) where T : DapperManagedObject<T>
        {
            var properties = ParseProperties<T>(criteria);
            ConditionalKeyWord = ConditionalKeyWord == null ? SQLKeyWord.And : ConditionalKeyWord;
            var sqlPairs = GetSqlPairs(properties.AllPairsDictionary, ConditionalKeyWord.Value);
            var sql = "";
            if (string.IsNullOrWhiteSpace(sqlPairs))
                sql = string.Format("SELECT * FROM [{0}] ", typeof(T).Name);
            else
                sql = string.Format("SELECT * FROM [{0}] WHERE {1}", typeof(T).Name, sqlPairs);
            return GetItems<T>(CommandType.Text, sql, properties.AllPairs);
        }

        protected IEnumerable<T> Select<T>(string sql, object criteria = null, SQLKeyWord ConditionalKeyWord = null) where T : DapperManagedObject<T>
        {
            var properties = ParseProperties<T>(criteria);
            ConditionalKeyWord = ConditionalKeyWord == null ? SQLKeyWord.And : ConditionalKeyWord;
            var sqlPairs = GetSqlPairs(properties.AllPairsDictionary, ConditionalKeyWord.Value);

            if (!string.IsNullOrWhiteSpace(sqlPairs))
                sql = string.Format(sql + " WHERE {0}", sqlPairs);
            return GetItems<T>(CommandType.Text, sql, properties.AllPairs);
        }

        protected IEnumerable<TReturn> Select<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object criteria = null, SQLKeyWord ConditionalKeyWord = null, string splitOn = "Id") where TReturn : DapperManagedObject<TReturn>
        {
            List<Type> types = new List<Type>() { typeof(TReturn), typeof(TFirst), typeof(TSecond) };
            var parameters = GetQueryParameters(types, sql, criteria, ConditionalKeyWord);
            return GetItems<TFirst, TSecond, TReturn>(CommandType.Text, parameters.Item1, parameters.Item2, map, splitOn);
        }

        protected IEnumerable<TReturn> Select<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object criteria = null, SQLKeyWord ConditionalKeyWord = null, string splitOn = "Id") where TReturn : DapperManagedObject<TReturn>
        {
            List<Type> types = new List<Type>() { typeof(TReturn), typeof(TFirst), typeof(TSecond), typeof(TThird) };
            var parameters = GetQueryParameters(types, sql, criteria, ConditionalKeyWord);
            return GetItems<TFirst, TSecond, TThird, TReturn>(CommandType.Text, parameters.Item1, parameters.Item2, map, splitOn);
        }

        protected IEnumerable<TReturn> Select<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object criteria = null, SQLKeyWord ConditionalKeyWord = null, string splitOn = "Id") where TReturn : DapperManagedObject<TReturn>
        {
            List<Type> types = new List<Type>() { typeof(TReturn), typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth) };
            var parameters = GetQueryParameters(types, sql, criteria, ConditionalKeyWord);
            return GetItems<TFirst, TSecond, TThird, TFourth, TReturn>(CommandType.Text, parameters.Item1, parameters.Item2, map, splitOn);
        }
        protected IEnumerable<TReturn> Select<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object criteria = null, SQLKeyWord ConditionalKeyWord = null, string splitOn = "Id") where TReturn : DapperManagedObject<TReturn>
        {
            List<Type> types = new List<Type>() { typeof(TReturn), typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth) };
            var parameters = GetQueryParameters(types, sql, criteria, ConditionalKeyWord);
            return GetItems<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandType.Text, parameters.Item1, parameters.Item2, map, splitOn);
        }
        protected IEnumerable<TReturn> Select<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object criteria = null, SQLKeyWord ConditionalKeyWord = null, string splitOn = "Id") where TReturn : DapperManagedObject<TReturn>
        {
            List<Type> types = new List<Type>() { typeof(TReturn), typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth), typeof(TSixth) };
            var parameters = GetQueryParameters(types, sql, criteria, ConditionalKeyWord);
            return GetItems<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandType.Text, parameters.Item1, parameters.Item2, map, splitOn);
        }
        protected IEnumerable<TReturn> Select<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object criteria = null, SQLKeyWord ConditionalKeyWord = null, string splitOn = "Id") where TReturn : DapperManagedObject<TReturn>
        {
            List<Type> types = new List<Type>() { typeof(TReturn), typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth), typeof(TSixth), typeof(TSeventh) };
            var parameters = GetQueryParameters(types, sql, criteria, ConditionalKeyWord);
            return GetItems<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandType.Text, parameters.Item1, parameters.Item2, map, splitOn);
        }

        //better to put TReturn first in the types list
        private Tuple<string, DynamicParameters> GetQueryParameters(List<Type> types, string sql, object criteria, SQLKeyWord ConditionalKeyWord)
        {
            ConditionalKeyWord = ConditionalKeyWord == null ? SQLKeyWord.And : ConditionalKeyWord;
            var sqlPairs = "";
            Dictionary<string, object> allPairs = new Dictionary<string, object>();
            List<string> blackList = new List<string>();
            foreach (var aType in types)
            {
                MethodInfo method = typeof(DapperRepositoryBase).GetMethod("ParseProperties");
                MethodInfo generic = method.MakeGenericMethod(aType);
                var properties = (PropertyContainer)generic.Invoke(this, new object[] { criteria, blackList });
                //var properties = ParseProperties<TFirst>(criteria);
                if (!string.IsNullOrWhiteSpace(sqlPairs) && properties.AllNames.Any())
                    sqlPairs += ConditionalKeyWord.Value;
                sqlPairs += GetSqlPairs(properties.AllPairsDictionary, ConditionalKeyWord.Value, properties.TypeName);
                blackList.AddRange(properties.AllNames);
                foreach (var pair in properties.AllPairsDictionary)
                    if (!allPairs.ContainsKey(pair.Key))
                        allPairs.Add(pair.Key, pair.Value);
                //allPairs.AddDynamicParams(properties.AllPairs);
            }

            var pairs = new DynamicParameters();
            foreach (var pair in allPairs)
                pairs.Add(pair.Key, value: pair.Value);

            if (!string.IsNullOrWhiteSpace(sqlPairs))
                sql = string.Format(sql + " WHERE {0}", sqlPairs);

            return new Tuple<string, DynamicParameters>(sql, pairs);
        }




        protected void Insert<T>(T obj) where T : DapperManagedObject<T>
        {
            var propertyContainer = ParseProperties<T>(obj);
            var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES (@{2}) SELECT CAST(scope_identity() AS int)",
                typeof(T).Name,
                string.Join(", ", propertyContainer.ValueNames),
                string.Join(", @", propertyContainer.ValueNames));

            using (var connection = GetOpenConnection())
            {
                var id = connection.Query<int>
                (sql, propertyContainer.ValuePairs, commandType: CommandType.Text).First();
                SetId(obj, id, propertyContainer.IdPairs);
                obj.SetSnapshot(obj);
            }
        }


        protected void Update<T>(T obj) where T : IGetId, IDbSnapshot<T>
        {
            //var propertyContainer = ParseProperties<T>(obj);
            //var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            //var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
            //var sql = string.Format("UPDATE [{0}] SET {1} WHERE {2}", typeof(T).Name, sqlValuePairs, sqlIdPairs);
            //Execute(CommandType.Text, sql, propertyContainer.AllPairs);
            using (var connection = GetOpenConnection())
            {
                var db = IRocksDatabase.Init(connection, commandTimeout: 3);
                var difference = obj.GetDifference(obj);
                if (difference.ParameterNames.Count() > 0)
                {
                    db.GetTable(obj).Update(obj.GetId(), difference, typeof(T).Name + "Id");
                    obj.SetSnapshot(obj);
                }
            }

        }

        protected void Delete<T>(T obj)
        {
            var propertyContainer = ParseProperties<T>(obj);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdPairsDictionary);
            var sql = string.Format("DELETE FROM [{0}] WHERE {1}", typeof(T).Name, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.IdPairs);
        }

        #endregion

        #region Reflection support

        /// <summary>
        /// Retrieves a Dictionary with name and value 
        /// for all object properties matching the given criteria.
        /// </summary>
        public static PropertyContainer ParseProperties<T>(object obj, List<string> BlackList = null)
        {
            var propertyContainer = new PropertyContainer();
            if (obj == null)
                return propertyContainer;
            propertyContainer.TypeName = typeof(T).Name;
            var validKeyNames = new[] { "Id",
            string.Format("{0}Id", propertyContainer.TypeName), string.Format("{0}_Id", propertyContainer.TypeName) };

            var properties = typeof(T).GetProperties();

            List<string> criteriaPropertyNames = new List<string>();
            if (obj.GetType() == typeof(JObject))
            {

                foreach (var property in (JObject)obj)
                {
                    criteriaPropertyNames.Add(property.Key);
                }
            }
            else
            {
                var criteriaProperties = obj.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in criteriaProperties)
                {
                    criteriaPropertyNames.Add(propertyInfo.Name);
                }
            }

            foreach (var property in properties)
            {
                //Skip properties not set in criteria
                if (!criteriaPropertyNames.Contains(property.Name))
                    continue;
                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null && !property.IsDefined(typeof(DapperDoNotIgnore), false))
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;

                var name = property.Name;
                dynamic value = null;

                if (obj.GetType() == typeof(JObject))
                    value = Convert.ChangeType(((JObject)obj)[property.Name], property.PropertyType);
                else
                    value = obj.GetType().GetProperty(property.Name).GetValue(obj, null);
                if (BlackList == null || !BlackList.Contains(name))
                {
                    if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name))
                    {
                        propertyContainer.AddId(name, value);
                    }
                    else
                    {

                        propertyContainer.AddValue(name, value);
                    }
                }
            }

            return propertyContainer;
        }

        /// <summary>
        /// Create a commaseparated list of value pairs on 
        /// the form: "key1=@value1, key2=@value2, ..."
        /// </summary>
        private static string GetSqlPairs
        (Dictionary<string, dynamic> pairs, string separator = ", ", string tableName = "")
        {
            if (!string.IsNullOrWhiteSpace(tableName))
                tableName += ".";
            var sqlPairs = new List<string>();
            foreach (var pair in pairs)
            {

                if (pair.Value.GetType().FullName == "Dapper.TableValuedParameter")
                    sqlPairs.Add(string.Format("{1}{0} IN (SELECT * FROM @{0})", pair.Key, tableName));
                else if ((pair.Value.GetType().IsGenericType && pair.Value is IEnumerable))
                    sqlPairs.Add(string.Format("{1}{0} IN @{0}", pair.Key, tableName));
                else
                    sqlPairs.Add(string.Format("{1}{0}=@{0}", pair.Key, tableName));
            }
            return string.Join(separator, sqlPairs);
        }

        private void SetId<T>(T obj, int id, DynamicParameters propertyPairs)
        {
            if (propertyPairs.ParameterNames.Count() == 1)
            {
                var propertyName = propertyPairs.ParameterNames.First();
                var propertyInfo = obj.GetType().GetProperty(propertyName);
                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
        }

        #endregion

        public class PropertyContainer
        {
            private readonly Dictionary<string, object> _ids;
            private readonly Dictionary<string, object> _values;

            #region Properties

            internal IEnumerable<string> IdNames
            {
                get { return _ids.Keys; }
            }

            internal IEnumerable<string> ValueNames
            {
                get { return _values.Keys; }
            }

            internal IEnumerable<string> AllNames
            {
                get { return _ids.Keys.Union(_values.Keys); }
            }

            internal DynamicParameters IdPairs
            {
                get
                {
                    var pairs = new DynamicParameters();
                    foreach (var pair in _ids)
                    {

                        pairs.Add("@" + pair.Key, value: GetParameterWithTVP(pair.Value));
                    }

                    return pairs;
                }
            }

            internal DynamicParameters ValuePairs
            {
                get
                {
                    var pairs = new DynamicParameters();
                    foreach (var pair in _values)
                    {

                        pairs.Add("@" + pair.Key, value: GetParameterWithTVP(pair.Value));
                    }

                    return pairs;
                }
            }

            internal DynamicParameters AllPairs
            {
                get
                {
                    var pairs = new DynamicParameters();
                    foreach (var pair in _ids.Concat(_values))
                    {

                        pairs.Add("@" + pair.Key, value: GetParameterWithTVP(pair.Value));
                    }

                    return pairs;
                }
            }
            internal Dictionary<string, object> IdPairsDictionary
            {
                get
                {
                    return _ids.ToDictionary(kvp => kvp.Key, kvp => GetParameterWithTVP(kvp.Value)); ;
                }
            }
            internal Dictionary<string, object> AllPairsDictionary
            {
                get
                {
                    return _ids.Concat(_values).ToDictionary(kvp => kvp.Key, kvp => GetParameterWithTVP(kvp.Value)); ;
                }
            }

            private object GetParameterWithTVP(dynamic parameterValue)
            {
                if (parameterValue != null && parameterValue.GetType().IsGenericType && parameterValue is IEnumerable && parameterValue.Count > 2000)
                {
                    var table = new DataTable { Columns = { { "id", parameterValue[0].GetType() } } };
                    foreach (var param in parameterValue)
                        table.Rows.Add(param);
                    if (parameterValue[0].GetType() == typeof(string))
                        return table.AsTableValuedParameter("TVPTypeString");
                    else if (parameterValue[0].GetType() == typeof(int))
                        return table.AsTableValuedParameter("TVPType");
                    else
                        return parameterValue;
                }
                else
                    return parameterValue;
            }

            public string TypeName { get; set; }
            #endregion

            #region Constructor

            internal PropertyContainer()
            {
                _ids = new Dictionary<string, object>();
                _values = new Dictionary<string, object>();
            }

            #endregion

            #region Methods

            internal void AddId(string name, object value)
            {
                _ids.Add(name, value);
            }

            internal void AddValue(string name, object value)
            {
                _values.Add(name, value);
            }

            #endregion
        }

        public class IRocksDatabase : Database<IRocksDatabase>
        {
            public Table<AppUser> Users { get; set; }
            public Table<AuthToken> AuthTokens { get; set; }
            public Table<Category> Categories { get; set; }
            public Table<ExternalLogin> ExternalLogins { get; set; }
            public Table<FacebookPostDetail> FacebookPostDetails { get; set; }
            public Table<FacebookUserDetail> FacebookUserDetails { get; set; }
            public Table<Friendship> Friendships { get; set; }
            public Table<Skill> Skills { get; set; }
            public Table<Vote> Votes { get; set; }
            public Table<Post> Posts { get; set; }
            public Table<Newsfeed> Newsfeeds { get; set; }
            public Table<PostRelationship> PostRelationships { get; set; }
            public Table<StoryTranslation> StoryTranslations { get; set; }
            public Table<Notification> Notifications { get; set; }
            public Table<WordProbability> WordProbabilities { get; set; }
            public Table<Badge> Badges { get; set; }
            public Table<BadgeCollected> BadgeCollection { get; set; }
            public Table<BadgeTranslation> BadgeTranslations { get; set; }
            public Table<TwitterPostDetail> TwitterPostDetails { get; set; }
            public Table<TwitterUserDetail> TwitterUserDetails { get; set; }
            public Table<AccessTokenPair> AccessTokenPairs { get; set; }
            public Table<CategoryTranslation> CategoryTranslations { get; set; }
            public Table<Hashtag> Hashtags { get; set; }
            public Table<PostMedia> PostMedias { get; set; }
            public Table<PostMentionedUser> PostMentionedUsers { get; set; }
            public Table<PostUrl> PostUrls { get; set; }
            public dynamic GetTable(dynamic obj)
            {
                if (obj is AppUser)
                    return Users;
                else if (obj is AuthToken)
                    return AuthTokens;
                else if (obj is Category)
                    return Categories;
                else if (obj is ExternalLogin)
                    return ExternalLogins;
                else if (obj is FacebookPostDetail)
                    return FacebookPostDetails;
                else if (obj is FacebookUserDetail)
                    return FacebookUserDetails;
                else if (obj is Friendship)
                    return Friendships;
                else if (obj is Skill)
                    return Skills;
                else if (obj is Vote)
                    return Votes;
                else if (obj is Post)
                    return Posts;
                else if (obj is Newsfeed)
                    return Newsfeeds;
                else if (obj is PostRelationship)
                    return PostRelationships;
                else if (obj is StoryTranslation)
                    return StoryTranslations;
                else if (obj is Notification)
                    return Notifications;
                else if (obj is WordProbability)
                    return WordProbabilities;
                else if (obj is Badge)
                    return Badges;
                else if (obj is BadgeCollected)
                    return BadgeCollection;
                else if (obj is BadgeTranslation)
                    return BadgeTranslations;
                else if (obj is TwitterPostDetail)
                    return TwitterPostDetails;
                else if (obj is TwitterUserDetail)
                    return TwitterUserDetails;
                else if (obj is AccessTokenPair)
                    return AccessTokenPairs;
                else if (obj is CategoryTranslation)
                    return CategoryTranslations;
                else if (obj is Hashtag)
                    return Hashtags;
                else if (obj is PostMedia)
                    return PostMedias;
                else if (obj is PostMentionedUser)
                    return PostMentionedUsers;
                else if (obj is PostUrl)
                    return PostUrls;
                return null;
            }
        }



    }

    public class SQLKeyWord
    {
        private SQLKeyWord(string value) { Value = value; }

        public string Value { get; set; }

        public static SQLKeyWord And { get { return new SQLKeyWord(" AND "); } }
        public static SQLKeyWord Or { get { return new SQLKeyWord(" OR "); } }
    }
}
