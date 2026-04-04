using Api.Shared.Services.Grpc.Skedular.Booking.V1;

namespace Booking.Domain.IntegrationTests.Api.Grpc.BookingGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class GetVersionShould(BookingService.BookingServiceClient bookingServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await bookingServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
