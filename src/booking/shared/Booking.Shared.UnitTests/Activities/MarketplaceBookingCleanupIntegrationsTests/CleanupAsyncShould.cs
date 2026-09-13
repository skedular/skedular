using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Temporalio.Testing;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingCleanupIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CleanupAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Ignore_Missing_Failures(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        MarketplaceBookingCleanupIntegrations sut,
        MarketplaceBookingCleanupInput input)
    {
        var activityEnvironment = new ActivityEnvironment();
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => failureRepository.GetByIdAsync(input.FailureId, activityEnvironment.CancellationTokenSource.Token))
            .Returns<MarketplaceBookingFailure?>(null);

        await activityEnvironment.RunAsync(() => sut.CleanupAsync(input));

        A.CallTo(() => failureRepository.GetByIdAsync(input.FailureId, activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Ignore_Already_Released_Failures(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        MarketplaceBookingCleanupIntegrations sut,
        MarketplaceBookingCleanupInput input)
    {
        var activityEnvironment = new ActivityEnvironment();
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => failureRepository.GetByIdAsync(input.FailureId, activityEnvironment.CancellationTokenSource.Token)).Returns(
            new MarketplaceBookingFailure
            {
                Id = input.FailureId,
                ResourceReleaseStatus = MarketplaceBookingFailureResourceReleaseStatusConstants.Released,
            });

        await activityEnvironment.RunAsync(() => sut.CleanupAsync(input));

        A.CallTo(() => failureRepository.GetByBookingIdAsync(A<string>._, activityEnvironment.CancellationTokenSource.Token)).MustNotHaveHappened();
    }
}
