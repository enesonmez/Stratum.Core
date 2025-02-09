using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.SeriLog;

public class SerilogFileLogger : SerilogLoggerServiceBase
{
    public SerilogFileLogger(ILogConfiguration configuration) : base(logger: null!)
    {
        var fileConfiguration = configuration as FileLogConfiguration ?? 
                             throw new ArgumentNullException(nameof(configuration));
        
        Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: $"{Directory.GetCurrentDirectory() + fileConfiguration.FolderPath}.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                fileSizeLimitBytes: 5000000,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
            )
            .CreateLogger();
    }
}