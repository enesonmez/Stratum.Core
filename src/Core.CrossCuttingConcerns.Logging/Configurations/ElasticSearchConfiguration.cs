using Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Core.CrossCuttingConcerns.Logging.Configurations;

public class ElasticSearchConfiguration : ILogConfiguration
{
    public string ConnectionString { get; set; }
    public string Username { get; set; }      
    public string Password { get; set; }      

    public ElasticSearchConfiguration()
    {
        ConnectionString = string.Empty;
        Username = string.Empty;
        Password = string.Empty;
    }
}