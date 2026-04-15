using System.IdentityModel.Tokens.Jwt;
using EmailValidation;
using Enterprise.Shared.Context;
using Enterprise.Shared.IdentityProviders.Configurations;
using Enterprise.Shared.Security.Token;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Shared.IdentityProviders.Cognito;

public interface ICognitoTokenService : ITokenService;

public class CognitoTokenService(
    IdentityProvidersConfiguration identityProvidersConfiguration,
    IContext context,
    IMemoryCache memoryCache,
    TimeProvider timeProvider,
    ILogger<CognitoTokenService> logger)
    : ICognitoTokenService
{
    private readonly IReadOnlyCollection<string> _audiences = identityProvidersConfiguration.Cognito!.Audiences is null
        ? []
        : identityProvidersConfiguration.Cognito.Audiences.Split(",").Select(audience => audience.Trim())
            .Where(audience => !string.IsNullOrWhiteSpace(audience)).ToList();

    private readonly Configurations.Cognito _cognitoConfiguration = identityProvidersConfiguration.Cognito!;

    public async Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Verifying Cognito token. TokenLength={TokenLength}", token.Length);

            var jws = await memoryCache.GetOrCreateAsync<Jws>("cognito-public-keys", async cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = timeProvider.GetUtcNow().AddMinutes(15);

                return await _cognitoConfiguration.JwksUri.GetJsonAsync<Jws>(cancellationToken: cancellationToken);
            });

            ArgumentNullException.ThrowIfNull(jws);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var issuer = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if (issuer is not null && _cognitoConfiguration.Issuer != issuer)
            {
                logger.LogWarning("Cognito token issuer mismatch. Issuer={Issuer}", issuer);
                return;
            }

            await new JsonWebTokenHandler().ValidateTokenAsync(
                jwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _cognitoConfiguration.Issuer,
                    ValidateAudience = true,
                    ValidAudiences = _audiences,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = jws.Keys,
                    ValidateLifetime = true
                });

            var value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            context.SetVerifiableToken(value);
            logger.LogDebug("Cognito token subject resolved");

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
            if (value is not null)
            {
                context.SetName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "given_name")?.Value;
            if (value is not null)
            {
                context.SetGivenName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "family_name")?.Value;
            if (value is not null)
            {
                context.SetFamilyName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "picture")?.Value;
            if (value is not null)
            {
                context.SetPhotoUrl(value);
                context.SetPhotoUrl24(value);
                context.SetPhotoUrl32(value);
                context.SetPhotoUrl48(value);
                context.SetPhotoUrl72(value);
                context.SetPhotoUrl192(value);
                context.SetPhotoUrl512(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            if (value is not null && EmailValidator.Validate(value))
            {
                context.SetEmail(value);

                value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email_verified")?.Value;
                if (value is not null)
                {
                    context.SetEmailVerified(bool.Parse(value));
                }
            }

            logger.LogInformation("Cognito token verified successfully");
        }
        catch
        {
            // ignored
        }
    }
}
