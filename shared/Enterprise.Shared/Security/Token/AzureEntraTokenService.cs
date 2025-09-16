using System.IdentityModel.Tokens.Jwt;
using EmailValidation;
using Enterprise.Shared.Azure.Configurations;
using Enterprise.Shared.Context;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Shared.Security.Token;

public interface IAzureEntraTokenService : ITokenService;

public class AzureEntraTokenService(
    AzureEntraConfiguration azureEntraConfiguration,
    IMemoryCache memoryCache,
    IContext context)
    : IAzureEntraTokenService
{
    public async Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var tenantId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "tid")?.Value;
            if (tenantId is null)
            {
                return;
            }

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var openIdConnectConfiguration = await memoryCache.GetOrCreateAsync<OpenIdConnectConfiguration>(
                $"openid-configuration-msteams-{tenantId}",
                async cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(15);

                    var documentRetriever = new HttpDocumentRetriever { RequireHttps = true };
                    var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                        $"{authority}/.well-known/openid-configuration",
                        new OpenIdConnectConfigurationRetriever(),
                        documentRetriever
                    );

                    return await configurationManager.GetConfigurationAsync(cancellationToken);
                });

            ArgumentNullException.ThrowIfNull(openIdConnectConfiguration);

            await new JsonWebTokenHandler().ValidateTokenAsync(
                jwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = true,
                    ValidAudience = azureEntraConfiguration.ClientId,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = openIdConnectConfiguration.SigningKeys,
                    ValidateLifetime = true
                });

            if (!Guid.TryParse(tenantId, out var tenant))
            {
                return;
            }

            context.SetAzureTenantId(tenant);

            var value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            context.SetVerifiableToken(value);

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
            if (value is not null)
            {
                context.SetName(value);
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            if (value is not null && EmailValidator.Validate(value))
            {
                context.SetEmail(value);
                context.SetEmailVerified(true);
            }
            else
            {
                value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "preferred_username")?.Value;
                if (value is not null && EmailValidator.Validate(value))
                {
                    context.SetEmail(value);
                    context.SetEmailVerified(true);
                }
            }

            value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "aud")?.Value;
            if (value is not null)
            {
                context.SetAzureTenantAudience(value);
            }
        }
        catch
        {
            // ignored
        }
    }
}
