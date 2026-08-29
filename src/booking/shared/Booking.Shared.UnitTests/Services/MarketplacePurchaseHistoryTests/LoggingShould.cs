using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.UnitTests.Services.MarketplacePurchaseHistoryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LoggingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Recorded_Event(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        ILogger<MarketplacePurchaseHistoryEventService> logger,
        MarketplacePurchaseHistoryEventService sut,
        MarketplacePurchaseHistoryEventModel eventModel,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => historyRepository.AppendEventAsync(eventModel, "stable-key", cancellationToken))
            .Returns(eventModel);

        await sut.AppendAsync(eventModel, "stable-key", cancellationToken);

        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappenedOnceOrMore();
    }
}
