using Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Core.CrossCuttingConcerns.Logging.Configurations;

public class RabbitMQConfiguration : ILogConfiguration
{
    public int Port { get; set; }
    public string? Exchange { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ExchangeType { get; set; }
    public string? RouteKey { get; set; }
    public List<string> Hostnames { get; set; }

    public RabbitMQConfiguration()
    {
        Hostnames = [];
    }
}