using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using BookingModel = Booking.Shared.Models.Booking;
using Resource = Booking.Shared.Database.Entities.Resource;

namespace Booking.Shared.UnitTests.Mappers.EntityMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MergeToShould
{
    [Fact]
    public void Associate_Only_Slots_Within_The_Booking_Window()
    {
        var sut = new EntityMapper(TimeProvider.System);
        var from = new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero);
        var until = from.AddDays(1);
        var resource = new Resource
        {
            Id = "resource-1",
            ResourceBookingSlots =
            [
                new ResourceBookingSlot { Id = "monday-slot", ResourceId = "resource-1", Start = from },
                new ResourceBookingSlot { Id = "wednesday-slot", ResourceId = "resource-1", Start = from.AddDays(2) }
            ]
        };
        var booking = new BookingModel
        {
            Id = "booking-1",
            From = from,
            Until = until,
            Category = BookingCategory.WorkingFromOffice,
            Channel = BookingChannel.Marketplace,
            Schedules = [new BookingSchedule(from, until)]
        };

        var result = sut.MapTo(booking, [], [], [], [], [resource], null, null, null, null, null);

        result.ResourceBookingSlots.Select(slot => slot.Id).ShouldBe(["monday-slot"]);
    }
}
