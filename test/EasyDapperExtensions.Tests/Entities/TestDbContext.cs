using Microsoft.EntityFrameworkCore;

namespace EasyDapperExtensions.Tests.Entities
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<InsertUser> InsertUsers { get; set; }

        public virtual DbSet<InsertUserUseId> InsertUserUseIds { get; set; }

        public virtual DbSet<InsertGuidUser> GuidUsers { get; set; }

        public virtual DbSet<ModifiedUser> ModifiedUsers { get; set; }

        public virtual DbSet<DeletionUser> DeletionUsers { get; set; }
    }
}
