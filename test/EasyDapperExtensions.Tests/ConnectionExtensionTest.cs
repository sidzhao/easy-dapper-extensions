using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EasyDapperExtensions.Tests.Entities;
using EasyDapperExtensions.Tests.Fixture;
using Xunit;

namespace EasyDapperExtensions.Tests
{
    public abstract class ConnectionExtensionTest
    {
        private readonly DatabaseFixture _fixture;

        protected IDbConnection Connection => _fixture.Connection;

        protected ConnectionExtensionTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        #region Get

        [Fact]
        public void TestGet()
        {
            // Act
            var user1 = Connection.Get<User>(1);
            var user4 = Connection.Get<User>(4);

            // Assert
            Assert.Equal("User1", user1.Name);
            Assert.Equal("User11", user4.Name);
            Assert.Throws<InvalidOperationException>(() => Connection.Get<NoKeyUser>(1));
        }

        [Fact]
        public async Task TestGetAsync()
        {
            // Act
            var user1 = await Connection.GetAsync<User>(1);
            var user4 = await Connection.GetAsync<User>(4);

            // Assert
            Assert.Equal("User1", user1.Name);
            Assert.Equal("User11", user4.Name);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Connection.GetAsync<NoKeyUser>(1));
        }

        #endregion

        #region GetAll

        [Fact]
        public void TestGetAll()
        {
            // Act
            var users1 = Connection.GetAll<User>(p => p.Name.Contains("1") && p.Status == UserStatus.Active && p.IsStatic.Value);
            var users2 = Connection.GetAll<User>(p => !p.Name.Contains("1") && p.Status == UserStatus.Inactive && p.IsStatic.Value);
            var users3 = Connection.GetAll<User>(p => p.IsStatic == null);

            // Assert
            Assert.Single(users1);
            Assert.Single(users2);
            Assert.Equal(2, users3.Count());
        }

        [Fact]
        public async Task TestGetAllAsync()
        {
            // Act
            var users1 = await Connection.GetAllAsync<User>(p => p.Name.Contains("1") && p.Status == UserStatus.Active && p.IsStatic.Value);
            var users2 = await Connection.GetAllAsync<User>(p => !p.Name.Contains("1") && p.Status == UserStatus.Inactive && p.IsStatic.Value);
            var users3 = await Connection.GetAllAsync<User>(p => p.IsStatic == null);

            // Assert
            Assert.Single(users1);
            Assert.Single(users2);
            Assert.Equal(2, users3.Count());
        }

        #endregion

        #region GetPaged

        [Fact]
        public void TestGetPaged()
        {
            // Act
            var users1 = Connection.GetPaged<User>(1, 3);
            var users2 = Connection.GetPaged<User>(2, 3, p => p.Status == UserStatus.Active);

            // Assert
            Assert.Equal(6, users1.TotalCount);
            Assert.Equal(3, users1.Result.Count());
            Assert.Equal(3, users1.Result.ToList()[1].Id);

            Assert.Equal(4, users2.TotalCount);
            Assert.Equal(2, users2.Result.Count());
            Assert.Equal(4, users2.Result.ToList()[0].Id);
        }

        [Fact]
        public async Task TestGetPagedAsync()
        {
            // Act
            var users1 = await Connection.GetPagedAsync<User>(1, 3);
            var users2 = await Connection.GetPagedAsync<User>(2, 3, p => p.Status == UserStatus.Active);

            // Assert
            Assert.Equal(6, users1.TotalCount);
            Assert.Equal(3, users1.Result.Count());
            Assert.Equal(3, users1.Result.ToList()[1].Id);

            Assert.Equal(4, users2.TotalCount);
            Assert.Equal(2, users2.Result.Count());
            Assert.Equal(4, users2.Result.ToList()[0].Id);
        }

        #endregion

        #region GetSingle

        [Fact]
        public void TestGetSingle()
        {
            // Act
            var user1 = Connection.GetSingle<User>(p => p.Id == 2);

            // Assert
            Assert.Equal("User2", user1.Name);
            Assert.Throws<InvalidOperationException>(() => Connection.GetSingle<User>(p => p.Id == 100));
            Assert.Throws<InvalidOperationException>(() => Connection.GetSingle<User>(p => p.Status == UserStatus.Active && p.IsStatic == null));
        }

        [Fact]
        public async Task TestGetSingleAsync()
        {
            // Act
            var user1 = await Connection.GetSingleAsync<User>(p => p.Id == 2);

            // Assert
            Assert.Equal("User2", user1.Name);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Connection.GetSingleAsync<User>(p => p.Id == 100));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Connection.GetSingleAsync<User>(p => p.Status == UserStatus.Active && p.IsStatic == null));
        }

        #endregion

        #region GetSingleOrDefault

        [Fact]
        public void TestGetSingleOrDefault()
        {
            // Act
            var user1 = Connection.GetSingleOrDefault<User>(p => p.Id == 2);
            var user2 = Connection.GetSingleOrDefault<User>(p => p.Id == 100);

            // Assert
            Assert.Equal("User2", user1.Name);
            Assert.Null(user2);
            Assert.Throws<InvalidOperationException>(() => Connection.GetSingleOrDefault<User>(p => p.Status == UserStatus.Active));
        }

        [Fact]
        public async Task TestGetSingleOrDefaultAsync()
        {
            // Act
            var user1 = await Connection.GetSingleOrDefaultAsync<User>(p => p.Id == 2);
            var user2 = await Connection.GetSingleOrDefaultAsync<User>(p => p.Id == 100);

            // Assert
            Assert.Equal("User2", user1.Name);
            Assert.Null(user2);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Connection.GetSingleOrDefaultAsync<User>(p => p.Status == UserStatus.Active));
        }

        #endregion

        #region GetFirst

        [Fact]
        public void TestGetFirst()
        {
            // Act
            var user1 = Connection.GetFirst<User>();
            var user3 = Connection.GetFirst<User>(p => p.Status == UserStatus.Inactive);

            // Assert
            Assert.Equal("User1", user1.Name);
            Assert.Equal("User3", user3.Name);
            Assert.Throws<InvalidOperationException>(() => Connection.GetFirst<User>(p => p.Id == 100));
        }

        [Fact]
        public async Task TestGetFirstAsync()
        {
            // Act
            var user1 = await Connection.GetFirstAsync<User>();
            var user3 = await Connection.GetFirstAsync<User>(p => p.Status == UserStatus.Inactive);

            // Assert
            Assert.Equal("User1", user1.Name);
            Assert.Equal("User3", user3.Name);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Connection.GetFirstAsync<User>(p => p.Id == 100));
        }

        #endregion

        #region GetFirstOrDefault

        [Fact]
        public void TestGetFirstOrDefault()
        {
            // Act
            var user1 = Connection.GetFirstOrDefault<User>();
            var user3 = Connection.GetFirstOrDefault<User>(p => p.Status == UserStatus.Inactive);

            // Assert
            Assert.Equal("User1", user1.Name);
            Assert.Equal("User3", user3.Name);
            Assert.Null(Connection.GetFirstOrDefault<User>(p => p.Id == 100));
        }

        [Fact]
        public async Task TestGetFirstOrDefaultAsync()
        {
            // Act
            var user1 = await Connection.GetFirstOrDefaultAsync<User>();
            var user3 = await Connection.GetFirstOrDefaultAsync<User>(p => p.Status == UserStatus.Inactive);

            // Assert
            Assert.Equal("User1", user1.Name);
            Assert.Equal("User3", user3.Name);
            Assert.Null(await Connection.GetFirstOrDefaultAsync<User>(p => p.Id == 100));
        }

        #endregion

        #region Count

        [Fact]
        public void TestCount()
        {
            // Arrange
            var user1Id = 1;
            var likeString = "2";

            // Assert
            Assert.Equal(6, Connection.Count<User>());
            Assert.Equal(6, Connection.Count<User>(p => true));
            Assert.Equal(0, Connection.Count<User>(p => 1 == 2));
            Assert.Equal(0, Connection.Count<User>(p => false));
            Assert.Equal(1, Connection.Count<User>(p => p.Id == user1Id));
            Assert.Equal(3, Connection.Count<User>(p => p.Id > 3));
            Assert.Equal(4, Connection.Count<User>(p => p.Id >= 3));
            Assert.Equal(3, Connection.Count<User>(p => p.Id < 4));
            Assert.Equal(4, Connection.Count<User>(p => p.Id <= 4));
            Assert.Equal(5, Connection.Count<User>(p => p.Id != 5));
            Assert.Equal(1, Connection.Count<User>(p => !p.IsStatic.Value));
            Assert.Equal(3, Connection.Count<User>(p => p.IsStatic.Value == false || p.IsStatic == null));
            Assert.Equal(3, Connection.Count<User>(p => p.Name.Contains(likeString) || (p.IsStatic.Value && p.Status == UserStatus.Active)));

            TestInOperation();
        }

        protected virtual void TestInOperation()
        {
            var notInIds = new[] { 4, 5 };
            Assert.Equal(3, Connection.Count<User>(p => new[] { 1, 2, 3 }.Contains(p.Id)));
            Assert.Equal(4, Connection.Count<User>(p => !notInIds.Contains(p.Id)));
        }

        [Fact]
        public async Task TestCountAsync()
        {
            // Arrange
            var user1Id = 1;
            var likeString = "2";

            // Assert
            Assert.Equal(6, await Connection.CountAsync<User>());
            Assert.Equal(6, await Connection.CountAsync<User>(p => true));
            Assert.Equal(0, await Connection.CountAsync<User>(p => 1 == 2));
            Assert.Equal(0, await Connection.CountAsync<User>(p => false));
            Assert.Equal(1, await Connection.CountAsync<User>(p => p.Id == user1Id));
            Assert.Equal(3, await Connection.CountAsync<User>(p => p.Id > 3));
            Assert.Equal(4, await Connection.CountAsync<User>(p => p.Id >= 3));
            Assert.Equal(3, await Connection.CountAsync<User>(p => p.Id < 4));
            Assert.Equal(4, await Connection.CountAsync<User>(p => p.Id <= 4));
            Assert.Equal(5, await Connection.CountAsync<User>(p => p.Id != 5));
            Assert.Equal(1, await Connection.CountAsync<User>(p => !p.IsStatic.Value));
            Assert.Equal(3, await Connection.CountAsync<User>(p => p.IsStatic.Value == false || p.IsStatic == null));
            Assert.Equal(3, await Connection.CountAsync<User>(p => p.Name.Contains(likeString) || (p.IsStatic.Value && p.Status == UserStatus.Active)));

            await TestInOperationAsync();
        }

        protected virtual async Task TestInOperationAsync()
        {
            var notInIds = new[] { 4, 5 };
            Assert.Equal(3, await Connection.CountAsync<User>(p => new[] { 1, 2, 3, }.Contains(p.Id)));
            Assert.Equal(4, await Connection.CountAsync<User>(p => !notInIds.Contains(p.Id)));
        }


        #endregion

        #region Any

        [Fact]
        public void TestAny()
        {
            // Assert
            Assert.True(Connection.Any<User>());
            Assert.True(Connection.Any<User>(p => true));
            Assert.False(Connection.Any<User>(p => 1 == 2));
            Assert.False(Connection.Any<User>(p => false));
            Assert.True(Connection.Any<User>(p => p.Name.Contains("2") || (p.IsStatic.Value && p.Status == UserStatus.Active)));
        }

        [Fact]
        public async Task TestAnyAsync()
        {
            // Assert
            Assert.True(await Connection.AnyAsync<User>());
            Assert.True(await Connection.AnyAsync<User>(p => true));
            Assert.False(await Connection.AnyAsync<User>(p => 1 == 2));
            Assert.False(await Connection.AnyAsync<User>(p => false));
            Assert.True(await Connection.AnyAsync<User>(p => p.Name.Contains("2") || (p.IsStatic.Value && p.Status == UserStatus.Active)));
        }

        #endregion

        #region Insert

        [Fact]
        public void TestInsert()
        {
            // Arrange
            var newUser = new InsertUser
            {
                Name = "User1",
            };

            var newGuidUserId = Guid.NewGuid();
            var newGuidUser = new InsertGuidUser
            {
                Id = newGuidUserId,
                Name = "GuidUser1"
            };

            // Act
            Connection.Insert(newUser);
            Connection.Insert(newGuidUser);

            // Assert
            Assert.True(Connection.Any<InsertUser>(p => p.Name == "User1"));
            Assert.True(Connection.Any<InsertGuidUser>(p => p.Id == newGuidUserId));
        }

        [Fact]
        public async Task TestInsertAsync()
        {
            // Arrange
            var newUser = new InsertUser
            {
                Name = "User2",
            };

            var newGuidUserId = Guid.NewGuid();
            var newGuidUser = new InsertGuidUser
            {
                Id = newGuidUserId,
                Name = "GuidUser2"
            };

            // Act
            await Connection.InsertAsync(newUser);
            await Connection.InsertAsync(newGuidUser);

            // Assert
            Assert.True(await Connection.AnyAsync<InsertUser>(p => p.Name == "User2"));
            Assert.True(await Connection.AnyAsync<InsertGuidUser>(p => p.Id == newGuidUserId));
        }

        #endregion

        #region InsertAndGetId

        [Fact]
        public void TestInsertAndGetId()
        {
#if Net50
            // Arrange
            var newUser = new InsertUser
            {
                Name = "User3",
            };

            var newUserUseId = new InsertUserUseId
            {
                Name = "User3"
            };

            // Act
            var newUserId = Connection.InsertAndGetId<InsertUser, int>(newUser);
            var newUserUseIdId = Connection.InsertAndGetId<InsertUserUseId, int>(newUserUseId);

            // Assert
            Assert.True(Connection.Any<InsertUser>(p => p.Key == newUserId));
            Assert.True(Connection.Any<InsertUserUseId>(p => p.Id == newUserUseIdId));
#endif

            var newGuidUserId = Guid.NewGuid();
            var newGuidUser = new InsertGuidUser
            {
                Id = newGuidUserId,
                Name = "GuidUser3"
            };

            Assert.Equal(newGuidUserId, Connection.InsertAndGetId<InsertGuidUser, Guid>(newGuidUser));
            Assert.True(Connection.Any<InsertGuidUser>(p => p.Id == newGuidUserId));
        }

        [Fact]
        public async Task TestInsertAndGetIdAsync()
        {
#if Net50
            // Arrange
            var newUser = new InsertUser
            {
                Name = "User4",
            };

            var newUserUseId = new InsertUserUseId
            {
                Name = "User4"
            };

            // Act
            var newUserId = await Connection.InsertAndGetIdAsync<InsertUser, int>(newUser);
            var newUserUseIdId = await Connection.InsertAndGetIdAsync<InsertUserUseId, int>(newUserUseId);

            // Assert
            Assert.True(await Connection.AnyAsync<InsertUser>(p => p.Key == newUserId));
            Assert.True(await Connection.AnyAsync<InsertUserUseId>(p => p.Id == newUserUseIdId));
#endif

            var newGuidUserId = Guid.NewGuid();
            var newGuidUser = new InsertGuidUser
            {
                Id = newGuidUserId,
                Name = "GuidUser4"
            };

            Assert.Equal(newGuidUserId, await Connection.InsertAndGetIdAsync<InsertGuidUser, Guid>(newGuidUser));
            Assert.True(await Connection.AnyAsync<InsertGuidUser>(p => p.Id == newGuidUserId));
        }

        #endregion

        #region Update

        [Fact]
        public void TestUpdate()
        {
            // Arrange
            var user1 = Connection.GetSingle<ModifiedUser>(p => p.Id == 1);

            // Act
            user1.Name = "UpdateUser1";
            user1.IsActive = false;
            Connection.Update(user1);

            user1 = Connection.GetSingle<ModifiedUser>(p => p.Id == 1);

            // Assert
            Assert.Equal("UpdateUser1", user1.Name);
            Assert.False(user1.IsActive);
            Assert.Throws<InvalidOperationException>(() => Connection.Update(new NoKeyUser()));
        }

        [Fact]
        public async Task TestUpdateAsync()
        {
            // Arrange
            var user2 = await Connection.GetSingleAsync<ModifiedUser>(p => p.Id == 2);

            // Act
            user2.Name = "UpdateUser2";
            user2.IsActive = false;
            await Connection.UpdateAsync(user2);

            user2 = await Connection.GetSingleAsync<ModifiedUser>(p => p.Id == 2);

            // Assert
            Assert.Equal("UpdateUser2", user2.Name);
            Assert.False(user2.IsActive);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Connection.UpdateAsync(new NoKeyUser()));
        }

        #endregion

        #region Delete

        [Fact]
        public void TestDelete()
        {
            // Act
            Connection.Delete<DeletionUser>(p => p.Id == 2);
            Connection.Delete<DeletionUser>(3);
            Connection.Delete<DeletionUser>(p => p.Name.Contains("1"));

            // Assert
            Assert.Null(Connection.GetSingleOrDefault<DeletionUser>(p => p.Id == 2));
            Assert.Null(Connection.GetSingleOrDefault<DeletionUser>(p => p.Id == 3));
            Assert.Null(Connection.GetFirstOrDefault<DeletionUser>(p => p.Name.Contains("1")));
            Assert.Throws<InvalidOperationException>(() => Connection.Delete<NoKeyUser>(1));
        }

        [Fact]
        public async Task TestDeleteAsync()
        {
            // Act
            await Connection.DeleteAsync<DeletionUser>(p => p.Id == 4);
            await Connection.DeleteAsync<DeletionUser>(5);

            // Assert
            Assert.Null(Connection.GetSingleOrDefault<DeletionUser>(p => p.Id == 4));
            Assert.Null(Connection.GetSingleOrDefault<DeletionUser>(p => p.Id == 5));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Connection.DeleteAsync<NoKeyUser>(1));
        }

        #endregion
    }
}