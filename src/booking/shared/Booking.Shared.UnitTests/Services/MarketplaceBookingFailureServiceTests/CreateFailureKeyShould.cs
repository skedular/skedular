using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingFailureServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CreateFailureKeyShould
{
    [Fact]
    public void Use_The_Booking_And_Category_For_A_One_Time_Failure()
    {
        var finalization = CreateFinalization(
            MarketplaceBookingFailureScopeConstants.OneTimeBooking,
            "booking-1");

        MarketplaceBookingFailureKey.Create(finalization)
            .ShouldBe("marketplace-booking-failure:OneTimeBooking:booking-1:AvailabilityConflict");
    }

    [Fact]
    public void Include_The_Occurrence_Window_For_A_Recurring_Occurrence()
    {
        var finalization = CreateFinalization(
            MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
            recurringBookingId: "recurring-1",
            requestedFrom: new DateTimeOffset(2026, 7, 22, 10, 0, 0, TimeSpan.Zero));

        MarketplaceBookingFailureKey.Create(finalization)
            .ShouldBe("marketplace-booking-failure:RecurringOccurrence:recurring-1:1784714400000:AvailabilityConflict");
    }

    private static MarketplaceBookingFailureFinalization CreateFinalization(
        string scope,
        string? bookingId = null,
        string? recurringBookingId = null,
        DateTimeOffset? requestedFrom = null) =>
        new(
            null,
            MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            scope,
            TimeProvider.System.GetUtcNow(),
            bookingId,
            recurringBookingId,
            null,
            requestedFrom,
            null,
            [],
            MarketplaceBookingFailureCustomerActionConstants.Rebook,
            null,
            null,
            null,
            []);
}
