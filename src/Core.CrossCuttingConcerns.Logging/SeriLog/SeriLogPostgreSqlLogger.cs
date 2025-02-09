using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogPostgreSqlLogger : SerilogLoggerServiceBase
{
    public SeriLogPostgreSqlLogger(ILogConfiguration configuration) : base(logger: null!)
    {
        var postgreSqlConfiguration = configuration as PostgreSqlConfiguration ?? 
                                      throw new ArgumentNullException(nameof(configuration));
        
        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter() },
            { "message_template", new MessageTemplateColumnWriter() },
            { "level", new LevelColumnWriter(renderAsText: true, NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter() },
            { "exception", new ExceptionColumnWriter() },
            { "properties", new LogEventSerializedColumnWriter() },
            { "props_test", new PropertiesColumnWriter() },
            { "machine_name", new SinglePropertyColumnWriter(propertyName: "MachineName", format: "l") }
        };
        
        Logger = new LoggerConfiguration()
            .WriteTo.PostgreSQL(
                postgreSqlConfiguration.ConnectionString,
                postgreSqlConfiguration.TableName,
                columnWriters,
                needAutoCreateTable: postgreSqlConfiguration.NeedAutoCreateTable
            )
            .CreateLogger();
    }
}