using System.Security.Claims;
using Enterprise.Shared.Azure.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.IdentityProviders.Azure;
using Enterprise.Shared.UnitTesting.Assertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Enterprise.Shared.UnitTests.Security.IdentityProviders.Azure.AzureEntraTokenServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class VerifyTokenAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Enrich_only_after_successful_validation(
        IContext context,
        TimeProvider timeProvider,
        ILogger<AzureEntraTokenService> logger,
        Guid tenant,
        string audience,
        string subject)
    {
        var issuer = $"https://sts.windows.net/{tenant:D}/";
        var key = JwtTestTokens.CreateKey();
        var otherKey = JwtTestTokens.CreateKey();
        otherKey.KeyId = key.KeyId;
        var metadata = new OpenIdConnectConfiguration
        {
            Issuer = issuer,
        };
        metadata.SigningKeys.Add(key);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set($"openid-configuration-msteams-{tenant:D}", metadata);
        var sut = new AzureEntraTokenService(
            new AzureEntraConfiguration
            {
                ClientId = audience,
            }, cache, context, timeProvider, logger);
        var claims = new[] { new Claim("tid", tenant.ToString("D")), new Claim("oid", subject) };

        foreach (var token in new[]
                 {
                     JwtTestTokens.Create(otherKey, issuer, audience, subject, claims: claims),
                     JwtTestTokens.Create(key, issuer, audience, subject, DateTime.UtcNow.AddMinutes(-10), claims),
                     JwtTestTokens.Create(key, issuer, $"{audience}-other", subject, claims: claims),
                     JwtTestTokens.Create(key, $"{issuer}other", audience, subject, claims: claims),
                     JwtTestTokens.Create(key, issuer, audience, subject, claims: [new Claim("tid", "invalid-tenant")]), "not-a-jwt",
                 })
        {
            await sut.VerifyTokenAsync(token, CancellationToken.None);
        }

        A.CallTo(context).Where(call => call.Method.Name.StartsWith("Set", StringComparison.Ordinal)).MustNotHaveHappened();
        LogAssertions.ACallToLog(logger, LogLevel.Warning).MustHaveHappened();

        await sut.VerifyTokenAsync(JwtTestTokens.Create(key, issuer, audience, subject, claims: claims), CancellationToken.None);

        A.CallTo(() => context.SetVerifiableToken(subject)).MustHaveHappenedOnceExactly();
        A.CallTo(() => context.SetAzureTenantId(tenant)).MustHaveHappenedOnceExactly();
    }
}
