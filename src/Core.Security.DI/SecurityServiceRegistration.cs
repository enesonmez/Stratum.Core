using Core.Security.Abstractions.Hashing;
using Core.Security.Abstractions.Jwt;
using Core.Security.Hashing;
using Core.Security.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Security.DI;

public static class SecurityServiceRegistration
{
    public static IServiceCollection AddSecurityServices<TUserId, TOperationClaimId, TRefreshTokenId>(
        this IServiceCollection services,
        TokenOptions tokenOptions
    )
    {
        services.AddScoped<
            ITokenHelper<TUserId, TOperationClaimId, TRefreshTokenId>,
            JwtHelper<TUserId, TOperationClaimId, TRefreshTokenId>
        >(_ => new JwtHelper<TUserId, TOperationClaimId, TRefreshTokenId>(tokenOptions));
        
        services.AddScoped<IHashingService, HashingService>();
        
        return services;
    }
}