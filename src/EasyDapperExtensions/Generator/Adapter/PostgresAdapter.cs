namespace EasyDapperExtensions.Generator.Adapter
{
    /// <summary>
    /// The Postgres database adapter.
    /// </summary>
    public class PostgresAdapter : ISqlAdapter
    {
        public string FormatTableName(string tableName)
        {
            return $"\"{tableName}\"";
        }

        public string FormatColumnName(string columnName)
        {
            return $"\"{columnName}\"";
        }

        public string FormatAfterInsert()
        {
            return "RETURNING";
        }

        public string FormatSelectCount(string tableName, string where)
        {
            return $"SELECT COUNT(1) FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectTop(int top, string tableName, string where)
        {
            return $"SELECT * FROM {FormatTableName(tableName)}{where} LIMIT {top}";
        }

        public string FormatSelectAll(string tableName, string where)
        {
            return $"SELECT * FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectPaging(string tableName, string where, string orderBy, int skip, int take)
        {
            return $"SELECT * FROM {FormatTableName(tableName)}{where} LIMIT {take} OFFSET {skip}";
        }

        public string FormatSelectExists(string tableName, string where)
        {
            return $"SELECT EXISTS(SELECT 1 FROM {FormatTableName(tableName)}{where})";
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
