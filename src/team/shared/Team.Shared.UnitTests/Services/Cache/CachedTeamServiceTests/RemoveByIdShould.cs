using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Team.Shared.Services.Cache;

namespace Team.Shared.UnitTests.Services.Cache.CachedTeamServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RemoveByIdShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Debug_When_Cache_Entry_Is_Evicted(
        [Frozen]
        HybridCache hybridCache,
        [Frozen]
        ILogger<CachedTeamService> logger,
        CachedTeamService sut,
        CancellationToken cancellationToken)
    {
        await sut.RemoveByIdAsync("team-1", cancellationToken);

        A.CallTo(() => hybridCache.RemoveAsync(A<string>._, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Debug)
            .MustHaveHappened();
    }
}
