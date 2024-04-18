using System.IdentityModel.Tokens.Jwt;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Google.Apis.Auth;

namespace Enterprise.Shared.Security.Token;

public interface IGoogleTokenService : ITokenService;

public class GoogleTokenService : IGoogleTokenService
{
    private readonly Configurations.Google _googleConfiguration;

    public GoogleTokenService(ApplicationConfiguration applicationConfiguration)
    {
        ArgumentNullException.ThrowIfNull(applicationConfiguration.IdentityProviders.Google);
        _googleConfiguration = applicationConfiguration.IdentityProviders.Google;
    }

    public async Task<PropertyBag?> VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var value = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if (value is not null && _googleConfiguration.Issuer != value)
            {
                return null;
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(token);
            if (payload.AudienceAsList.All(audience => audience != _googleConfiguration.ApplicationId))
            {
                return null;
            }

            return new PropertyBag()
                .AddVerifiableToken(payload.Subject)
                .AddName(payload.Name)
                .AddGivenName(payload.GivenName)
                .AddFamilyName(payload.FamilyName)
                .AddPhotoUrl(payload.Picture)
                .AddPhotoUrl24(payload.Picture)
                .AddPhotoUrl32(payload.Picture)
                .AddPhotoUrl48(payload.Picture)
                .AddPhotoUrl72(payload.Picture)
                .AddPhotoUrl192(payload.Picture)
                .AddPhotoUrl512(payload.Picture)
                .AddLocale(payload.Locale)
                .AddEmail(payload.Email)
                .AddEmailVerified(payload.EmailVerified);
        }
        catch
        {
            return null;
        }
    }
}
