#if Net50
using EasyDapperExtensions.Tests.Fixture;
using System.Threading.Tasks;
using Xunit;

namespace EasyDapperExtensions.Tests
{
    public class PostgresConnectionExtensionTest : ConnectionExtensionTest, IClassFixture<PostgresDatabaseFixture>
    {
        public PostgresConnectionExtensionTest(PostgresDatabaseFixture fixture) : base(fixture)
        {
        }

        protected override void TestInOperation()
        {
            // Do nothing
        }

        protected override Task TestInOperationAsync()
        {
            return Task.CompletedTask;
        }
    }
}

#endif