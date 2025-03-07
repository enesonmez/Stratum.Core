using Core.CrossCuttingConcerns.Logging.Abstraction;
using PackageSerilog = Serilog;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SerilogLoggerServiceBase : ILogger
{
    protected PackageSerilog.ILogger? Logger { get; set; }
    
    protected SerilogLoggerServiceBase(PackageSerilog.ILogger logger)
    {
        Logger = logger;
    }
    
    public void Trace(string message)
    {
        Logger?.Verbose(message);
    }

    public void Critical(string message)
    {
        Logger?.Fatal(message);
    }

    public void Information(string message)
    {
        Logger?.Information(message);
    }

    public void Warning(string message)
    {
        Logger?.Warning(message);
    }

    public void Debug(string message)
    {
        Logger?.Debug(message);
    }

    public void Error(string message)
    {
        Logger?.Error(message);
    }
}