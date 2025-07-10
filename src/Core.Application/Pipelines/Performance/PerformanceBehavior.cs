using System.Diagnostics;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Enums;
using MediatR;
using Serilog.Context;

namespace Core.Application.Pipelines.Performance;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IPerformanceRequest
{
    private readonly ILogger _logger;
    private readonly Stopwatch _stopwatch;

    public PerformanceBehavior(ILogger logger)
    {
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;
        TResponse response;
        
        try
        {
            _stopwatch.Start();
            response = await next();
        }
        finally
        {
            if (_stopwatch.Elapsed.TotalMilliseconds > request.Interval)
            {
                string message = $"Performance -> {requestName} {_stopwatch.Elapsed.TotalMilliseconds} ms";

                using (LogContext.PushProperty(nameof(LogType.Performance), null))
                {
                    _logger.Information(message);
                }
            }
            _stopwatch.Stop();
        }
        
        return response;
    }
}