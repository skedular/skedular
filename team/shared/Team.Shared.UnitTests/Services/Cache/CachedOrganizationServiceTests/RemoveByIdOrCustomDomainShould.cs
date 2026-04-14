using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Team.Shared.Services.Cache;

namespace Team.Shared.UnitTests.Services.Cache.CachedOrganizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RemoveByIdOrCustomDomainShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Debug_When_Cache_Entry_Is_Evicted(
        [Frozen] HybridCache hybridCache,
        [Frozen] ILogger<CachedOrganizationService> logger,
        CachedOrganizationService sut,
        CancellationToken cancellationToken)
    {
        await sut.RemoveByIdOrCustomDomainAsync("org-1", "acme", cancellationToken);

        A.CallTo(() => hybridCache.RemoveAsync(A<string>._, cancellationToken)).MustHaveHappenedTwiceExactly();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Debug)
            .MustHaveHappened();
    }
}
