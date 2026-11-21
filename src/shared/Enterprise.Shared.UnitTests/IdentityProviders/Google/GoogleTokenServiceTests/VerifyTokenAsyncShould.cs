using Enterprise.Shared.Context;
using Enterprise.Shared.IdentityProviders.Configurations;
using Enterprise.Shared.IdentityProviders.Google;
using Enterprise.Shared.UnitTesting.Assertions;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Security.IdentityProviders.Google.GoogleTokenServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class VerifyTokenAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_malformed_tokens_without_enriching_identity(
        IContext context,
        ILogger<GoogleTokenService> logger,
        string issuer,
        string token)
    {
        var configuration = new IdentityProvidersConfiguration
        {
            Google = new Shared.IdentityProviders.Configurations.Google
            {
                Issuer = issuer,
            },
        };
        var sut = new GoogleTokenService(configuration, context, logger);

        await sut.VerifyTokenAsync(token, CancellationToken.None);

        A.CallTo(context).Where(call => call.Method.Name.StartsWith("Set", StringComparison.Ordinal)).MustNotHaveHappened();
        LogAssertions.ACallToLog(logger, LogLevel.Warning).MustHaveHappenedOnceExactly();
        Fake.GetCalls(logger).Where(call => call.Method.Name == "Log")
            .ShouldAllBe(call =>
                !call.GetArgument<object>(2).ShouldNotBeNull().ToString().ShouldNotBeNull().Contains(token, StringComparison.Ordinal));
    }
}
