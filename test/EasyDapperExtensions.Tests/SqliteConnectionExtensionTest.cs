#if Net50
using EasyDapperExtensions.Tests.Fixture;
using Xunit;

namespace EasyDapperExtensions.Tests
{
    public class SqliteConnectionExtensionTest : ConnectionExtensionTest, IClassFixture<SqliteDatabaseFixture>
    {
        public SqliteConnectionExtensionTest(SqliteDatabaseFixture fixture) : base(fixture)
        {
        }
    }
}
#endif