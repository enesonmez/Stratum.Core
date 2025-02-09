using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using MongoDB.Driver;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogMongoDbLogger : SerilogLoggerServiceBase
{
    public SeriLogMongoDbLogger(ILogConfiguration configuration) : base(logger: null!)
    {
        var mongoConfiguration = configuration as MongoDbConfiguration ?? 
                                throw new ArgumentNullException(nameof(configuration));
        
        Logger = new LoggerConfiguration()
            .WriteTo.MongoDBBson(cfg =>
            {
                MongoClient client = new(mongoConfiguration.ConnectionString);
                IMongoDatabase? mongoDbInstance = client.GetDatabase(mongoConfiguration.Collection);
                cfg.SetMongoDatabase(mongoDbInstance);
            })
            .CreateLogger();
    }
}