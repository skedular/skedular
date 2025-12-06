using Api.Shared.Clients.OpenApi.Skedular.Booking.V1;
using Shouldly;
using Testing.Shared;

namespace Booking.Domain.IntegrationTests.Api.Rest.BookingControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class GetVersionShould(IBookingClient bookingClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await bookingClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
