#if Net50
using EasyDapperExtensions.Tests.Fixture;
using Xunit;

namespace EasyDapperExtensions.Tests
{
    public class SqlServerConnectionExtensionTest : ConnectionExtensionTest, IClassFixture<SqlServerDatabaseFixture>
    {
        public SqlServerConnectionExtensionTest(SqlServerDatabaseFixture fixture) : base(fixture)
        {
        }
    }
}
#endif