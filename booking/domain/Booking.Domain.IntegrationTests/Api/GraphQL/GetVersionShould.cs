using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class GetVersionShould(IGetVersionQuery getVersionQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await getVersionQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.BookingVersion.ShouldNotBeNull();
    }
}
