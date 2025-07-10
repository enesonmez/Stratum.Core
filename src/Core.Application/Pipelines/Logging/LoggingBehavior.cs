using System.Text.Json;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Core.Application.Pipelines.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ILoggableRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;

    public LoggingBehavior(ILogger logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response = await next();

        List<LogParameter> logParameters =
        [
            new() { Type = request.GetType().Name, Value = request },
            new() { Type = response?.GetType().Name ?? "", Value = response ?? new object() }
        ];

        LogDetail logDetail =
            new()
            {
                MethodName = typeof(TRequest).Name,
                Parameters = logParameters,
                User = _httpContextAccessor.HttpContext.User.Identity?.Name ?? "?"
            };
        
        using (LogContext.PushProperty(nameof(LogType.Request), null))
        {
            _logger.Information(JsonSerializer.Serialize(logDetail));
        }
        

        return response;
    }
}