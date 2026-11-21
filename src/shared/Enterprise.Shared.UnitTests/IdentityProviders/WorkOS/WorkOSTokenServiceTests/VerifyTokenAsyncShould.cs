using Enterprise.Shared.Context;
using Enterprise.Shared.IdentityProviders.Configurations;
using Enterprise.Shared.IdentityProviders.WorkOS;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.UnitTesting.Assertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WorkOS;

namespace Enterprise.Shared.UnitTests.Security.IdentityProviders.WorkOS.WorkOSTokenServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class VerifyTokenAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Accept_signed_AuthKit_tokens_for_configured_issuers_only(
        IContext context,
        TimeProvider timeProvider,
        ILogger<WorkOSTokenService> logger,
        string subject)
    {
        var issuer = "https://api.workos.com/";
        var customIssuer = new Uri("https://auth.example.com/");
        var key = JwtTestTokens.CreateKey();
        var otherKey = JwtTestTokens.CreateKey();
        otherKey.KeyId = key.KeyId;
        using var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("workos-public-keys", new Jws
        {
            Keys = [JsonWebKeyConverter.ConvertFromSymmetricSecurityKey(key)],
        });
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configuration = new IdentityProvidersConfiguration
        {
            WorkOS = new Shared.IdentityProviders.Configurations.WorkOS
            {
                Issuer = issuer,
                OtherIssuers = [customIssuer],
            },
        };
        var sut = new WorkOSTokenService(
            configuration, context, cache, new WorkOSClient(new WorkOSOptions
            {
                ApiKey = "test-key",
            }),
            serviceProvider, timeProvider, logger);

        foreach (var token in new[]
                 {
                     JwtTestTokens.Create(otherKey, issuer, null, subject),
                     JwtTestTokens.Create(key, issuer, null, subject, DateTime.UtcNow.AddMinutes(-10)),
                     JwtTestTokens.Create(key, "https://untrusted.example.com/", null, subject), "not-a-jwt",
                 })
        {
            await sut.VerifyTokenAsync(token, CancellationToken.None);
        }

        A.CallTo(context).Where(call => call.Method.Name.StartsWith("Set", StringComparison.Ordinal)).MustNotHaveHappened();
        LogAssertions.ACallToLog(logger, LogLevel.Warning).MustHaveHappened();

        await sut.VerifyTokenAsync(JwtTestTokens.Create(key, issuer, null, subject), CancellationToken.None);
        await sut.VerifyTokenAsync(JwtTestTokens.Create(key, customIssuer.ToString(), null, subject), CancellationToken.None);

        A.CallTo(() => context.SetVerifiableToken(subject)).MustHaveHappenedTwiceExactly();
    }
}
