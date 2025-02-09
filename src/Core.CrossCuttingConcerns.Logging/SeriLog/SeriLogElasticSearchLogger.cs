using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogElasticSearchLogger : SerilogLoggerServiceBase
{
    public SeriLogElasticSearchLogger(ILogConfiguration configuration) : base(logger: null!)
    {
        var elasticConfiguration = configuration as ElasticSearchConfiguration ?? 
                                throw new ArgumentNullException(nameof(configuration));
        
        Logger = new LoggerConfiguration()
            .WriteTo.Elasticsearch(
                [new Uri(elasticConfiguration.ConnectionString)], opts =>
                {
                }, transport =>
                {
                    transport.Authentication(new BasicAuthentication(elasticConfiguration.Username, 
                        elasticConfiguration.Password)); 
                })
            .CreateLogger();
    }
}