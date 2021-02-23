namespace EasyDapperExtensions.Generator
{
    /// <summary>
    /// Class that models the data structure in converting the expression tree into SQL and Params.
    /// </summary>
    internal class QueryParameter
    {
        internal QueryParameter()
        {
            IsGroup = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter" /> class.
        /// </summary>
        /// <param name="linkingOperator">The linking operator.</param>
        internal QueryParameter(string linkingOperator)
        {
            IsGroup = true;
            LinkingOperator = linkingOperator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="queryOperator">The query operator.</param>
        internal QueryParameter(string propertyName, object propertyValue, string queryOperator)
        {
            IsGroup = false;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            QueryOperator = queryOperator;
        }

        public bool IsGroup { get; }

        public string LinkingOperator { get; set; }

        public string PropertyName { get; set; }

        public object PropertyValue { get; set; }

        public string QueryOperator { get; set; }

        public string Sql { get; set; }

        public QueryParameter LeftParameter { get; set; }

        public QueryParameter RightParameter { get; set; }
    }
}