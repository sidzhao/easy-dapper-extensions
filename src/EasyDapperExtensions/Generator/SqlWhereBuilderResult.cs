namespace EasyDapperExtensions.Generator
{
    internal class SqlWhereBuilderResult
    {
        public SqlWhereBuilderResult()
        {
        }

        public SqlWhereBuilderResult(string where, object parameters)
        {
            Where = where;
            Parameters = parameters;
        }

        /// <summary>
        /// Where string
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// Gets the param, for Select
        /// </summary>
        /// <value>
        /// The param.
        /// </value>
        public object Parameters { get; }
    }
}
