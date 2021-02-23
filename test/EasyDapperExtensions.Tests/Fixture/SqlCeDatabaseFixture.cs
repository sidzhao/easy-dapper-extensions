#if !Net50
using Microsoft.EntityFrameworkCore;

namespace EasyDapperExtensions.Tests.Fixture
{
    /// <summary>
    /// Download SQLCe4.0 https://www.microsoft.com/zh-cn/download/confirmation.aspx?id=30709
    /// </summary>
    public class SqlCeDatabaseFixture : DatabaseFixture
    {
        public SqlCeDatabaseFixture()
        {
            var connectionString = "DataSource=easy_dapper_test.sdf;";
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlCe(connectionString);

            OpenConnection(builder.Options);
        }
    }
}
#endif