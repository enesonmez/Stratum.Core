using System.Collections.Immutable;
using Core.Localization.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.Localization.WebApi;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;

    public LocalizationMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext httpContext, ILocalizationService localizationService)
    {
        IList<StringWithQualityHeaderValue> acceptLanguages = httpContext.Request.GetTypedHeaders().AcceptLanguage;
        if (acceptLanguages != null && acceptLanguages.Count > 0)
            localizationService.AcceptLocales = acceptLanguages
                .OrderByDescending(x => x.Quality ?? 1)
                .Select(x => x.Value.ToString())
                .ToImmutableArray();
        
        await _next(httpContext);
    }
}