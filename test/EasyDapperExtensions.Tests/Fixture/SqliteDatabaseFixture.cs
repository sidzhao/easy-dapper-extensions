#if Net50
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EasyDapperExtensions.Tests.Fixture
{
    public class SqliteDatabaseFixture : DatabaseFixture
    {
        public SqliteDatabaseFixture()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite(connection);

            OpenConnection(builder.Options);
        }
    }
}
#endif
