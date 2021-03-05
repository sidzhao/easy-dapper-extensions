namespace EasyDapperExtensions.Generator.Adapter
{
    /// <summary>
    /// The Firebase SQL adapter.
    /// </summary>
    public class FbAdapter : ISqlAdapter
    {
        //public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        //{
        //    var cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
        //    connection.Execute(cmd, entityToInsert, transaction, commandTimeout);

        //    var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
        //    var keyName = propertyInfos[0].Name;
        //    var r = connection.Query($"SELECT FIRST 1 {keyName} ID FROM {tableName} ORDER BY {keyName} DESC", transaction: transaction, commandTimeout: commandTimeout);

        //    var id = r.First().ID;
        //    if (id == null) return 0;
        //    if (propertyInfos.Length == 0) return Convert.ToInt32(id);

        //    var idp = propertyInfos[0];
        //    idp.SetValue(entityToInsert, Convert.ChangeType(id, idp.PropertyType), null);

        //    return Convert.ToInt32(id);
        //}

        public string FormatAfterInsert()
        {
            return "SELECT last_insert_rowid()";
        }

        public string FormatTableName(string tableName)
        {
            return tableName;
        }

        public string FormatColumnName(string columnName)
        {
            return columnName;
        }

        public string FormatSelectCount(string tableName, string where)
        {
            return $"SELECT COUNT(1) FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectTop(int top, string tableName, string where)
        {
            return $"SELECT FIRST {top} * FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectAll(string tableName, string where)
        {
            return $"SELECT * FROM {FormatTableName(tableName)}{where}";
        }

        public string FormatSelectPaging(string tableName, string where, string orderBy, int skip, int take)
        {
            return $"SELECT * FROM {FormatTableName(tableName)}{where} LIMIT {skip},{take}";
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
