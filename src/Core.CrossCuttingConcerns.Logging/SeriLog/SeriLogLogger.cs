using Core.CrossCuttingConcerns.Logging.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using PackageSerilog = Serilog;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SeriLogLogger : ILogger
{
    private PackageSerilog.ILogger Logger { get; set; }

    public SeriLogLogger(IServiceProvider serviceProvider)
    {
        var loggerConfig = new PackageSerilog.LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext();

        var sinkProviders = serviceProvider.GetServices<ISeriLogSinkProvider>();
        foreach (var provider in sinkProviders)
        {
            loggerConfig = provider.Configure(loggerConfig);
        }

        Logger = loggerConfig.CreateLogger();
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