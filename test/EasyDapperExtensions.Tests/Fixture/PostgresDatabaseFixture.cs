#if Net50
using Microsoft.EntityFrameworkCore;

namespace EasyDapperExtensions.Tests.Fixture
{
    public class PostgresDatabaseFixture : DatabaseFixture
    {
        public PostgresDatabaseFixture()
        {
            var connectionString = @"Server=localhost;Database=easy_dapper_test;User ID=postgres;Password=123456;";
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(connectionString);

            OpenConnection(builder.Options);
        }
    }
}
#endif