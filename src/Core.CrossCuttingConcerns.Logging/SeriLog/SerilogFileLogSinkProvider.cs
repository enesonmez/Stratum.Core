using Core.Constants;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Core.CrossCuttingConcerns.Logging.Enums;
using Serilog;
using Serilog.Configuration;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SerilogFileLogSinkProvider : ISeriLogSinkProvider
{
    private readonly FileLogConfiguration _fileConfiguration;

    public SerilogFileLogSinkProvider(ILogConfiguration configuration)
    {
        _fileConfiguration = configuration as FileLogConfiguration ??
                             throw new ArgumentNullException(nameof(configuration));
    }

    public LoggerConfiguration Configure(LoggerConfiguration loggerConfig)
    {
        return loggerConfig
            .WriteTo.Async(a => a.FileCustom(
                $"{Directory.GetCurrentDirectory() + _fileConfiguration.FolderPath + nameof(LogType.General) +
                   CommonConstants.DotString + FileExtensions.Txt}"))
            .WriteTo.Async(a => a.Logger(lc => lc
                .Filter.ByIncludingOnly(le => le.Level == Serilog.Events.LogEventLevel.Error)
                .WriteTo.FileCustom(
                    $"{Directory.GetCurrentDirectory() + _fileConfiguration.FolderPath + nameof(LogType.Error) +
                       CommonConstants.DotString + FileExtensions.Txt}")))
            .WriteTo.Async(a => a.Logger(lc => lc
                .Filter.ByIncludingOnly(le => le.Properties.ContainsKey(nameof(LogType.Request)))
                .WriteTo.FileCustom(
                    $"{Directory.GetCurrentDirectory() + _fileConfiguration.FolderPath + nameof(LogType.Request) +
                       CommonConstants.DotString + FileExtensions.Txt}")))
            .WriteTo.Async(a => a.Logger(lc => lc
                .Filter.ByIncludingOnly(le => le.Properties.ContainsKey(nameof(LogType.Performance)))
                .WriteTo.FileCustom(
                    $"{Directory.GetCurrentDirectory() + _fileConfiguration.FolderPath + nameof(LogType.Performance) +
                       CommonConstants.DotString + FileExtensions.Txt}")));
    }
}

public static class LoggerConfigurationExtentions
{
    public static LoggerConfiguration FileCustom(this LoggerSinkConfiguration sinkConfiguration, string path)
    {
        return sinkConfiguration.File(
            path: path,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: null,
            fileSizeLimitBytes: 5000000,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
        );
    }
}