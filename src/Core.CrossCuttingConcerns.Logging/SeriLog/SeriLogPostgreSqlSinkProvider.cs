using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogPostgreSqlSinkProvider : ISeriLogSinkProvider
{
    private readonly PostgreSqlConfiguration _postgreSqlConfiguration;

    public SeriLogPostgreSqlSinkProvider(ILogConfiguration configuration)
    {
        _postgreSqlConfiguration = configuration as PostgreSqlConfiguration ??
                                   throw new ArgumentNullException(nameof(configuration));
    }

    public LoggerConfiguration Configure(LoggerConfiguration loggerConfig)
    {
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

        return loggerConfig
            .WriteTo.PostgreSQL(
                _postgreSqlConfiguration.ConnectionString,
                _postgreSqlConfiguration.TableName,
                columnWriters,
                needAutoCreateTable: _postgreSqlConfiguration.NeedAutoCreateTable
            );
    }
}