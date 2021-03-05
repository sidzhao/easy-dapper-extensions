using System;
using System.Data;
using EasyDapperExtensions.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyDapperExtensions.Tests.Fixture
{
    public abstract class DatabaseFixture : IDisposable
    {

        protected TestDbContext DbContext { get; set; }

        public IDbConnection Connection => DbContext?.Database.GetDbConnection();

        protected void OpenConnection(DbContextOptions options)
        {
            InitDatabase(options);

            InsertUsers();

            InsertModifiedUsers();

            InsertDeletionUsers();
        }

        private void InitDatabase(DbContextOptions options)
        {
            DbContext = new TestDbContext(options);

            DbContext.Database.EnsureDeleted();

            DbContext.Database.EnsureCreated();
        }

        private void InsertUsers()
        {
            DbContext.Users.Add(new User
            {
                Name = "User1",
                Status = UserStatus.Active,
                CreatedTime = DateTime.Now,
                IsStatic = true
            });

            DbContext.Users.Add(new User
            {
                Name = "User2",
                Status = UserStatus.Active,
                CreatedTime = DateTime.Now,
                IsStatic = true
            });

            DbContext.Users.Add(new User
            {
                Name = "User3",
                Status = UserStatus.Inactive,
                CreatedTime = DateTime.Now,
                IsStatic = true
            });

            DbContext.Users.Add(new User
            {
                Name = "User11",
                Status = UserStatus.Active,
                CreatedTime = DateTime.Now
            });

            DbContext.Users.Add(new User
            {
                Name = "User22",
                Status = UserStatus.Active,
                CreatedTime = DateTime.Now
            });


            DbContext.Users.Add(new User
            {
                Name = "User33",
                Status = UserStatus.Inactive,
                CreatedTime = DateTime.Now,
                IsStatic = false
            });

            DbContext.SaveChanges();
        }

        private void InsertModifiedUsers()
        {
            DbContext.ModifiedUsers.Add(new ModifiedUser
            {
                Name = "User1",
                IsActive = true
            });

            DbContext.ModifiedUsers.Add(new ModifiedUser
            {
                Name = "User2",
                IsActive = true
            });

            DbContext.SaveChanges();
        }

        private void InsertDeletionUsers()
        {
            DbContext.DeletionUsers.Add(new DeletionUser
            {
                Name = "User1",
            });

            DbContext.DeletionUsers.Add(new DeletionUser
            {
                Name = "User2",
            });

            DbContext.DeletionUsers.Add(new DeletionUser
            {
                Name = "User11",
            });

            DbContext.DeletionUsers.Add(new DeletionUser
            {
                Name = "User4",
            });

            DbContext.DeletionUsers.Add(new DeletionUser
            {
                Name = "User5",
            });

            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext?.Dispose();
        }
    }
}
