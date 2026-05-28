using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingSubscriptionStatusesQueryShould(
    IMarketplaceBookingSubscriptionStatusesQuery marketplaceBookingSubscriptionStatusesQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_All_Five_Subscription_Status_Options(CancellationToken cancellationToken)
    {
        var result = await marketplaceBookingSubscriptionStatusesQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.MarketplaceBookingSubscriptionStatuses.ShouldNotBeNull();
        result.Data.MarketplaceBookingSubscriptionStatuses.Count.ShouldBe(5);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Status_Options_With_Non_Empty_Names(CancellationToken cancellationToken)
    {
        var result = await marketplaceBookingSubscriptionStatusesQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        foreach (var status in result.Data.MarketplaceBookingSubscriptionStatuses)
        {
            status.Name.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
