using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SerLogGraylogsinkProvider : ISeriLogSinkProvider
{
    private readonly GraylogConfiguration _graylogConfiguration;

    public SerLogGraylogsinkProvider(ILogConfiguration configuration)
    {
        _graylogConfiguration = configuration as GraylogConfiguration ??
                                throw new ArgumentNullException(nameof(configuration));
    }

    public LoggerConfiguration Configure(LoggerConfiguration loggerConfig)
    {
        return loggerConfig
            .WriteTo.Graylog(
                new GraylogSinkOptions
                {
                    HostnameOrAddress = _graylogConfiguration.HostnameOrAddress,
                    Port = _graylogConfiguration.Port,
                    TransportType = TransportType.Udp
                }
            );
    }
}