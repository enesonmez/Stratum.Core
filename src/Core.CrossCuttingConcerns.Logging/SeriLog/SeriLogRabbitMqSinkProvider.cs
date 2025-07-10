using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogRabbitMqSinkProvider : ISeriLogSinkProvider
{
    private readonly RabbitMQConfiguration _rabbitConfiguration;

    public SeriLogRabbitMqSinkProvider(ILogConfiguration configuration)
    {
        _rabbitConfiguration = configuration as RabbitMQConfiguration ??
                               throw new ArgumentNullException(nameof(configuration));
    }

    public LoggerConfiguration Configure(LoggerConfiguration loggerConfig)
    {
        return loggerConfig
            .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
                {
                    clientConfiguration.Port = _rabbitConfiguration.Port;
                    clientConfiguration.Username = _rabbitConfiguration.Username ?? string.Empty;
                    clientConfiguration.Password = _rabbitConfiguration.Password ?? string.Empty;
                    clientConfiguration.Exchange = _rabbitConfiguration.Exchange ?? string.Empty;
                    clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
                    clientConfiguration.ExchangeType = _rabbitConfiguration.ExchangeType ?? string.Empty;
                    clientConfiguration.RoutingKey = _rabbitConfiguration.RouteKey ?? string.Empty;
                    _rabbitConfiguration.Hostnames.ForEach(clientConfiguration.Hostnames.Add);

                    sinkConfiguration.TextFormatter = new JsonFormatter();
                }
            );
    }
}