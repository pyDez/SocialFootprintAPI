using System;
using System.Linq;
using iRocks.DataLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.ComponentModel;
using NClassifier.Bayesian;
using System.IO;
using NClassifier;
using iRocks.AI;

namespace iRocks.DataLayer.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {

        [TestMethod]
        public void Get_all_AppUser_should_return_6_results()
        {
            //arrange
            IUserRepository repository = CreateRepository();
            //act
            var emptyList = new List<int>();
            IEnumerable<AppUser> users = repository.Select(DephtLevel.NewsFeed, new { AppUserId = 23409 });
            repository.Save(DephtLevel.NewsFeed, users.First());
            var postbyuser = users.First().Friends.Select(f => f.Posts);
            List<Post> posts = new List<Post>();
            foreach (var list in postbyuser)
                posts.AddRange(list);
            var postsWithoutSnapshots = posts.Where(p => p.Snapshot == null);
            //assert²
            users.Should().NotBeNull();
            users.ToList().Count.Should().Be(6);
        }

        [TestMethod]
        public void Get_all_AppUser_And_collect_them_badges()
        {
            //arrange
            IUserRepository repository = CreateRepository();
            //act
            IEnumerable<AppUser> users = repository.Select(DephtLevel.NewsFeed, new { AppUserId = 10719 });

           foreach(var user in users)
            {
                collectBadges(user);
            }

            repository.Save(DephtLevel.NewsFeed, users.ToList());
            //assert²
            users.Should().NotBeNull();
            //users.ToList().Count.Should().Be(6);
        }
        private void collectBadges(AppUser user)
        {
            IBadgeRepository repository = new BadgeDapperRepository();
            var badges = repository.Select();

            ICategoryRepository categoryRepository = new CategoryDapperRepository();
            var categories = categoryRepository.Select();

            foreach( var category in categories)
            {
                if (category.CategoryId == 6)//comic
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 1).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 2).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
                if (category.CategoryId == 2)//RH
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 3).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 4).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
                if (category.CategoryId == 3)//Société
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 5).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 6).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
                if (category.CategoryId == 7)//Health
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 7).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 8).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
                if (category.CategoryId == 5)//sport
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 9).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 10).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
                if (category.CategoryId == 4)//culture
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 11).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 12).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
                if (category.CategoryId == 1)//General
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 13).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 14).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
                if (category.CategoryId == 8)//Business
                {
                    if (user.GetSkillLevel(category.CategoryId) > 75)
                    {
                        var badge = badges.Where(b => b.BadgeId == 15).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                    if (user.GetSkillLevel(category.CategoryId) > 90)
                    {
                        var badge = badges.Where(b => b.BadgeId == 16).FirstOrDefault();
                        user.Badges.Add(new BadgeCollected()
                        {
                            AppUserId = user.AppUserId,
                            BadgeId = badge.BadgeId,
                            CollectDate = DateTime.Today
                        });
                    }
                }
            }


            if (user.Votes.Count > 10)
            {
                var badge = badges.Where(b => b.BadgeId == 21).FirstOrDefault();
                user.Badges.Add(new BadgeCollected()
                {
                    AppUserId = user.AppUserId,
                    BadgeId = badge.BadgeId,
                    CollectDate = DateTime.Today
                });
            }
            if (user.Votes.Count > 50)
            {
                var badge = badges.Where(b => b.BadgeId == 22).FirstOrDefault();
                user.Badges.Add(new BadgeCollected()
                {
                    AppUserId = user.AppUserId,
                    BadgeId = badge.BadgeId,
                    CollectDate = DateTime.Today
                });
            }
            if (user.Votes.Count > 500)
            {
                var badge = badges.Where(b => b.BadgeId == 23).FirstOrDefault();
                user.Badges.Add(new BadgeCollected()
                {
                    AppUserId = user.AppUserId,
                    BadgeId = badge.BadgeId,
                    CollectDate = DateTime.Today
                });
            }
            if (user.Votes.Count > 5000)
            {
                var badge = badges.Where(b => b.BadgeId == 24).FirstOrDefault();
                user.Badges.Add(new BadgeCollected()
                {
                    AppUserId = user.AppUserId,
                    BadgeId = badge.BadgeId,
                    CollectDate = DateTime.Today
                });
            }
            foreach (var post in user.Posts)
            {
                if (post.UpVotes.Count() > 10)
                {
                    var badge = badges.Where(b => b.BadgeId == 17).FirstOrDefault();
                    user.Badges.Add(new BadgeCollected()
                    {
                        AppUserId = user.AppUserId,
                        BadgeId = badge.BadgeId,
                        CollectDate = DateTime.Today,
                        PostId = post.PostId
                    });
                }
                if (post.UpVotes.Count() > 30)
                {
                    var badge = badges.Where(b => b.BadgeId == 18).FirstOrDefault();
                    user.Badges.Add(new BadgeCollected()
                    {
                        AppUserId = user.AppUserId,
                        BadgeId = badge.BadgeId,
                        CollectDate = DateTime.Today,
                        PostId = post.PostId
                    });
                }
                if (post.UpVotes.Count() > 100)
                {
                    var badge = badges.Where(b => b.BadgeId == 19).FirstOrDefault();
                    user.Badges.Add(new BadgeCollected()
                    {
                        AppUserId = user.AppUserId,
                        BadgeId = badge.BadgeId,
                        CollectDate = DateTime.Today,
                        PostId = post.PostId
                    });
                }
                if (post.UpVotes.Count() > 1000)
                {
                    var badge = badges.Where(b => b.BadgeId == 20).FirstOrDefault();
                    user.Badges.Add(new BadgeCollected()
                    {
                        AppUserId = user.AppUserId,
                        BadgeId = badge.BadgeId,
                        CollectDate = DateTime.Today,
                        PostId = post.PostId
                    });
                }
            }

            
        }

        [TestMethod]
        public void Get_all_Post_should_return_6_results()
        {
            //arrange
            IPostRepository repository = CreatePostRepository();
            //act
            IEnumerable<Post> posts = repository.Select();
            var postsWithoutSnapshots = posts.Where(p => p.Snapshot == null);
            //assert²
            postsWithoutSnapshots.Should().BeNull();
            posts.Should().NotBeNull();
            posts.ToList().Count.Should().Be(14439);
        }

        [TestMethod]
        public void Get_Translation()
        {
            //arrangeRT
            var test=TranslationHelper.GetTranslation("fr_FR", "String1");
            //act
            //assert²
            test.Should().Be("test");
        }

        [TestMethod]
        public void Get_post()
        {
            var repo = new PostDapperRepository();
            //arrangeRT
            var posts = repo.Select(new { PostId = 2785 });
            //act
            //assert²
            posts.ToList().Count.Should().Be(1);
        }

        [TestMethod]
        public void Update_Post_should_roxxxx()
        {
            //arrange
            IPostRepository repository = CreatePostRepository();
            //act
            Post post = repository.Select(new { PostId = "5" }).First();
            //assert²
            post.Should().NotBeNull();
            //post.Activated.Should().Be(true);
            //arrange2

            post.Activated = false;
            //act2
            repository.Update(post);
            post = repository.Select(new { PostId = "5" }).First();
            //assert2
            post.Should().NotBeNull();
            post.Activated.Should().Be(false);

        }

        [TestMethod]
        public void Classify()
        {


            DbWordsDataSource wds = new DbWordsDataSource(new WordProbabilityDapperRepository());
            BayesianClassifier classifier = new BayesianClassifier(wds, new CategoryDapperRepository());
           
            /* using (StreamReader reader = new StreamReader(@"Resources\rt-polarity.pos"))
             {
                 positiveSamples = reader.ReadToEnd();
             }
             using (StreamReader reader = new StreamReader(@"Resources\rt-polarity.neg"))
             {
                 negativeSamples = reader.ReadToEnd();
             }

             classifier.TeachMatch(Categories.POSITIVE_CATEGORY, positiveSamples);
             classifier.TeachMatch(Categories.NEGATIVE_CATEGORY, negativeSamples);
             classifier.TeachNonMatch(Categories.POSITIVE_CATEGORY, negativeSamples);
             classifier.TeachNonMatch(Categories.NEGATIVE_CATEGORY, positiveSamples);*/

            var ShouldBeGood = classifier.Classify("I Love all of you guys");
            var ShouldBeBad = classifier.Classify("I am the happiest guy in the world");

            ShouldBeBad = classifier.Classify("loosing money all day... I am fed up");
            ShouldBeGood = classifier.Classify("hte ugliest death i have seen...");



        }



        static int id = 2;

        [TestMethod]
        public void Find_User_should_retrieve_existing_entity()
        {
            // arrange
            IUserRepository repository = CreateRepository();
            // act
            var user = repository.Select(DephtLevel.NewsFeed, new { AppUserId = 4750 }).FirstOrDefault();

            // assert
            user.Should().NotBeNull();
            user.AppUserId.Should().Be(id);
            user.FirstName.Should().Be("LaBron");
            user.LastName.Should().Be("James");
            user.EmailAddress.Should().Be("labron@heat.com");
            user.SigningUpDate.Should().BeOnOrAfter(new DateTime(2005, 10, 12, 12, 32, 9, 0));
            user.SigningUpDate.Should().BeOnOrBefore(new DateTime(2005, 12, 10, 12, 32, 11, 0));

            //user.Posts.Count.Should().Be(2);
            //user.Posts.Last().Category.Should().Be("Trip");

            //user.Posts.Last().UpVotes.Count.Should().NotBe(0);
            //user.Posts.Last().DownVotes.Count.Should().NotBe(0);
            //user.Posts.Last().UpVotes.First().Id.Should().Be(1);
            //user.Posts.Last().DownVotes.First().Id.Should().Be(4);


            //user.Votes.Count.Should().Be(1);
            //user.Votes.First().UpPostId.Should().Be(6);

            //user.Personality.ArtisticalCurrent.Should().Be(360);
        }

        [TestMethod]
        public void TVP_should_allowed_better_select()
        {
            using (var connection = GetOpenConnection())
            {


                var table = new DataTable { Columns = { { "id", typeof(int) } }, Rows = { { 1 }, { 2 }, { 3 } } };

                //int count = connection.Query<int>("#DataTableParameters", new { ids = table.AsTableValuedParameter() }, commandType: CommandType.StoredProcedure).First();
                //count.Should().Be(3);

                int count = connection.Query<int>("select count(1) from @ids", new { ids = table.AsTableValuedParameter("TVPType") }).First();
                count.Should().Be(3);


                var Parameters = new DynamicParameters();
                var Posts = connection.Query<Post>("select * from Post");

                Parameters.AddDynamicParams(new { PostIds = Posts.Select(c => c.PostId).ToList() });
                table = ToDataTable("id", Posts.Select(c => c.PostId));

                table = new DataTable { Columns = { { "id", typeof(int) } } };
                Posts.Select(c => c.PostId).ToList().ForEach(id => table.Rows.Add(id));
                IEnumerable<FacebookPostDetail> FacebookPostDetails = connection.Query<FacebookPostDetail>("SELECT * FROM FacebookPostDetail WHERE PostId IN (select * from @PostIds) ", new { PostIds = table.AsTableValuedParameter("TVPType") });

                //FacebookPostDetails = connection.Query<FacebookPostDetail>("SELECT * FROM FacebookPostDetail FPD INNER JOIN @PostIds p ON (p.id = FPD.PostId or p.id is null) ", new { PostIds = table.AsTableValuedParameter("TVPType") });

                try
                {
                    connection.Query<int>("select count(1) from @ids", new { ids = table.AsTableValuedParameter() }).First();
                    throw new InvalidOperationException();
                }
                catch (Exception ex)
                {
                    ex.Message.Equals("The table type parameter 'ids' must have a valid type name.");
                }
            }
        }

        [TestMethod]
        public void Insert_should_assign_identity_to_new_entity()
        {
            // arrange
            IUserRepository repository = CreateRepository();
            DateTime now = DateTime.Now;
            var user = new AppUser
            {
                FirstName = "Joe",
                LastName = "Blow",
                EmailAddress = "joe.blow@gmail.com",
                Activated = true,
                LastLogInDate = now,
                SigningUpDate = now
            };

            user.FacebookDetail = new FacebookUserDetail
            {
                FacebookAccessToken = "789754654"
            };
            Post post = new Post
            {
                CategoryId = 3,
                CreationDate = now
            };
            post.FacebookDetail = new FacebookPostDetail
            {
                FacebookPostId = "789754654",
                Link = "https://mail.google.com/mail/u/0/#inbox",
                LinkName = "Gmail",
                Message = "Hey hey hey",
                UpdateTime = now

            };

            user.Posts.Add(post);

            Vote vote = new Vote
            {
                UpPostId = 5,
                DownPostId = 2
            };
            user.Votes.Add(vote);
            user.Footprint.Add(
                new Skill { CategoryId = 2, SkillLevel = 183 }
                );
            user.Friends.Add(repository.Select(DephtLevel.UserBasic, new { AppUserId = 5 }).SingleOrDefault());

            // act
            repository.Save(DephtLevel.Friends, user);

            // assert
            var createdUser = repository.Select(DephtLevel.Friends, new { AppUserId = user.AppUserId }).SingleOrDefault();
            createdUser.AppUserId.Should().NotBe(0, "because Identity should have been assigned by database.");
            createdUser.Votes.Count.Should().NotBe(0);
            createdUser.Footprint.Count.Should().NotBe(0);
            createdUser.LastLogInDate.Should().Be(now);
            createdUser.SigningUpDate.Should().Be(now);
            Console.WriteLine("New ID: " + createdUser.AppUserId);
            id = user.AppUserId;
        }

        [TestMethod]
        public void Modify_should_update_existing_entity()
        {
            // arrange
            IUserRepository repository = CreateRepository();

            // act
            var user = repository.Select(DephtLevel.UserBasic, new { AppUserId = id }).FirstOrDefault();
            user.FirstName = "Bob";
            //user.Friends.RemoveAt(0);

            user.LastLogInDate = new DateTime(2014, 12, 10, 12, 32, 11, 0);
            //user.Posts[0].Category = PostCategory.Comical;
            repository.Save(DephtLevel.Friends, user);

            // create a new repository for verification purposes
            IUserRepository repository2 = CreateRepository();
            var modifiedUser = repository2.Select(DephtLevel.UserBasic, new { AppUserId = id }).FirstOrDefault();

            // assert
            modifiedUser.FirstName.Should().Be("Bob");
            //modifiedUser.Posts.First().Category.Should().Be(PostCategory.Comical);
            //modifiedUser.Friends.Count.Should().Be(0);
            modifiedUser.LastLogInDate.Should().Be(new DateTime(2014, 12, 10, 12, 32, 11, 0));
        }

        [TestMethod]
        public void Delete_should_remove_entity()
        {
            // arrange
            IUserRepository repository = CreateRepository();

            // act
            repository.Delete(repository.Select(DephtLevel.Friends, new { AppUserId = id }).FirstOrDefault());

            // create a new repository for verification purposes
            IUserRepository repository2 = CreateRepository();
            var deletedEntity = repository2.Select(DephtLevel.Friends, new { AppUserId = id }).FirstOrDefault();

            // assert
            deletedEntity.Should().BeNull();
        }

        //[TestMethod]
        //public void Find_Post_should_retrieve_existing_entity()
        //{
        //    // arrange
        //    IUserRepository repository = CreateRepository();

        //    // act
        //    var post = repository.GetFullPost(id);

        //    // assert
        //    post.Should().NotBeNull();
        //    post.Id.Should().Be(id);
        //    post.DownVotes.Count.Should().NotBe(0);
        //    post.UpVotes.Count.Should().Be(0);
        //   post.DownVotes.First().Id.Should().Be(3);

        //}

        //[TestMethod]
        //public void Find_Post_For_User_should_retrieve_existing_entities()
        //{
        //    // arrange
        //    IUserRepository repository = CreateRepository();

        //    // act
        //    var posts = repository.GetPostsForUser(id);

        //    // assert
        //    posts.Should().NotBeNull();
        //    posts.Count.Should().NotBe(0);
        //    foreach (var post in posts)
        //    {
        //        post.UserId.Should().Be(id);
        //        post.DownVotes.Count.Should().NotBe(0);
        //    }

        //}




        //[TestMethod]
        //public void Save_should_add_an_entity()
        //{
        //    // arrange
        //    IUserRepository repository = CreateRepository();

        //    // act
        //    User test = new User()
        //    {
        //        FirstName = "test",
        //        LastName = "test",
        //        EmailAddress = "test",
        //        FacebookId = "test",
        //        Personality = new Personality(),
        //        Activated = false,
        //        SigningUpDate = DateTime.Now,
        //        LastLogInDate = DateTime.Now
        //    };
        //    repository.SaveUser(test);
        //    Post testP = new Post()
        //    {
        //        UserId=3,
        //        Category = PostCategory.Other,
        //        Activated = true,
        //        CreationDate = DateTime.Now,
        //        FacebookId = "test",
        //        Link = "test",
        //        Message = "test",
        //        LinkName = "test",
        //        AttachedObjectId = "test",
        //        Picture = "test",
        //        Privacy = "test",
        //        VideoSource = "test",
        //        StatusType = "test",
        //        Story = "test",
        //        GeneralStatusType = "test",
        //        UpdateTime = DateTime.Now
        //    };
        //    repository.SavePost(testP);

        //    // create a new repository for verification purposes
        //    IUserRepository repository2 = CreateRepository();
        //    var modifiedUser = repository2.GetFullUser(id);

        //    // assert
        //    modifiedUser.FirstName.Should().Be("Bob");
        //    modifiedUser.Posts.First().Category.Should().Be(PostCategory.Comical);
        //    modifiedUser.Friends.Count.Should().Be(0);
        //    modifiedUser.LastLogInDate.Should().Be(new DateTime(2014, 12, 10, 12, 32, 11, 0));
        //}



        //[TestMethod]
        //public void DesactivateUser_should_Desactivate_entity()
        //{
        //    // arrange
        //    IUserRepository repository = CreateRepository();

        //    // act
        //    repository.DesactivateUser(id);

        //    // create a new repository for verification purposes
        //    IUserRepository repository2 = CreateRepository();
        //    var DesactivatedEntity = repository2.GetFullUser(id);

        //    // assert
        //    DesactivatedEntity.Activated.Should().Be(false);
        //    DesactivatedEntity.Posts.First().Activated.Should().Be(false);
        //}

        //[TestMethod]
        //public void DesactivatePost_should_Desactivate_entity()
        //{
        //    // arrange
        //    IUserRepository repository = CreateRepository();

        //    // act
        //    repository.DesactivatePost(id);

        //    // create a new repository for verification purposes
        //    IUserRepository repository2 = CreateRepository();
        //    var DesactivatedEntity = repository2.GetFullPost(id);

        //    // assert
        //    DesactivatedEntity.Activated.Should().Be(false);
        //}

        private IUserRepository CreateRepository()
        {
            return new UserDapperRepository();
        }
        private IPostRepository CreatePostRepository()
        {
            return new PostDapperRepository();
        }
        private SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection("server=WIN-798AGHPBJT4;database=iRocks;Trusted_Connection=Yes;"); 
            
            connection.Open();
            return connection;
        }
        public static DataTable ToDataTable<T>(string columnName, IEnumerable<T> data)
        {
            DataTable table = new DataTable();
            table.Columns.Add(columnName, typeof(int));

            foreach (T item in data)
            {
                table.Rows.Add(item);
            }
            return table;
        }

    }
}


