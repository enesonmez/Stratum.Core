using Core.CrossCuttingConcerns.Exception.Handlers;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.CrossCuttingConcerns.Exception.WebApi.Extensions;
using Core.CrossCuttingConcerns.Exception.WebApi.HttpProblemDetails;
using Core.Localization.Abstraction;
using Microsoft.AspNetCore.Http;

namespace Core.CrossCuttingConcerns.Exception.WebApi.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
    
    private HttpResponse? _response;
    private readonly ILocalizationService _localizationService;
    
    public HttpExceptionHandler(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }
    
    public HttpResponse Response
    {
        #pragma warning disable S112 // General or reserved exceptions should never be thrown
        get => _response ?? throw new NullReferenceException(nameof(_response));
        #pragma warning restore S112 // General or reserved exceptions should never be thrown
        set => _response = value;
    }
    
    public override async Task HandleException(BusinessException businessException)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string localizedMessage = await _localizationService.GetLocalizedAsync(
            key: businessException.Message, 
            keySection: businessException.KeySection 
        );
        string details = new BusinessProblemDetails(localizedMessage).ToJson();
        await Response.WriteAsync(details);
    }

    public override Task HandleException(ValidationException validationException)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new ValidationProblemDetails(validationException.Errors).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(AuthorizationException authorizationException)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        string details = new AuthorizationProblemDetails(authorizationException.Message).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(NotFoundException notFoundException)
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        string details = new NotFoundProblemDetails(notFoundException.Message).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(System.Exception exception)
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        string details = new InternalServerErrorProblemDetails(exception.Message).ToJson();
        return Response.WriteAsync(details);
    }
}