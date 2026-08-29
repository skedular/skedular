using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;

namespace Booking.Api.UnitTests.Services.MarketplaceBookingFailureReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetByBookingIdAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Map_Persisted_Statuses_To_Enums(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        MarketplaceBookingFailureReadService sut,
        CancellationToken cancellationToken)
    {
        var entity = new MarketplaceBookingFailure
        {
            Id = "failure-1",
            Category = MarketplaceBookingFailureCategoryConstants.PaymentFailed,
            Scope = MarketplaceBookingFailureScopeConstants.OneTimeBooking,
            ResourceReleaseStatus = MarketplaceBookingFailureResourceReleaseStatusConstants.Released,
            AccountingCleanupStatus = MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired,
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => failureRepository.GetByBookingIdAsync("booking-1", cancellationToken)).Returns(entity);

        var result = await sut.GetByBookingIdAsync("booking-1", cancellationToken);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
        result.ResourceReleaseStatus.ShouldBe(MarketplaceBookingFailureResourceReleaseStatus.Released);
        result.AccountingCleanupStatus.ShouldBe(MarketplaceBookingFailureAccountingCleanupStatus.TransitionRequired);
    }
}
