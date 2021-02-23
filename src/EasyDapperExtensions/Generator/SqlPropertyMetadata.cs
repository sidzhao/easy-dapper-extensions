using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace EasyDapperExtensions.Generator
{
    internal class SqlPropertyMetadata
    {
        public PropertyInfo PropertyInfo { get; }

        public string Alias { get; set; }

        public string ColumnName { get; set; }

        public string Name => PropertyInfo.Name;

        public bool IsKey { get; set; }

        public bool IsIdentity { get; set; }

        public DatabaseGeneratedOption? SpecifiedDatabaseGeneratedOption { get; set; }

        public SqlPropertyMetadata(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            var alias = PropertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (alias != null)
            {

                Alias = alias.Name;
                ColumnName = Alias;
            }
            else
            {
                ColumnName = PropertyInfo.Name;
            }

            // Key
            IsKey = propertyInfo.GetCustomAttributes<KeyAttribute>().Any();

            // Identity
            var databaseGeneratedAttribute = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (databaseGeneratedAttribute != null)
            {
                SpecifiedDatabaseGeneratedOption = databaseGeneratedAttribute.DatabaseGeneratedOption;
                IsIdentity = SpecifiedDatabaseGeneratedOption != DatabaseGeneratedOption.None;
            }
        }
    }
}