namespace EasyDapperExtensions.Generator.Adapter
{
    /// <summary>
    /// The interface for all EasyDapperExtensions database operations
    /// Implementing this is each provider's model.
    /// </summary>
    public interface ISqlAdapter
    {
        /// <summary>
        /// Formats the sql for getting new id after insert.
        /// </summary>
        string FormatAfterInsert();

        /// <summary>
        /// Formats the sql of table name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string FormatTableName(string tableName);

        /// <summary>
        /// Formats the sql of column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string FormatColumnName(string columnName);


        /// <summary>
        /// Formats the sql for selecting a first row.
        /// </summary>
        /// <param name="top">The table name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="where">The query condition.</param>
        string FormatSelectTop(int top, string tableName, string where);

        /// <summary>
        /// Formats the sql for counting.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="where">The query condition.</param>
        string FormatSelectCount(string tableName, string where);

        /// <summary>
        /// Formats the sql for selecting all.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="where">The query condition.</param>
        string FormatSelectAll(string tableName, string where);

        /// <summary>
        /// Formats the sql for paging.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="where">The query condition.</param>
        /// <param name="orderBy">The query order by.</param>
        /// <param name="skip">The skip rows number.</param>
        /// <param name="take">The select rows number.</param>
        string FormatSelectPaging(string tableName, string where, string orderBy, int skip, int take);

        /// <summary>
        /// Formats the sql for selecting exists.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="where">The query condition.</param>
        string FormatSelectExists(string tableName, string where);

        /// <summary>
        /// Formats the sql for field is null.
        /// </summary>
        /// <param name="isEqual">Whether is equal or not equal.</param>
        /// <param name="columnName">The column name.</param>
        string FormatFieldIsNull(bool isEqual, string columnName);

        /// <summary>
        /// Gets a column equality to a parameter.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="operator">The operator name.</param>
        /// <param name="parameterName">The parameter name.</param>
        string FormatColumnNameEqualsValue(string columnName, string @operator, string parameterName);
    }
}
