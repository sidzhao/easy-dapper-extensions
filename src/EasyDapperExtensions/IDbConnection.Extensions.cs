using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using EasyDapperExtensions.Generator;
using EasyDapperExtensions.Generator.Adapter;
using Microsoft.Extensions.Logging;

namespace EasyDapperExtensions
{
    // ReSharper disable once InconsistentNaming
    public static class IDbConnectionExtensions
    {
        #region Select

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="id">The entity of primary key.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static TEntity Get<TEntity>(
            this IDbConnection connection,
            object id,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectById<TEntity>(connection, id);

            logger?.LogDbCommand(result);

            return connection.QuerySingle<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="id">The entity of primary key.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<TEntity> GetAsync<TEntity>(
            this IDbConnection connection,
            object id,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectById<TEntity>(connection, id);

            logger?.LogDbCommand(result);

            return connection.QuerySingleAsync<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static IEnumerable<TEntity> GetAll<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null, 
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectAll(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.Query<TEntity>(result.Sql, result.Parameters, transaction, buffered, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectAll(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QueryAsync<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="skip">The query skip how many rows.</param>
        /// <param name="take">The query take how many rows.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static PagedResult<TEntity> GetPaged<TEntity>(
            this IDbConnection connection,
            int skip,
            int take,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {

            var result = SqlGenerator.GetSelectPaging(connection, predicate, skip, take);

            logger?.LogDbCommand(result);

            if (result.Context.Adapter is SqlCeServerAdapter)
            {
                var sqlArray = result.Sql.Split(';');

                return new PagedResult<TEntity>
                {
                    Result = connection.Query<TEntity>(sqlArray[0], result.Parameters, transaction, commandTimeout: commandTimeout, commandType: commandType),
                    TotalCount = connection.ExecuteScalar<int>(sqlArray[1], result.Parameters, transaction, commandTimeout, commandType)
                };
            }
            else
            {
                using (var multi = connection.QueryMultiple(result.Sql, result.Parameters, transaction, commandTimeout, commandType))
                {
                    return new PagedResult<TEntity>
                    {
                        Result = multi.Read<TEntity>(),
                        TotalCount = multi.ReadFirst<int>()
                    };
                }
            }
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="skip">The query skip how many rows.</param>
        /// <param name="take">The query take how many rows.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static async Task<PagedResult<TEntity>> GetPagedAsync<TEntity>(
            this IDbConnection connection,
            int skip,
            int take,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectPaging(connection, predicate, skip, take);

            logger?.LogDbCommand(result);

            if (result.Context.Adapter is SqlCeServerAdapter)
            {
                var sqlArray = result.Sql.Split(';');

                return new PagedResult<TEntity>
                {
                    Result = await connection.QueryAsync<TEntity>(sqlArray[0], result.Parameters, transaction, commandTimeout, commandType),
                    TotalCount = await connection.ExecuteScalarAsync<int>(sqlArray[1], result.Parameters, transaction, commandTimeout, commandType)
                };
            }
            else
            {
                using (var multi = await connection.QueryMultipleAsync(result.Sql, result.Parameters, transaction, commandTimeout, commandType))
                {
                    return new PagedResult<TEntity>
                    {
                        Result = multi.Read<TEntity>(),
                        TotalCount = multi.ReadFirst<int>()
                    };
                }
            }
        }

        /// <summary>
        /// Executes a query, returning a single row value typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static TEntity GetSingle<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = SqlGenerator.GetSelectAll(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QuerySingle<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a single row value typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<TEntity> GetSingleAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = SqlGenerator.GetSelectAll(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QuerySingleAsync<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a single row value typed as <typeparamref name="TEntity" /> or null.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static TEntity GetSingleOrDefault<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = SqlGenerator.GetSelectAll(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QuerySingleOrDefault<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a single row value typed as <typeparamref name="TEntity" /> or null.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<TEntity> GetSingleOrDefaultAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = SqlGenerator.GetSelectAll(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QuerySingleOrDefaultAsync<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a first row value typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static TEntity GetFirst<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectFirst(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QueryFirst<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a first row value typed as <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<TEntity> GetFirstAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectFirst(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QueryFirstAsync<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a single row value typed as <typeparamref name="TEntity" /> or null.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static TEntity GetFirstOrDefault<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectFirst(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QuerySingleOrDefault<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a single row value typed as <typeparamref name="TEntity" /> or null.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectFirst(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.QuerySingleOrDefaultAsync<TEntity>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        #endregion

        #region Insert

        /// <summary>
        /// Executes a query, inserting a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entity">The connection to query on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static void Insert<TEntity>(
            this IDbConnection connection,
            TEntity entity,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetInsert(connection, entity);

            logger?.LogDbCommand(result);

            connection.Execute(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, inserting a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entity">The connection to query on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task InsertAsync<TEntity>(
            this IDbConnection connection,
            TEntity entity,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetInsert(connection, entity);

            logger?.LogDbCommand(result);

            return connection.ExecuteAsync(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, inserting a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entity">The connection to query on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static TPrimaryKey InsertAndGetId<TEntity, TPrimaryKey>(
            this IDbConnection connection,
            TEntity entity,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetInsertAndGetId(connection, entity);

            logger?.LogDbCommand(result);

            if (result.Context.HasIdentitySqlProperty)
            {
                if (result.Context.Adapter is SqlCeServerAdapter)
                {
                    var sqlArray = result.Sql.Split(';');

                    connection.Execute(sqlArray[0], result.Parameters, transaction, commandTimeout, commandType);

                    return connection.ExecuteScalar<TPrimaryKey>(sqlArray[1], null, transaction, commandTimeout, commandType);
                }
                else
                {
                    return connection.ExecuteScalar<TPrimaryKey>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
                }
            }
            else
            {
                connection.Execute(result.Sql, result.Parameters, transaction, commandTimeout, commandType);

                if (result.Context.KeySqlProperties.Any())
                {
                    if (result.Context.KeySqlProperties.Length == 1)
                    {
                        return (TPrimaryKey)result.Context.KeySqlProperties[0].PropertyInfo.GetValue(entity);
                    }

                    throw new InvalidOperationException("There are multiple primary keys, cannot return their values.");
                }

                throw new InvalidOperationException("Not found primary key.");
            }
        }

        /// <summary>
        /// Executes a query, inserting a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entity">The connection to query on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static async Task<TPrimaryKey> InsertAndGetIdAsync<TEntity, TPrimaryKey>(
            this IDbConnection connection,
            TEntity entity,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetInsertAndGetId(connection, entity);

            logger?.LogDbCommand(result);

            if (result.Context.HasIdentitySqlProperty)
            {
                return await connection.ExecuteScalarAsync<TPrimaryKey>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
            }
            else
            {
                await connection.ExecuteAsync(result.Sql, result.Parameters, transaction, commandTimeout, commandType);

                if (result.Context.KeySqlProperties.Any())
                {
                    if (result.Context.KeySqlProperties.Length == 1)
                    {
                        return (TPrimaryKey)result.Context.KeySqlProperties[0].PropertyInfo.GetValue(entity);
                    }

                    throw new InvalidOperationException("There are multiple primary keys, cannot return their values.");
                }

                throw new InvalidOperationException("Not found primary key.");
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Executes a query, updating a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entity">The connection to query on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static int Update<TEntity>(
            this IDbConnection connection,
            TEntity entity,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null, 
            ILogger logger = null)
        {
            var result = SqlGenerator.GetUpdate(connection, entity);

            logger?.LogDbCommand(result);

            return connection.Execute(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, updating a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entity">The connection to query on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<int> UpdateAsync<TEntity>(
            this IDbConnection connection,
            TEntity entity,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetUpdate(connection, entity);

            logger?.LogDbCommand(result);

            return connection.ExecuteAsync(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Executes a query, updating a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="id">The entity of the primary key.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static int Delete<TEntity>(
            this IDbConnection connection,
            object id,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetDelete<TEntity>(connection, id);

            logger?.LogDbCommand(result);

            return connection.Execute(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, updating a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="id">The entity of the primary key.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<int> DeleteAsync<TEntity>(
            this IDbConnection connection,
            object id,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetDelete<TEntity>(connection, id);

            logger?.LogDbCommand(result);

            return connection.ExecuteAsync(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, updating a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static int Delete<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetDelete(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.Execute(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, updating a entity.
        /// </summary>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<int> DeleteAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetDelete(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.ExecuteAsync(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        #endregion

        #region Aggregates

        /// <summary>
        /// Executes a query, returning a counting value.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static int Count<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            return connection.Count<TEntity, int>(predicate, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a counting value.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <typeparam name="TCount">The type of count result.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static TCount Count<TEntity, TCount>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectCount(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.ExecuteScalar<TCount>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a count value.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<int> CountAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            return connection.CountAsync<TEntity, int>(predicate, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a count value.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <typeparam name="TCount">The type of count result.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<TCount> CountAsync<TEntity, TCount>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectCount(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.ExecuteScalarAsync<TCount>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a bool value that used to indicate whether it's exists.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static bool Any<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectAny(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.ExecuteScalar<bool>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Executes a query, returning a bool value that used to indicate whether it's exists.
        /// </summary>
        /// <typeparam name="TEntity">The type of results to return.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="predicate">The query condition to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="logger">The logger.</param>
        public static Task<bool> AnyAsync<TEntity>(
            this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            ILogger logger = null)
        {
            var result = SqlGenerator.GetSelectAny(connection, predicate);

            logger?.LogDbCommand(result);

            return connection.ExecuteScalarAsync<bool>(result.Sql, result.Parameters, transaction, commandTimeout, commandType);
        }

        #endregion
    }
}
