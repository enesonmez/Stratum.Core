using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Core.Security.Encryption;
using Core.Security.Abstractions.Entities;
using Core.Security.Abstractions.Jwt;
using Core.Security.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Core.Security.Jwt;

public class JwtHelper<TUserId, TOperationClaimId, TRefreshTokenId> :
    ITokenHelper<TUserId, TOperationClaimId, TRefreshTokenId>
{
    private readonly TokenOptions _tokenOptions;

    public JwtHelper(TokenOptions tokenOptions)
    {
        _tokenOptions = tokenOptions;
    }

    public virtual AccessToken CreateToken(User<TUserId> user, IList<OperationClaim<TOperationClaimId>> operationClaims)
    {
        DateTime accessTokenExpirationDate = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);
        SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
        JwtSecurityToken jwt = CreateJwtSecurityToken(
            _tokenOptions,
            user,
            signingCredentials,
            operationClaims,
            accessTokenExpirationDate
        );

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string token = jwtSecurityTokenHandler.WriteToken(jwt);

        return new AccessToken(token, accessTokenExpirationDate);
    }

    public RefreshToken<TRefreshTokenId, TUserId> CreateRefreshToken(User<TUserId> user, string ipAddress)
    {
        return new RefreshToken<TRefreshTokenId, TUserId>()
        {
            UserId = user.Id,
            Token = RandomRefreshToken(),
            ExpirationDate = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenTtl),
            CreatedByIp = ipAddress
        };
    }

    public virtual JwtSecurityToken CreateJwtSecurityToken(
        TokenOptions tokenOptions,
        User<TUserId> user,
        SigningCredentials signingCredentials,
        IList<OperationClaim<TOperationClaimId>> operationClaims,
        DateTime accessTokenExpiration
    )
    {
        return new JwtSecurityToken(
            tokenOptions.Issuer,
            tokenOptions.Audience,
            expires: accessTokenExpiration,
            notBefore: DateTime.UtcNow,
            claims: SetClaims(user, operationClaims),
            signingCredentials: signingCredentials
        );
    }

    protected virtual IEnumerable<Claim> SetClaims(User<TUserId> user,
        IList<OperationClaim<TOperationClaimId>> operationClaims)
    {
        List<Claim> claims = [];
        claims.AddNameIdentifier(user!.Id!.ToString()!);
        claims.AddEmail(user.Email);
        claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
        return claims.ToImmutableList();
    }

    private string RandomRefreshToken()
    {
        byte[] numberByte = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(numberByte);
        return Convert.ToBase64String(numberByte);
    }
}