using System.IdentityModel.Tokens.Jwt;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Google.Apis.Auth;

namespace Enterprise.Shared.Security.Token;

public interface IGoogleTokenService : ITokenService;

public class GoogleTokenService : IGoogleTokenService
{
    private readonly IContext _context;
    private readonly Configurations.Google _googleConfiguration;

    public GoogleTokenService(IdentityProvidersConfiguration identityProvidersConfiguration, IContext context)
    {
        ArgumentNullException.ThrowIfNull(identityProvidersConfiguration.Google);
        _googleConfiguration = identityProvidersConfiguration.Google;
        _context = context;
    }

    public async Task VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var issuer = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if (issuer is not null && _googleConfiguration.Issuer != issuer)
            {
                return;
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(token);
            if (payload.AudienceAsList.All(audience => audience != _googleConfiguration.ApplicationId))
            {
                return;
            }

            _context.SetVerifiableToken(payload.Subject);
            _context.SetName(payload.Name);
            _context.SetGivenName(payload.GivenName);
            _context.SetFamilyName(payload.FamilyName);
            _context.SetPhotoUrl(payload.Picture);
            _context.SetPhotoUrl24(payload.Picture);
            _context.SetPhotoUrl32(payload.Picture);
            _context.SetPhotoUrl48(payload.Picture);
            _context.SetPhotoUrl72(payload.Picture);
            _context.SetPhotoUrl192(payload.Picture);
            _context.SetPhotoUrl512(payload.Picture);
            _context.SetLocale(payload.Locale);
            _context.SetEmail(payload.Email);
            _context.SetEmailVerified(payload.EmailVerified);
        }
        catch
        {
            // ignored
        }
    }
}
