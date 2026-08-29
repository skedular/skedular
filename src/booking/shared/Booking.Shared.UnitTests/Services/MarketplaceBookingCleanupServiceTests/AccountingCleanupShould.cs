using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingCleanupServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AccountingCleanupShould
{
    [Theory]
    [InlineData(MarketplaceBookingFailureAccountingCleanupStatus.NotRequired, MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired)]
    [InlineData(MarketplaceBookingFailureAccountingCleanupStatus.Pending, MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending)]
    [InlineData(MarketplaceBookingFailureAccountingCleanupStatus.TransitionRequired,
        MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired)]
    public void Round_Trip_Persisted_Accounting_Status(
        MarketplaceBookingFailureAccountingCleanupStatus status,
        string expectedPersistedValue) =>
        status.ToPersistedValue().ShouldBe(expectedPersistedValue);

    [Fact]
    public void Expose_Transition_Required_As_An_Actionable_Display_Name() =>
        MarketplaceBookingFailureAccountingCleanupStatus.TransitionRequired
            .ToDisplayName()
            .ShouldBe("Transition required");
}
