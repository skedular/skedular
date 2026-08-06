using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Team.Shared.Database.Entities;
using Team.Shared.Services.Cache;

namespace Team.Shared.UnitTests.Services.Cache.CachedCustomerServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RemoveShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Debug_When_Cache_Entries_Are_Evicted(
        [Frozen]
        HybridCache hybridCache,
        [Frozen]
        ILogger<CachedCustomerService> logger,
        CachedCustomerService sut,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = "customer-1",
            Identities =
            [
                new Identity
                {
                    Id = "token-1",
                },
                new Identity
                {
                    Id = "token-2",
                },
            ],
        };

        await sut.RemoveAsync([customer], cancellationToken);

        A.CallTo(() => hybridCache.RemoveAsync(A<string>._, cancellationToken)).MustHaveHappened(3, Times.Exactly);
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Debug)
            .MustHaveHappened();
    }
}
