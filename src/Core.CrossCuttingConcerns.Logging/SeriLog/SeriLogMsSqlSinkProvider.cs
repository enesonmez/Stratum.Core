using System.Collections.ObjectModel;
using System.Data;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogMsSqlSinkProvider : ISeriLogSinkProvider
{
    private readonly MsSqlConfiguration _mssqlConfiguration;

    public SeriLogMsSqlSinkProvider(ILogConfiguration configuration)
    {
        _mssqlConfiguration = configuration as MsSqlConfiguration ??
                              throw new ArgumentNullException(nameof(configuration));
    }

    public LoggerConfiguration Configure(LoggerConfiguration loggerConfig)
    {
        MSSqlServerSinkOptions sinkOptions =
            new()
            {
                TableName = _mssqlConfiguration.TableName, AutoCreateSqlTable = _mssqlConfiguration.AutoCreateSqlTable
            };

        ColumnOptions columnOptions = new();

        columnOptions.AdditionalColumns = new Collection<SqlColumn>(
            _mssqlConfiguration.AdditionalColumns.Select(col =>
            {
                var sqlType = Enum.Parse<SqlDbType>(col.SqlDbType, ignoreCase: true);
                return new SqlColumn(col.Name, sqlType, dataLength: col.Length ?? 0);
            }).ToList()
        );

        return loggerConfig
            .WriteTo.MSSqlServer(_mssqlConfiguration.ConnectionString, sinkOptions, columnOptions: columnOptions);
    }
}