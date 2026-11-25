using Booking.Shared.Activities;
using Booking.Shared.Repositories;
using Temporalio.Testing;

namespace Booking.Shared.UnitTests.Activities.ResourceBookingSlotCleanupIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeleteUnbookedResourceBookingSlotsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Delete_Slots_Before_One_Month_Cutoff(
        [Frozen]
        IResourceBookingSlotRepository resourceBookingSlotRepository,
        [Frozen]
        TimeProvider timeProvider,
        ResourceBookingSlotCleanupIntegrations sut)
    {
        var now = new DateTimeOffset(2026, 3, 31, 14, 25, 0, TimeSpan.Zero);
        var expectedCutoff = new DateTimeOffset(2026, 2, 28, 14, 25, 0, TimeSpan.Zero);
        var deletedCount = 42;

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => resourceBookingSlotRepository.DeleteUnbookedBeforeAsync(
                expectedCutoff,
                A<CancellationToken>._))
            .Returns(deletedCount);

        var activityEnvironment = new ActivityEnvironment();
        var result = await activityEnvironment.RunAsync(() => sut.DeleteUnbookedResourceBookingSlotsAsync());

        result.ShouldBe(deletedCount);
        A.CallTo(() => resourceBookingSlotRepository.DeleteUnbookedBeforeAsync(
                expectedCutoff,
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
