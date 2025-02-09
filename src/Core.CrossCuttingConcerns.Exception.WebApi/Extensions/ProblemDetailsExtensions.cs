using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Core.CrossCuttingConcerns.Exception.WebApi.Extensions;

public static class ProblemDetailsExtensions
{
    public static string ToJson<TProblemDetail>(this TProblemDetail problemDetails)
        where TProblemDetail : ProblemDetails
    {
        return JsonSerializer.Serialize(problemDetails);
    }
}