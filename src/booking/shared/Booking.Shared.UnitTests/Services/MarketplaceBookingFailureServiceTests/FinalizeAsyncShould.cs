using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Random;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingFailureServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FinalizeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_Release_Pending_Until_A_Release_Activity_Commits_It(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        [Frozen]
        IRandomHelper randomHelper,
        MarketplaceBookingFailureService sut,
        CancellationToken cancellationToken)
    {
        var storedFailure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
            ResourceReleaseStatus = MarketplaceBookingFailureResourceReleaseStatusConstants.Pending,
            AccountingCleanupStatus = MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired,
        };
        var finalization = new MarketplaceBookingFailureFinalization(
            null,
            MarketplaceBookingFailureCategoryConstants.PaymentFailed,
            MarketplaceBookingFailureScopeConstants.OneTimeBooking,
            TimeProvider.System.GetUtcNow(),
            "booking-1",
            null,
            null,
            null,
            null,
            [],
            MarketplaceBookingFailureCustomerActionConstants.Rebook,
            null,
            "Stripe setup was unavailable.",
            null,
            []);

        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => failureRepository.GetByFailureKeyAsync(A<string>._, cancellationToken))
            .Returns((MarketplaceBookingFailure?)null);
        A.CallTo(() => randomHelper.Generate()).Returns("failure-1");
        A.CallTo(() => failureRepository.Add(A<MarketplaceBookingFailure>._))
            .Returns(storedFailure);

        var result = await sut.FinalizeAsync(finalization, cancellationToken);

        result.ResourceReleaseStatus.ShouldBe(MarketplaceBookingFailureResourceReleaseStatus.Pending);
        result.AccountingCleanupStatus.ShouldBe(MarketplaceBookingFailureAccountingCleanupStatus.NotRequired);
        A.CallTo(() => failureRepository.Add(A<MarketplaceBookingFailure>.That.Matches(item =>
                item.ResourceReleaseStatus == MarketplaceBookingFailureResourceReleaseStatusConstants.Pending &&
                item.AccountingCleanupStatus == MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Existing_Failure_For_The_Same_Stable_Key(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        MarketplaceBookingFailureService sut,
        CancellationToken cancellationToken)
    {
        var existing = new MarketplaceBookingFailure
        {
            Id = "failure-1",
            FailureKey = "marketplace-booking-failure:OneTimeBooking:booking-1:PaymentExpired",
        };
        var finalization = new MarketplaceBookingFailureFinalization(
            null,
            MarketplaceBookingFailureCategoryConstants.PaymentExpired,
            MarketplaceBookingFailureScopeConstants.OneTimeBooking,
            TimeProvider.System.GetUtcNow(),
            "booking-1",
            null,
            null,
            null,
            null,
            [],
            MarketplaceBookingFailureCustomerActionConstants.Rebook,
            null,
            null,
            null,
            []);
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => failureRepository.GetByFailureKeyAsync(existing.FailureKey, cancellationToken)).Returns(existing);

        var result = await sut.FinalizeAsync(finalization, cancellationToken);

        result.Id.ShouldBe(existing.Id);
        A.CallTo(() => failureRepository.Add(A<MarketplaceBookingFailure>._)).MustNotHaveHappened();
    }
}
