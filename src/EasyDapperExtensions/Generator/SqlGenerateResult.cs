
namespace EasyDapperExtensions.Generator
{
    /// <summary>
    /// A object with the generated sql and dynamic params.
    /// </summary>
    public class SqlGenerateResult
    {

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The param.</param>
        public SqlGenerateResult(string sql, dynamic param)
        {
            Parameters = param;
            Sql = sql;
        }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>
        /// The SQL.
        /// </value>
        public string Sql { get;  }

        /// <summary>
        /// Gets the param, for Select
        /// </summary>
        /// <value>
        /// The param.
        /// </value>
        public object Parameters { get; }

        /// <summary>
        /// 
        /// </summary>
        internal SqlGenerateContext Context { get; set; }
    }
}
