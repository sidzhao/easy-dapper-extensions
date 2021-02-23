#if !Net50
using EasyDapperExtensions.Tests.Fixture;
using Xunit;

namespace EasyDapperExtensions.Tests
{
    public class SqlCeConnectionExtensionTest : ConnectionExtensionTest, IClassFixture<SqlCeDatabaseFixture>
    {
        public SqlCeConnectionExtensionTest(SqlCeDatabaseFixture fixture) : base(fixture)
        {
        }
    }
}
#endif