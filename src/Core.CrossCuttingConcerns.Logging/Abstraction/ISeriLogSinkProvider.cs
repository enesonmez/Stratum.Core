using Serilog;

namespace Core.CrossCuttingConcerns.Logging.Abstraction;

public interface ISeriLogSinkProvider : ILogSinkProvider
{
    LoggerConfiguration Configure(LoggerConfiguration loggerConfig);
}