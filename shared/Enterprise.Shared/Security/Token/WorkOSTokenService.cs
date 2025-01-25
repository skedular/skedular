using System.IdentityModel.Tokens.Jwt;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WorkOS;
using Profile = Enterprise.Shared.Security.WorkOS.Profile;

namespace Enterprise.Shared.Security.Token;

// ReSharper disable once InconsistentNaming
public interface IWorkOSTokenService : ITokenService;

// ReSharper disable once InconsistentNaming
public class WorkOSTokenService : IWorkOSTokenService
{
    private readonly Configurations.WorkOS _cognitoConfiguration;
    private readonly IContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceProvider _serviceProvider;
    private readonly WorkOSClient _workOsClient;

    public WorkOSTokenService(
        ApplicationConfiguration applicationConfiguration,
        IContext context,
        IMemoryCache memoryCache,
        WorkOSClient workOsClient,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(applicationConfiguration.IdentityProviders.WorkOS);
        _cognitoConfiguration = applicationConfiguration.IdentityProviders.WorkOS;

        _context = context;
        _memoryCache = memoryCache;
        _workOsClient = workOsClient;
        _serviceProvider = serviceProvider;
    }

    public async Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var jws = await _memoryCache.GetOrCreateAsync<Jws>("workos-public-keys", async cacheEntry =>
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
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = jws.Keys,
                    ValidateLifetime = true
                });

            var sub = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            ArgumentException.ThrowIfNullOrWhiteSpace(sub);
            _context.SetVerifiableToken(sub);

            await using var scope = _serviceProvider.CreateAsyncScope();
            var customerHelper = scope.ServiceProvider.GetService<ICustomerHelper>();
            if (customerHelper is not null && !await customerHelper.DoesCustomerExistAsync(sub, cancellationToken))
            {
                var userProfile = await _workOsClient.MakeAPIRequest<Profile>(
                    new WorkOSRequest { Method = HttpMethod.Get, AccessToken = token, Path = $"/user_management/users/{sub}" },
                    cancellationToken);

                _context.SetEmail(userProfile.Email);
                _context.SetEmailVerified(userProfile.EmailVerified);

                if (!string.IsNullOrWhiteSpace(userProfile.PhotoUrl))
                {
                    _context.SetPhotoUrl(userProfile.PhotoUrl);
                    _context.SetPhotoUrl24(userProfile.PhotoUrl);
                    _context.SetPhotoUrl32(userProfile.PhotoUrl);
                    _context.SetPhotoUrl48(userProfile.PhotoUrl);
                    _context.SetPhotoUrl72(userProfile.PhotoUrl);
                    _context.SetPhotoUrl192(userProfile.PhotoUrl);
                    _context.SetPhotoUrl512(userProfile.PhotoUrl);
                    _context.SetPhotoUrl24(userProfile.PhotoUrl);
                }

                if (!string.IsNullOrWhiteSpace(userProfile.FirstName) && !string.IsNullOrWhiteSpace(userProfile.LastName))
                {
                    _context.SetGivenName(userProfile.FirstName);
                    _context.SetFamilyName(userProfile.LastName);
                    _context.SetName($"{userProfile.FirstName} {userProfile.LastName}");
                }
                else if (!string.IsNullOrWhiteSpace(userProfile.FirstName))
                {
                    _context.SetGivenName(userProfile.FirstName);
                }
                else if (!string.IsNullOrWhiteSpace(userProfile.LastName))
                {
                    _context.SetFamilyName(userProfile.LastName);
                }
            }
        }
        catch
        {
            // ignored
        }
    }
}
