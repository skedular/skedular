using System.IdentityModel.Tokens.Jwt;
using Enterprise.Shared.Context;
using Enterprise.Shared.IdentityProviders.Configurations;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Token;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WorkOS;

namespace Enterprise.Shared.IdentityProviders.WorkOS;

// ReSharper disable once InconsistentNaming
public interface IWorkOSTokenService : ITokenService;

// ReSharper disable once InconsistentNaming
public class WorkOSTokenService(
    IdentityProvidersConfiguration identityProvidersConfiguration,
    IContext context,
    IMemoryCache memoryCache,
    WorkOSClient workOsClient,
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<WorkOSTokenService> logger)
    : IWorkOSTokenService
{
    private readonly Configurations.WorkOS _configuration = identityProvidersConfiguration.WorkOS!;

    public async Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Verifying WorkOS token. TokenLength={TokenLength}", token.Length);

            var jws = await memoryCache.GetOrCreateAsync<Jws>("workos-public-keys", async cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = timeProvider.GetUtcNow().AddMinutes(15);

                return await _configuration.JwksUri.GetJsonAsync<Jws>(cancellationToken: cancellationToken);
            });

            ArgumentNullException.ThrowIfNull(jws);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var issuer = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if (issuer is null || (_configuration.Issuer != issuer && _configuration.OtherIssuers.All(item => item.ToString() != issuer)))
            {
                return;
            }

            await new JsonWebTokenHandler().ValidateTokenAsync(
                jwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _configuration.Issuer,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = jws.Keys,
                    ValidateLifetime = true,
                });

            var sub = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            ArgumentException.ThrowIfNullOrWhiteSpace(sub);
            context.SetVerifiableToken(sub);
            logger.LogDebug("WorkOS token subject resolved");

            await using var scope = serviceProvider.CreateAsyncScope();
            var customerHelper = scope.ServiceProvider.GetService<ICustomerHelper>();
            if (customerHelper is not null && !await customerHelper.DoesCustomerExistAsync(sub, cancellationToken))
            {
                logger.LogDebug("Customer not found locally. Fetching WorkOS profile");

                var userProfile = await workOsClient.MakeAPIRequest<Profile>(
                    new WorkOSRequest
                    {
                        Method = HttpMethod.Get,
                        Path = $"/user_management/users/{sub}",
                    },
                    cancellationToken);

                context.SetEmail(userProfile.Email);
                context.SetEmailVerified(userProfile.EmailVerified);

                if (!string.IsNullOrWhiteSpace(userProfile.PhotoUrl))
                {
                    context.SetPhotoUrl(userProfile.PhotoUrl);
                    context.SetPhotoUrl24(userProfile.PhotoUrl);
                    context.SetPhotoUrl32(userProfile.PhotoUrl);
                    context.SetPhotoUrl48(userProfile.PhotoUrl);
                    context.SetPhotoUrl72(userProfile.PhotoUrl);
                    context.SetPhotoUrl192(userProfile.PhotoUrl);
                    context.SetPhotoUrl512(userProfile.PhotoUrl);
                }

                if (!string.IsNullOrWhiteSpace(userProfile.FirstName) && !string.IsNullOrWhiteSpace(userProfile.LastName))
                {
                    context.SetGivenName(userProfile.FirstName);
                    context.SetFamilyName(userProfile.LastName);
                    context.SetName($"{userProfile.FirstName} {userProfile.LastName}");
                }
                else if (!string.IsNullOrWhiteSpace(userProfile.FirstName))
                {
                    context.SetGivenName(userProfile.FirstName);
                }
                else if (!string.IsNullOrWhiteSpace(userProfile.LastName))
                {
                    context.SetFamilyName(userProfile.LastName);
                }
            }

            logger.LogInformation("WorkOS token verified successfully");
        }
        catch
        {
            // ignored
        }
    }
}
