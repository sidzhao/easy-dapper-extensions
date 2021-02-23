#if Net50
using Microsoft.EntityFrameworkCore;

namespace EasyDapperExtensions.Tests.Fixture
{
    public class MySqlDatabaseFixture : DatabaseFixture
    {
        public MySqlDatabaseFixture()
        {
            var connectionString = "Server=localhost;Port=3306;Database=easy_dapper_test;Uid=root;Pwd=123456;";
            var builder = new DbContextOptionsBuilder();
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            OpenConnection(builder.Options);
        }
    }
}
#endif