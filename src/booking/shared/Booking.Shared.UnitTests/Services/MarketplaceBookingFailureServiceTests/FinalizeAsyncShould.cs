using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingFailureServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FinalizeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Existing_Failure_For_The_Same_Stable_Key(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingFailureRepository failureRepository,
        MarketplaceBookingFailureService sut,
        CancellationToken cancellationToken)
    {
        var existing = new MarketplaceBookingFailure
        {
            Id = "failure-1", FailureKey = "marketplace-booking-failure:OneTimeBooking:booking-1:PaymentExpired"
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
