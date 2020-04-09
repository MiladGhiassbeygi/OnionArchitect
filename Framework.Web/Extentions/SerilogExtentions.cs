using Framework.Web.Filters;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace Framework.Web.Extentions
{
    public static class SerilogExtensions
    {
        /// <summary>
        /// Filter log events to include only those that has specified property
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="propertyName">Name of property</param>
        /// <param name="propertyValue">Expected property value</param>
        /// <returns>LoggerConfiguration</returns>
        public static LoggerConfiguration FilterByProperty(this LoggerFilterConfiguration configuration, string propertyName, string propertyValue)
        {
            return configuration.ByIncludingOnly(Matching.WithProperty(propertyName, propertyValue)); //new ScalarValue(propertyValue)

            //var scalarValue = new ScalarValue(propertyValue);
            //return configuration.ByIncludingOnly(logEvent =>
            //{
            //    if (logEvent.Properties.TryGetValue(propertyName, out var propertyValue) && propertyValue is ScalarValue stValue)
            //        return scalarValue.Equals(stValue);
            //    return false;
            //});
        }

        /// <summary>
        /// Filter log events to include only those that has specified property
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="propertyName">Name of property</param>
        /// <returns>LoggerConfiguration</returns>
        public static LoggerConfiguration FilterByHasProperty(this LoggerFilterConfiguration configuration, string propertyName)
        {
            return configuration.ByIncludingOnly(Matching.WithProperty(propertyName));

            //return configuration.ByIncludingOnly(logEvent => logEvent.Properties.ContainsKey(propertyName));
        }

        /// <summary>
        /// Filter log events to include only those that has specified property
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="propertyName">Name of property</param>
        /// <param name="propertyValue">Expected property value</param>
        /// <returns>LoggerConfiguration</returns>
        public static LoggerConfiguration FilterOnlyLogRequests(this LoggerFilterConfiguration configuration)
        {
            return configuration.ByIncludingOnly(logEvent =>
            {
                var propertyName = nameof(LogRequestAttribute);
                var contains = logEvent.Properties.ContainsKey(propertyName); //Matching.WithProperty(propertyName)(logEvent);
                logEvent.RemovePropertyIfPresent(propertyName);
                return contains;
            });
        }
        public static LoggerConfiguration WriteLogRequestsToSqlServer(this LoggerConfiguration loggerConfiguration, string connectionString, string tableName = "RequestLogs")
        {
           
            loggerConfiguration.WriteTo.Logger(subLoggerConfigurations =>
            {
                var options = new ColumnOptions();
                options.Store.Remove(StandardColumn.Properties);
                options.Store.Add(StandardColumn.LogEvent);
                options.LogEvent.ExcludeAdditionalProperties = true;
                options.LogEvent.ExcludeStandardColumns = true;
                //options.TimeStamp.ConvertToUtc = true;
                options.AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn {ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 500, AllowNull = false},
                    new SqlColumn {ColumnName = "RequestMethod", DataType = SqlDbType.NVarChar, DataLength = 10, AllowNull = false},
                    new SqlColumn {ColumnName = "StatusCode", DataType = SqlDbType.Int, AllowNull = false},
                    new SqlColumn {ColumnName = "Elapsed", DataType = SqlDbType.Float, AllowNull = false},
                    new SqlColumn {ColumnName = "IpAddress", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = false},
                    new SqlColumn {ColumnName = "UserKey", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true},
                };

                subLoggerConfigurations
                    .Filter.FilterOnlyLogRequests()
                    //.Enrich.FromLogContext()
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        tableName: tableName,
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        columnOptions: options,
                        autoCreateSqlTable: true);
            });

            return loggerConfiguration;
        }
        public static LoggerConfiguration WriteLogsToSqlServer(this LoggerConfiguration loggerConfiguration, string connectionString, string tableName = "Logs")
        {
            loggerConfiguration
                            .WriteTo.Logger(subLoggerConfiguration =>
                            {
                                var options = new ColumnOptions();
                                options.Store.Remove(StandardColumn.Properties);
                                options.Store.Add(StandardColumn.LogEvent);

                                subLoggerConfiguration
                                    .Filter.ByExcluding(Matching.WithProperty(nameof(LogRequestAttribute)))
                                    .WriteTo.MSSqlServer(
                                        connectionString: connectionString,
                                        tableName: tableName,
                                        restrictedToMinimumLevel: LogEventLevel.Warning,
                                        columnOptions: options,
                                        autoCreateSqlTable: true);
                            });
            return loggerConfiguration;
        }
    }
}
