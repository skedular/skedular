using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingCleanupServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StatusMappingShould
{
    [Theory]
    [InlineData(MarketplaceBookingFailureResourceReleaseStatusConstants.Pending, MarketplaceBookingFailureResourceReleaseStatus.Pending)]
    [InlineData(MarketplaceBookingFailureResourceReleaseStatusConstants.Released, MarketplaceBookingFailureResourceReleaseStatus.Released)]
    public void Map_Resource_Release_Status(string persisted, MarketplaceBookingFailureResourceReleaseStatus expected) =>
        persisted.ToResourceReleaseStatus().ShouldBe(expected);

    [Theory]
    [InlineData(MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired, MarketplaceBookingFailureAccountingCleanupStatus.NotRequired)]
    [InlineData(MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending, MarketplaceBookingFailureAccountingCleanupStatus.Pending)]
    [InlineData(MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired,
        MarketplaceBookingFailureAccountingCleanupStatus.TransitionRequired)]
    public void Map_Accounting_Cleanup_Status(string persisted, MarketplaceBookingFailureAccountingCleanupStatus expected) =>
        persisted.ToAccountingCleanupStatus().ShouldBe(expected);

    [Fact]
    public void Reject_Unknown_Status_Persistence()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            MarketplaceBookingFailureResourceReleaseStatus.Unknown.ToPersistedValue());
        Should.Throw<ArgumentOutOfRangeException>(() =>
            MarketplaceBookingFailureAccountingCleanupStatus.Unknown.ToPersistedValue());
    }
}
