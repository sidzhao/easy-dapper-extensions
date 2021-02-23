namespace EasyDapperExtensions.Generator.Adapter
{
    /// <summary>
    /// The SQL Server Compact Edition database adapter.
    /// </summary>
    public class SqlCeServerAdapter : ISqlAdapter
    {
        public string FormatTableName(string tableName)
        {
            return $"[{tableName}]";
        }

        public string FormatColumnName(string columnName)
        {
            return $"[{columnName}]";
        }

        public string FormatAfterInsert()
        {
            return "SELECT @@IDENTITY";
        }

        public string FormatSelectCount(string tableName, string where)
        {
            return $"SELECT COUNT(1) FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectTop(int top, string tableName, string where)
        {
            return $"SELECT TOP {top} * FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectAll(string tableName, string where)
        {
            return $"SELECT * FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectPaging(string tableName, string where, string orderBy, int skip, int take)
        {
            return $"SELECT TOP {take} * FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectExists(string tableName, string where)
        {
            return $"SELECT TOP 1 'True' FROM {FormatTableName(tableName)} WHERE EXISTS(SELECT 1 FROM {FormatTableName(tableName)}{where})";
        }

        public string FormatFieldIsNull(bool isEqual, string columnName)
        {
            return $"{FormatColumnName(columnName)} {(isEqual ? "IS" : "IS NOT")} NULL";
        }

        public string FormatColumnNameEqualsValue(string columnName, string @operator,
            string parameterName)
        {
            return $"{FormatColumnName(columnName)} {@operator} @{parameterName}";
        }
    }
}
