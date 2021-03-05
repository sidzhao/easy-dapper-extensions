#if Net50
using EasyDapperExtensions.Tests.Fixture;
using Xunit;

namespace EasyDapperExtensions.Tests
{
    public class MySqlConnectionExtensionTest : ConnectionExtensionTest, IClassFixture<MySqlDatabaseFixture>
    {
        public MySqlConnectionExtensionTest(MySqlDatabaseFixture fixture) : base(fixture)
        {
        }
    }
}
#endif