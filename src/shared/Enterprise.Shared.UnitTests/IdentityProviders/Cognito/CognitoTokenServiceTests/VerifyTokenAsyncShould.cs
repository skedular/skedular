using Enterprise.Shared.Context;
using Enterprise.Shared.IdentityProviders.Cognito;
using Enterprise.Shared.IdentityProviders.Configurations;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.UnitTesting.Assertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Shared.UnitTests.Security.IdentityProviders.Cognito.CognitoTokenServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class VerifyTokenAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Enrich_only_after_successful_validation(
        IContext context,
        TimeProvider timeProvider,
        ILogger<CognitoTokenService> logger,
        string issuer,
        string audience,
        string subject)
    {
        var key = JwtTestTokens.CreateKey();
        var otherKey = JwtTestTokens.CreateKey();
        otherKey.KeyId = key.KeyId;
        using var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("cognito-public-keys", new Jws
        {
            Keys = [JsonWebKeyConverter.ConvertFromSymmetricSecurityKey(key)],
        });
        var configuration = new IdentityProvidersConfiguration
        {
            Cognito = new Shared.IdentityProviders.Configurations.Cognito
            {
                Issuer = issuer,
                Audiences = audience,
            },
        };
        var sut = new CognitoTokenService(configuration, context, cache, timeProvider, logger);

        foreach (var token in new[]
                 {
                     JwtTestTokens.Create(otherKey, issuer, audience, subject),
                     JwtTestTokens.Create(key, issuer, audience, subject, DateTime.UtcNow.AddMinutes(-10)),
                     JwtTestTokens.Create(key, issuer, $"{audience}-other", subject),
                     JwtTestTokens.Create(key, $"{issuer}-other", audience, subject), "not-a-jwt",
                 })
        {
            await sut.VerifyTokenAsync(token, CancellationToken.None);
        }

        A.CallTo(context).Where(call => call.Method.Name.StartsWith("Set", StringComparison.Ordinal)).MustNotHaveHappened();
        LogAssertions.ACallToLog(logger, LogLevel.Warning).MustHaveHappened();

        await sut.VerifyTokenAsync(JwtTestTokens.Create(key, issuer, audience, subject), CancellationToken.None);

        A.CallTo(() => context.SetVerifiableToken(subject)).MustHaveHappenedOnceExactly();
    }
}
