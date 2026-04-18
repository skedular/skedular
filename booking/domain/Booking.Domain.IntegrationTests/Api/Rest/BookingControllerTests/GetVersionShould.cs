using Api.Shared.Clients.OpenApi.Skedular.Booking.Core.V1;

namespace Booking.Domain.IntegrationTests.Api.Rest.BookingControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class GetVersionShould(IBookingCoreClient bookingCoreClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await bookingCoreClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
