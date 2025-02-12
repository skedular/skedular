using System.IdentityModel.Tokens.Jwt;
using EmailValidation;
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
    private readonly IContext _context;
    private readonly IMemoryCache _memoryCache;

    public CognitoTokenService(
        ApplicationConfiguration applicationConfiguration,
        IContext context,
        IMemoryCache memoryCache)
    {
        ArgumentNullException.ThrowIfNull(applicationConfiguration.IdentityProviders.Cognito);
        _cognitoConfiguration = applicationConfiguration.IdentityProviders.Cognito;

        _audiences = _cognitoConfiguration.Audiences is null
            ? []
            : _cognitoConfiguration.Audiences.Split(",").Select(audience => audience.Trim())
                .Where(audience => !string.IsNullOrWhiteSpace(audience)).ToList();

        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
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
            var issuer = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if (issuer is not null && _cognitoConfiguration.Issuer != issuer)
            {
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
            _context.SetVerifiableToken(value);

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
            if (value is not null)
            {
                _context.SetName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "given_name")?.Value;
            if (value is not null)
            {
                _context.SetGivenName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "family_name")?.Value;
            if (value is not null)
            {
                _context.SetFamilyName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "picture")?.Value;
            if (value is not null)
            {
                _context.SetPhotoUrl(value);
                _context.SetPhotoUrl24(value);
                _context.SetPhotoUrl32(value);
                _context.SetPhotoUrl48(value);
                _context.SetPhotoUrl72(value);
                _context.SetPhotoUrl192(value);
                _context.SetPhotoUrl512(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            if (value is not null && EmailValidator.Validate(value))
            {
                _context.SetEmail(value);

                value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email_verified")?.Value;
                if (value is not null)
                {
                    _context.SetEmailVerified(bool.Parse(value));
                }
            }
        }
        catch
        {
            // ignored
        }
    }
}
