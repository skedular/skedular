using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;

namespace Booking.Api.UnitTests.Mappers.GraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapModifyMarketplaceBookingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_The_Requested_Window_Resources_And_Reason(GraphQlMapper sut)
    {
        var input = new ModifyMarketplaceBookingInput
        {
            BookingId = "booking-1",
            ExpectedVersion = 3,
            From = new DateTimeOffset(2026, 8, 9, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 8, 9, 10, 0, 0, TimeSpan.Zero),
            ResourceIds = ["resource-1", "resource-2"],
            Reason = "Requested a different room",
        };

        var command = sut.MapTo(input);

        command.BookingId.ShouldBe(input.BookingId);
        command.ExpectedVersion.ShouldBe((uint)input.ExpectedVersion);
        command.From.ShouldBe(input.From);
        command.Until.ShouldBe(input.Until);
        command.ResourceIds.ShouldBe(input.ResourceIds);
        command.Reason.ShouldBe(input.Reason);
    }
}
