using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Mappers.MapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToRecurringBookingShould
{
    [Fact]
    public void Preserve_Recurring_Duration_When_Mapping_Missing_Booking_Day()
    {
        var sut = new Mapper(TimeProvider.System);
        var recurringBooking = new RecurringBooking
        {
            From = new DateTimeOffset(2026, 4, 10, 22, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 4, 11, 2, 30, 0, TimeSpan.Zero),
            Category = BookingCategoryConstants.WorkingFromOffice,
            Channel = BookingChannelConstants.Private,
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            RequestedResources = []
        };

        var result = sut.MapTo(recurringBooking, new DateOnly(2026, 4, 14));

        result.From.ShouldBe(new DateTimeOffset(2026, 4, 14, 22, 0, 0, TimeSpan.Zero));
        result.Until.ShouldBe(new DateTimeOffset(2026, 4, 15, 2, 30, 0, TimeSpan.Zero));
        result.Schedules.Single().From.ShouldBe(result.From);
        result.Schedules.Single().Until.ShouldBe(result.Until);
    }

    [Fact]
    public void Preserve_Recurring_Duration_When_Remapping_Existing_Booking_To_A_Different_Day()
    {
        var sut = new Mapper(TimeProvider.System);
        var recurringBooking = new RecurringBooking
        {
            From = new DateTimeOffset(2026, 4, 10, 22, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 4, 11, 2, 30, 0, TimeSpan.Zero),
            Category = BookingCategoryConstants.WorkingFromOffice,
            Channel = BookingChannelConstants.Private,
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            RequestedResources = []
        };
        var existingBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
            From = new DateTimeOffset(2026, 4, 12, 22, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 4, 13, 2, 30, 0, TimeSpan.Zero),
            Resources = []
        };

        var result = sut.MapTo(recurringBooking, existingBooking, null, new DateOnly(2026, 4, 16));

        result.From.ShouldBe(new DateTimeOffset(2026, 4, 16, 22, 0, 0, TimeSpan.Zero));
        result.Until.ShouldBe(new DateTimeOffset(2026, 4, 17, 2, 30, 0, TimeSpan.Zero));
        result.Schedules.Single().From.ShouldBe(result.From);
        result.Schedules.Single().Until.ShouldBe(result.Until);
    }
}
