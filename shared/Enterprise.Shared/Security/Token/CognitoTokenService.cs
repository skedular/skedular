using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Shared.Security.Token;

public interface ICognitoTokenService : ITokenService;

public class CognitoTokenService : ICognitoTokenService
{
    private readonly IReadOnlyCollection<string> _audiences;
    private readonly Cognito _cognitoConfiguration;
    private readonly IMemoryCache _memoryCache;

    public CognitoTokenService(ApplicationConfiguration applicationConfiguration, IMemoryCache memoryCache)
    {
        ArgumentNullException.ThrowIfNull(applicationConfiguration.IdentityProviders.Cognito);
        _cognitoConfiguration = applicationConfiguration.IdentityProviders.Cognito;

        _audiences = _cognitoConfiguration.Audiences is null
            ? []
            : _cognitoConfiguration.Audiences.Split(",").Select(audience => audience.Trim())
                .Where(audience => !string.IsNullOrWhiteSpace(audience)).ToList();

        _memoryCache = memoryCache;
    }

    public async Task<PropertyBag?> VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var jws = await _memoryCache.GetOrCreateAsync<Jws>("cognito-public-keys", async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(15);

                return await _cognitoConfiguration.JwksUri.GetJsonAsync<Jws>(cancellationToken: cancellationToken);
            });

            ArgumentNullException.ThrowIfNull(jws);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if (value is not null && _cognitoConfiguration.Issuer != value)
            {
                return null;
            }

            await new JsonWebTokenHandler().ValidateTokenAsync(jwtToken,
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

            var propertyBag = new PropertyBag();
            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            if (value is not null)
            {
                propertyBag.AddVerifiableToken(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
            if (value is not null)
            {
                propertyBag.AddName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "given_name")?.Value;
            if (value is not null)
            {
                propertyBag.AddGivenName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "family_name")?.Value;
            if (value is not null)
            {
                propertyBag.AddFamilyName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "picture")?.Value;
            if (value is not null)
            {
                propertyBag
                    .AddPhotoUrl(value)
                    .AddPhotoUrl24(value)
                    .AddPhotoUrl32(value)
                    .AddPhotoUrl48(value)
                    .AddPhotoUrl72(value)
                    .AddPhotoUrl192(value)
                    .AddPhotoUrl512(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            if (value is not null)
            {
                propertyBag.AddEmail(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email_verified")?.Value;
            if (value is not null)
            {
                propertyBag.AddEmailVerified(bool.Parse(value));
            }

            return propertyBag;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private class Jws
    {
        [JsonPropertyName("keys")] public JsonWebKey[] Keys { get; } = [];
    }
}
