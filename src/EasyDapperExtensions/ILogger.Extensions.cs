using System.Dynamic;
using System.Linq;
using System.Reflection;
using EasyDapperExtensions.Generator;
using Microsoft.Extensions.Logging;

namespace EasyDapperExtensions
{
    public static class EasyDapperExtensionsLoggerExtensions
    {
        public static void LogDbCommand(this ILogger logger, SqlGenerateResult result)
        {
            logger?.LogInformationDbCommand(result.Sql, result.Parameters);
        }

        public static void LogDebugDbCommand(this ILogger logger, string sql, object parameters)
        {
            if (logger != null && logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"Executed DbCommand{FormatSqlParameters(parameters)}\r\n{sql}");
            }
        }

        public static void LogInformationDbCommand(this ILogger logger, string sql, object parameters)
        {
            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation($"Executed DbCommand{FormatSqlParameters(parameters)}\r\n{sql}");
            }
        }

        public static void LogWarningDbCommand(this ILogger logger, string sql, object parameters)
        {
            if (logger != null && logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning($"Executed DbCommand{FormatSqlParameters(parameters)}\r\n{sql}");
            }
        }

        public static void LogErrorDbCommand(this ILogger logger, string sql, object parameters)
        {
            if (logger != null && logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError($"Executed DbCommand{FormatSqlParameters(parameters)}\r\n{sql}");
            }
        }

        private static string FormatSqlParameters(object parameters)
        {
            if (parameters == null)
                return string.Empty;

            if (parameters is ExpandoObject expando)
            {
                return $" [Parameters: {string.Join(", ", expando.Select(p => p.Key + " = " + p.Value))}]";
            }
            else
            {
                var properties = parameters.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                return $" [Parameters: {string.Join(", ", properties.Select(p => p.Name + " = " + p.GetValue(parameters)))}]";
            }
        }
    }
}
