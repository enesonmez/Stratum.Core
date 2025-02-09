using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogRabbitMQLogger : SerilogLoggerServiceBase
{
    public SeriLogRabbitMQLogger(ILogConfiguration configuration) : base(logger: null!)
    {
        var rabbitConfiguration = configuration as RabbitMQConfiguration ?? 
                                throw new ArgumentNullException(nameof(configuration));
        
        Logger = new LoggerConfiguration()
            .WriteTo.RabbitMQ(
                (clientConfiguration, sinkConfiguration) =>
                {
                    clientConfiguration.Port = rabbitConfiguration.Port;
                    clientConfiguration.Username = rabbitConfiguration.Username ?? string.Empty;
                    clientConfiguration.Password = rabbitConfiguration.Password ?? string.Empty;
                    clientConfiguration.Exchange = rabbitConfiguration.Exchange ?? string.Empty;
                    clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
                    clientConfiguration.ExchangeType = rabbitConfiguration.ExchangeType ?? string.Empty;
                    clientConfiguration.RoutingKey = rabbitConfiguration.RouteKey ?? string.Empty;
                    rabbitConfiguration.Hostnames.ForEach(clientConfiguration.Hostnames.Add);
                    
                    sinkConfiguration.TextFormatter = new JsonFormatter();
                }
            )
            .CreateLogger();
    }
}