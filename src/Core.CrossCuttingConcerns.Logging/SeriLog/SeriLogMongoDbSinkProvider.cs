using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using MongoDB.Driver;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogMongoDbSinkProvider : ISeriLogSinkProvider
{
    private readonly MongoDbConfiguration _mongoConfiguration;

    public SeriLogMongoDbSinkProvider(ILogConfiguration configuration)
    {
        _mongoConfiguration = configuration as MongoDbConfiguration ??
                              throw new ArgumentNullException(nameof(configuration));
    }

    public LoggerConfiguration Configure(LoggerConfiguration loggerConfig)
    {
        return loggerConfig
            .WriteTo.MongoDBBson(cfg =>
            {
                MongoClient client = new(_mongoConfiguration.ConnectionString);
                IMongoDatabase? mongoDbInstance = client.GetDatabase(_mongoConfiguration.Collection);
                cfg.SetMongoDatabase(mongoDbInstance);
            });
    }
}