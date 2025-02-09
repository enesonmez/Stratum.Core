using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SerLogGraylogLogger : SerilogLoggerServiceBase
{
    public SerLogGraylogLogger(ILogConfiguration configuration) : base(logger: null!)
    {
        var graylogConfiguration = configuration as GraylogConfiguration ?? 
                                throw new ArgumentNullException(nameof(configuration));
        
        Logger = new LoggerConfiguration()
            .WriteTo.Graylog(
                new GraylogSinkOptions
                {
                    HostnameOrAddress = graylogConfiguration.HostnameOrAddress,
                    Port = graylogConfiguration.Port,
                    TransportType = TransportType.Udp
                }
            )
            .CreateLogger();
    }
}