using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Shared.UnitTests.Security.IdentityProviders;

internal static class JwtTestTokens
{
    public static SymmetricSecurityKey CreateKey() => new(RandomNumberGenerator.GetBytes(64))
    {
        KeyId = Guid.NewGuid().ToString(),
    };

    public static string Create(
        SecurityKey key,
        string issuer,
        string? audience,
        string subject,
        DateTime? expires = null,
        IEnumerable<Claim>? claims = null) =>
        new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer,
            audience,
            new[] { new Claim("sub", subject) }.Concat(claims ?? []),
            DateTime.UtcNow.AddHours(-1),
            expires ?? DateTime.UtcNow.AddMinutes(10),
            new SigningCredentials(key, SecurityAlgorithms.HmacSha256)));
}
