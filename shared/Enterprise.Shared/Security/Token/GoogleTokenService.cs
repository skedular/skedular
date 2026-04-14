using System.IdentityModel.Tokens.Jwt;
using Enterprise.Shared.Context;
using Enterprise.Shared.Security.Configurations;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security.Token;

public interface IGoogleTokenService : ITokenService;

public class GoogleTokenService(
    IdentityProvidersConfiguration identityProvidersConfiguration,
    IContext context,
    ILogger<GoogleTokenService> logger)
    : IGoogleTokenService
{
    private readonly Configurations.Google _googleConfiguration = identityProvidersConfiguration.Google!;

    public async Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Verifying Google token. TokenLength={TokenLength}", token.Length);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var issuer = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if (issuer is not null && _googleConfiguration.Issuer != issuer)
            {
                logger.LogWarning("Google token issuer mismatch. Issuer={Issuer}", issuer);
                return;
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(token);
            if (payload.AudienceAsList.All(audience => audience != _googleConfiguration.ApplicationId))
            {
                logger.LogWarning("Google token audience mismatch. AudienceCount={AudienceCount}", payload.AudienceAsList.Count());
                return;
            }

            context.SetVerifiableToken(payload.Subject);
            context.SetName(payload.Name);
            context.SetGivenName(payload.GivenName);
            context.SetFamilyName(payload.FamilyName);
            context.SetPhotoUrl(payload.Picture);
            context.SetPhotoUrl24(payload.Picture);
            context.SetPhotoUrl32(payload.Picture);
            context.SetPhotoUrl48(payload.Picture);
            context.SetPhotoUrl72(payload.Picture);
            context.SetPhotoUrl192(payload.Picture);
            context.SetPhotoUrl512(payload.Picture);
            context.SetLocale(payload.Locale);
            context.SetEmail(payload.Email);
            context.SetEmailVerified(payload.EmailVerified);

            logger.LogInformation("Google token verified successfully");
        }
        catch
        {
            // ignored
        }
    }
}
