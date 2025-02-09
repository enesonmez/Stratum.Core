using Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Core.CrossCuttingConcerns.Logging.Configurations;

public class MongoDbConfiguration : ILogConfiguration
{
    public string ConnectionString { get; set; }
    public string Collection { get; set; }

    public MongoDbConfiguration()
    {
        ConnectionString = string.Empty;
        Collection = string.Empty;
    }

    public MongoDbConfiguration(string connectionString, string collection)
    {
        ConnectionString = connectionString;
        Collection = collection;
    }
}