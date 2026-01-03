using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Security.Swagger.Extensions;

public class BearerSecurityRequirementOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        const string openApiSecurityName = "Bearer";
        OpenApiSecurityRequirement openApiSecurityRequirement =
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = openApiSecurityName },
                        Name = openApiSecurityName,
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            };
        operation.Security.Add(openApiSecurityRequirement);
    }
}