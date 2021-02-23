using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EasyDapperExtensions.Generator.Adapter;
using EasyDapperExtensions.Helper;

namespace EasyDapperExtensions.Generator
{
    /// <summary>
    /// Universal SqlGenerator for Tables
    /// </summary>
    internal static class SqlGenerator
    {
        private static readonly ConcurrentDictionary<string, SqlGenerateContext> SqlGenerateContextDictionary = new ConcurrentDictionary<string, SqlGenerateContext>();

        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary
            = new Dictionary<string, ISqlAdapter>(6)
            {
                ["sqlconnection"] = new SqlServerAdapter(),
                ["sqlceconnection"] = new SqlCeServerAdapter(),
                ["npgsqlconnection"] = new PostgresAdapter(),
                ["sqliteconnection"] = new SQLiteAdapter(),
                ["mysqlconnection"] = new MySqlAdapter(),
                ["fbconnection"] = new FbAdapter()
            };

        #region Insert

        /// <summary>
        /// Get SQL for INSERT Query
        /// </summary>
        public static SqlGenerateResult GetInsert<TEntity>(IDbConnection connection, TEntity entity)
        {
            var context = BuildContext<TEntity>(connection);

            var nonIdentitySqlProperties = context.NonIdentitySqlProperties;

            var columns = string.Join(", ", nonIdentitySqlProperties.Select(p => context.Adapter.FormatColumnName(p.ColumnName)));
            string values = string.Join(", ", nonIdentitySqlProperties.Select(p => $"@{p.Name}"));
            var sql = $"INSERT INTO {context.Adapter.FormatTableName(context.TableName)} {(string.IsNullOrEmpty(columns) ? "" : $"({columns})")} VALUES {(string.IsNullOrEmpty(values) ? "" : $"({values})")}";

            return new SqlGenerateResult(sql, entity) { Context = context };
        }

        /// <summary>
        /// Get SQL for INSERT Query
        /// </summary>
        public static SqlGenerateResult GetInsertAndGetId<TEntity>(IDbConnection connection, TEntity entity)
        {
            var context = BuildContext<TEntity>(connection);

            var nonIdentitySqlProperties = context.NonIdentitySqlProperties;

            var columns = string.Join(", ", nonIdentitySqlProperties.Select(p => context.Adapter.FormatColumnName(p.ColumnName)));
            string values = string.Join(", ", nonIdentitySqlProperties.Select(p => $"@{p.Name}"));
            var sql = $"INSERT INTO {context.Adapter.FormatTableName(context.TableName)}{(string.IsNullOrEmpty(columns) ? "" : $"({columns})")} VALUES{(string.IsNullOrEmpty(values) ? "" : $"({values})")}";


            if (context.HasIdentitySqlProperty)
            {
                if (context.Adapter is PostgresAdapter)
                {
                    sql += $" {context.Adapter.FormatAfterInsert()} {string.Join(",", context.IdentitySqlProperties.Select(p => context.Adapter.FormatColumnName(p.ColumnName)))}";
                }
                else
                {
                    sql += $";{context.Adapter.FormatAfterInsert()}";
                }

            }

            return new SqlGenerateResult(sql, entity) { Context = context };
        }

        #endregion

        #region Update

        /// <summary>
        /// Get SQL for UPDATE Query
        /// </summary>
        public static SqlGenerateResult GetUpdate<TEntity>(IDbConnection connection, TEntity entity)
        {
            var context = BuildContext<TEntity>(connection);

            if (context.KeySqlProperties == null || context.KeySqlProperties.Length == 0)
            {
                throw new InvalidOperationException($"Not found the primary key in {typeof(TEntity).AssemblyQualifiedName}. Please specifies the primary key using {typeof(KeyAttribute).AssemblyQualifiedName}.");
            }

            var sql = $"UPDATE {context.Adapter.FormatTableName(context.TableName)} SET {string.Join(", ", context.NonKeySqlProperties.Select(p => context.Adapter.FormatColumnNameEqualsValue(p.ColumnName, "=", p.Name)))} WHERE {string.Join(" AND ", context.KeySqlProperties.Select(p => context.Adapter.FormatColumnNameEqualsValue(p.ColumnName, "=", p.Name)))}";

            return new SqlGenerateResult(sql.TrimEnd(), entity);
        }

        #endregion Update

        #region Delete

        /// <summary>
        /// Get SQL for DELETE Query
        /// </summary>
        public static SqlGenerateResult GetDelete<TEntity>(IDbConnection connection, object id)
        {
            var context = BuildContext<TEntity>(connection);

            return BuildDeleteSqlGenerate(context, CreateEqualityExpressionForId<TEntity>(context, id));
        }

        /// <summary>
        /// Get SQL for DELETE Query
        /// </summary>
        public static SqlGenerateResult GetDelete<TEntity>(IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            var context = BuildContext<TEntity>(connection);

            return BuildDeleteSqlGenerate(context, predicate);
        }

        private static SqlGenerateResult BuildDeleteSqlGenerate<TEntity>(SqlGenerateContext context, Expression<Func<TEntity, bool>> predicate)
        {
            var whereResult = BuildWhere(context, predicate);

            var sql = $"DELETE FROM {context.Adapter.FormatTableName(context.TableName)} {whereResult?.Where}";

            return new SqlGenerateResult(sql.TrimEnd(), whereResult?.Parameters);
        }

        #endregion

        #region  Select

        public static SqlGenerateResult GetSelectById<TEntity>(IDbConnection connection, object id)
        {
            var context = BuildContext<TEntity>(connection);
            var whereResult = BuildWhere(context, CreateEqualityExpressionForId<TEntity>(context, id));
            return new SqlGenerateResult(context.Adapter.FormatSelectAll(context.TableName, whereResult?.Where), whereResult?.Parameters);
        }

        public static SqlGenerateResult GetSelectFirst<TEntity>(IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            var context = BuildContext<TEntity>(connection);
            var whereResult = BuildWhere(context, predicate);
            return new SqlGenerateResult(context.Adapter.FormatSelectTop(1, context.TableName, whereResult?.Where), whereResult?.Parameters);
        }

        public static SqlGenerateResult GetSelectAll<TEntity>(IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            var context = BuildContext<TEntity>(connection);
            var whereResult = BuildWhere(context, predicate);
            return new SqlGenerateResult(context.Adapter.FormatSelectAll(context.TableName, whereResult?.Where), whereResult?.Parameters);
        }

        public static SqlGenerateResult GetSelectCount<TEntity>(IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            var context = BuildContext<TEntity>(connection);
            var whereResult = BuildWhere(context, predicate);
            return new SqlGenerateResult(context.Adapter.FormatSelectCount(context.TableName, whereResult?.Where), whereResult?.Parameters);
        }

        public static SqlGenerateResult GetSelectAny<TEntity>(IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            var context = BuildContext<TEntity>(connection);
            var whereResult = BuildWhere(context, predicate);
            return new SqlGenerateResult(context.Adapter.FormatSelectExists(context.TableName, whereResult?.Where), whereResult?.Parameters);
        }

        public static SqlGenerateResult GetSelectPaging<TEntity>(IDbConnection connection, Expression<Func<TEntity, bool>> predicate, int skip, int take)
        {
            var context = BuildContext<TEntity>(connection);
            var whereResult = BuildWhere(context, predicate);
            var orderBy = string.Empty;

            if (context.Adapter is SqlServerAdapter)
            {
                var keySqlProperties = context.KeySqlProperties;
                if (keySqlProperties.Length == 0) throw new InvalidOperationException("Cannot found any primary key property.");

                orderBy = $" ORDER BY {string.Join(",", keySqlProperties.Select(p => context.Adapter.FormatColumnName(p.ColumnName)))}";
            }
            else if (context.Adapter is SqlCeServerAdapter)
            {
                var keySqlProperties = context.KeySqlProperties;
                if (keySqlProperties.Length == 0) throw new InvalidOperationException("Cannot found any primary key property.");
                if (keySqlProperties.Length > 1) throw new InvalidOperationException("More than one key primary key.");
                var idColumn = keySqlProperties.First().ColumnName;

                var pagingCondition = $"{context.Adapter.FormatColumnName(idColumn)} NOT IN (SELECT TOP {skip} {context.Adapter.FormatColumnName(idColumn)} FROM {context.Adapter.FormatTableName(context.TableName)})";
                var pagingWhere = string.IsNullOrEmpty(whereResult?.Where) ? $" WHERE {pagingCondition}" : $"{whereResult.Where} AND {pagingCondition}";

                var cePagingSql = $"{context.Adapter.FormatSelectPaging(context.TableName, pagingWhere, orderBy, skip, take)};{context.Adapter.FormatSelectCount(context.TableName, whereResult?.Where)}";
                return new SqlGenerateResult(cePagingSql, whereResult?.Parameters) { Context = context };
            }

            var sql = $"{context.Adapter.FormatSelectPaging(context.TableName, whereResult?.Where, orderBy, skip, take)};{context.Adapter.FormatSelectCount(context.TableName, whereResult?.Where)};";
            return new SqlGenerateResult(sql, whereResult?.Parameters) { Context = context };
        }

        #endregion  Select

        private static SqlGenerateContext BuildContext<TEntity>(IDbConnection connection)
        {
            var entityType = typeof(TEntity);
            var connectionName = connection.GetType().Name.ToLower();

            //SqlGenerateContext context;
            var sqlGenerateContext = SqlGenerateContextDictionary.GetOrAdd($"{entityType.FullName}-{connectionName}", s =>
            {
                var context = new SqlGenerateContext();

                // Sql properties
                var entityTypeInfo = entityType.GetTypeInfo();
                var tableAliasAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();
                context.TableName = tableAliasAttribute != null ? tableAliasAttribute.Name : entityTypeInfo.Name;

                context.AllProperties = entityType.GetProperties().Where(q => q.CanWrite).ToArray();
                var props = context.AllProperties.Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

                // Filter the non stored properties
                context.SqlProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToList();

                // Adapter
                if (AdapterDictionary.TryGetValue(connectionName, out var adapter))
                {
                    context.Adapter = adapter;
                }
                else
                {
                    throw new Exception($"Not support {connectionName}.");
                }

                return context;
            });

            return sqlGenerateContext;
        }

        private static SqlWhereBuilderResult BuildWhere<TEntity>(SqlGenerateContext context, Expression<Func<TEntity, bool>> predicate)
        {
            var sb = new StringBuilder();

            // WHERE
            if (predicate != null)
            {
                IDictionary<string, object> expando = new ExpandoObject();
                var queryParameter = FillQueryProperties(context, ExpressionHelper.GetBinaryExpression(predicate.Body));

                sb.Append(" WHERE ");

                AppendWhereString(context, sb, queryParameter, expando);

                return new SqlWhereBuilderResult(sb.ToString(), expando);
            }
            return null;
        }

        private static void AppendWhereString(SqlGenerateContext context, StringBuilder sb, QueryParameter queryParameter, IDictionary<string, object> expando)
        {
            if (!queryParameter.IsGroup)
            {
                if (string.IsNullOrEmpty(queryParameter.Sql))
                {
                    var columnName = context.SqlProperties.First(x => x.Name == queryParameter.PropertyName).ColumnName;

                    if (queryParameter.PropertyValue == null)
                    {
                        sb.Append(context.Adapter.FormatFieldIsNull(queryParameter.QueryOperator == "=", columnName));
                    }
                    else
                    {
                        sb.Append(context.Adapter.FormatColumnNameEqualsValue(columnName, queryParameter.QueryOperator, queryParameter.PropertyName));
                        expando[queryParameter.PropertyName] = queryParameter.PropertyValue;
                    }
                }
                else
                {
                    sb.Append(queryParameter.Sql);
                }
            }
            else
            {
                sb.Append("(");

                AppendWhereString(context, sb, queryParameter.LeftParameter, expando);

                sb.Append($" {queryParameter.LinkingOperator} ");

                AppendWhereString(context, sb, queryParameter.RightParameter, expando);

                sb.Append(")");
            }
        }

        private static QueryParameter FillQueryProperties(SqlGenerateContext context, BinaryExpression body)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                var bodyString = body.ToString();

                // Always false
                if (string.Equals("(False == True)", bodyString, StringComparison.OrdinalIgnoreCase))
                    return new QueryParameter { Sql = "1 = 2" };

                // Always true
                if (string.Equals("(True == True)", bodyString, StringComparison.OrdinalIgnoreCase))
                    return new QueryParameter { Sql = "1 = 1" };

                // Like Or In
                if (bodyString.Contains("Contains"))
                {
                    var bodyLeft = body.Left;

                    if (body.Left.NodeType == ExpressionType.Not && body.Left is UnaryExpression unaryExpression)
                    {
                        bodyLeft = unaryExpression.Operand;
                    }

                    var argumentsPropertyInfo = bodyLeft.GetType().GetProperty("Arguments");
                    if (argumentsPropertyInfo == null) throw new Exception($"Cannot found property Arguments in Expression {bodyString}.");
                    var arguments = (ReadOnlyCollection<Expression>)argumentsPropertyInfo.GetValue(bodyLeft);

                    // Like
                    if (arguments.Count == 1)
                    {
                        var propertyName = ExpressionHelper.GetPropertyName(body);
                        if (!context.SqlProperties.Select(x => x.Name).Contains(propertyName))
                        {
                            throw new NotImplementedException("predicate can't parse");
                        }
                        var opr = body.Left.NodeType == ExpressionType.Not ? "NOT LIKE" : "LIKE";
                        return new QueryParameter(propertyName, $"%{ExpressionHelper.GetValue(arguments[0])}%", opr);
                    }
                    else if (arguments.Count == 2) // In
                    {
                        var propertyName = arguments[1].ToString().Split('.')[1];
                        if (!context.SqlProperties.Select(x => x.Name).Contains(propertyName))
                        {
                            throw new NotImplementedException("predicate can't parse");
                        }

                        var opr = body.Left.NodeType == ExpressionType.Not ? "NOT IN" : "IN";
                        return new QueryParameter(propertyName, ExpressionHelper.GetValue(arguments[0]), opr);
                    }

                    throw new NotImplementedException($"Unknown Expression {bodyString}.");
                }
                else
                {
                    // Normal
                    var propertyName = ExpressionHelper.GetPropertyName(body);

                    if (!context.SqlProperties.Select(x => x.Name).Contains(propertyName))
                    {
                        throw new NotImplementedException("predicate can't parse");
                    }

                    var propertyValue = ExpressionHelper.GetValue(body.Right);
                    var opr = ExpressionHelper.GetSqlOperator(body.NodeType);

                    // Apply Not keyword
                    if (bodyString.Contains("Not") && propertyValue is bool value)
                    {
                        propertyValue = !value;
                    }

                    return new QueryParameter(propertyName, propertyValue, opr);
                }
            }
            else
            {
                var link = ExpressionHelper.GetSqlOperator(body.NodeType);
                return new QueryParameter(link)
                {
                    LeftParameter = FillQueryProperties(context, ExpressionHelper.GetBinaryExpression(body.Left)),
                    RightParameter = FillQueryProperties(context, ExpressionHelper.GetBinaryExpression(body.Right))
                };
            }
        }

        private static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity>(SqlGenerateContext context, object id)
        {
            var keySqlProperties = context.KeySqlProperties;
            if (keySqlProperties.Length == 0) throw new InvalidOperationException("Cannot found any primary key property.");
            if (keySqlProperties.Length > 1) throw new InvalidOperationException("More than one primary key property.");

            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var leftExpression = Expression.PropertyOrField(lambdaParam, keySqlProperties.First().Name);

            var idValue = Convert.ChangeType(id, id.GetType());

            Expression<Func<object>> closure = () => idValue;
            var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

            var lambdaBody = Expression.Equal(leftExpression, rightExpression);

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}