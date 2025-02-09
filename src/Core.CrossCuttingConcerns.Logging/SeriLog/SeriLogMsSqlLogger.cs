using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogMsSqlLogger : SerilogLoggerServiceBase
{
    protected SeriLogMsSqlLogger(ILogConfiguration configuration) : base(logger: null!)
    {
        var mssqlConfiguration = configuration as MsSqlConfiguration ?? 
                                throw new ArgumentNullException(nameof(configuration));
        
        MSSqlServerSinkOptions sinkOptions =
            new() { TableName = mssqlConfiguration.TableName, AutoCreateSqlTable = mssqlConfiguration.AutoCreateSqlTable };
        
        ColumnOptions columnOptions = new();
        
        Logger = new LoggerConfiguration()
            .WriteTo.MSSqlServer(mssqlConfiguration.ConnectionString, sinkOptions, columnOptions: columnOptions)
            .CreateLogger();
    }
}