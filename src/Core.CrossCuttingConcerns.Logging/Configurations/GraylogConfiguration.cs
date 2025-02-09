using Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Core.CrossCuttingConcerns.Logging.Configurations;

public class GraylogConfiguration : ILogConfiguration
{
    public string? HostnameOrAddress { get; set; }
    public int Port { get; set; }
}