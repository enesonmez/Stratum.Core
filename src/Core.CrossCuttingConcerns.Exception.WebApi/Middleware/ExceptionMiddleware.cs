using System.Net.Mime;
using System.Text.Json;
using Core.CrossCuttingConcerns.Exception.WebApi.Handlers;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Enums;
using Core.Localization.Abstraction;
using Microsoft.AspNetCore.Http;

namespace Core.CrossCuttingConcerns.Exception.WebApi.Middleware;

public class ExceptionMiddleware
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HttpExceptionHandler _httpExceptionHandler;
    private readonly RequestDelegate _next;
    private readonly ILogger _loggerService;

    public ExceptionMiddleware(RequestDelegate next, IHttpContextAccessor contextAccessor, ILogger loggerService, ILocalizationService localizationService)
    {
        _next = next;
        _contextAccessor = contextAccessor;
        _loggerService = loggerService;
        _httpExceptionHandler = new HttpExceptionHandler(localizationService);
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering(bufferThreshold: 1024 * 30);
            await _next(context);
        }
        catch (System.Exception exception)
        {
            await LogException(context, exception);
            await HandleExceptionAsync(context.Response, exception);
        }
    }
    
    protected virtual Task HandleExceptionAsync(HttpResponse response, dynamic exception)
    {
        response.ContentType = MediaTypeNames.Application.Json;
        _httpExceptionHandler.Response = response;

        return _httpExceptionHandler.HandleException(exception);
    }
    
    protected virtual async Task LogException(HttpContext context, System.Exception exception)
    {
        string requestBody = "Body could not be read.";
    
        try 
        {
            if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync(); 
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
            }
        }
        catch
        {
            requestBody = "[Body Read Error]"; 
        }
        
        var loggableRequest = new 
        {
            Method = context.Request.Method,
            Path = context.Request.Path.Value,
            QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
            Body = requestBody
        };
        
        List<LogParameter> logParameters = 
        [
            new LogParameter 
            { 
                Type = loggableRequest.Path, 
                Value = JsonSerializer.Serialize(loggableRequest) 
            }
        ];
        
        LogDetailWithException logDetail =
            new()
            {
                ExceptionMessage = exception.ToString(),
                MethodName = context.Request.GetType().Name,
                Parameters = logParameters,
                User = _contextAccessor.HttpContext?.User.Identity?.Name ?? "?"
            };

        using (_loggerService.PushProperty(nameof(LogType.Error), null))
        {
            _loggerService.Error("{@LogDetail}", logDetail);
        }
    }
}