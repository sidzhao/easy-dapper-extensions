#if Net50
using Microsoft.EntityFrameworkCore;

namespace EasyDapperExtensions.Tests.Fixture
{
    public class SqlServerDatabaseFixture : DatabaseFixture
    {
        public SqlServerDatabaseFixture()
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EasyDapperTest;Integrated Security=True;";
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(connectionString);

            OpenConnection(builder.Options);
        }
    }
}
#endif