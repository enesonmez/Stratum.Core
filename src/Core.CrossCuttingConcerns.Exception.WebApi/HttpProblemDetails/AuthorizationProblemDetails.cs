using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.CrossCuttingConcerns.Exception.WebApi.HttpProblemDetails;

public class AuthorizationProblemDetails : ProblemDetails
{
    public AuthorizationProblemDetails(string detail)
    {
        Title = "Authorization Error";
        Detail = detail;
        Status = StatusCodes.Status401Unauthorized;
        Type = "https://example.com/probs/authorization";
    }
}