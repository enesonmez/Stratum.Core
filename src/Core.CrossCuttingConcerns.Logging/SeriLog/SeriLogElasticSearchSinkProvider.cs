using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogElasticSearchSinkProvider : ISeriLogSinkProvider
{
    private readonly ElasticSearchConfiguration _elasticConfiguration;

    public SeriLogElasticSearchSinkProvider(ILogConfiguration configuration)
    {
        _elasticConfiguration = configuration as ElasticSearchConfiguration ??
                                throw new ArgumentNullException(nameof(configuration));
    }

    public LoggerConfiguration Configure(LoggerConfiguration loggerConfig)
    {
        return loggerConfig
            .WriteTo.Elasticsearch(
                [new Uri(_elasticConfiguration.ConnectionString)], opts => { }, transport =>
                {
                    transport.Authentication(new BasicAuthentication(_elasticConfiguration.Username,
                        _elasticConfiguration.Password));
                });
    }
}