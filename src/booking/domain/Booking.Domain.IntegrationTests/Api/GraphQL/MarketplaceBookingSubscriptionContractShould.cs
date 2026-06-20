using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingSubscriptionContractShould(
    IMarketplaceBookingSubscriptionContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Expose_Weekly_Selected_Days_On_The_Subscription_Mutation_And_Details(
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var data = result.Data.ShouldNotBeNull();
        var addInput = data.AddMarketplaceBookingSubscriptionInput.ShouldNotBeNull();
        var details = data.MarketplaceBookingSubscriptionDetails.ShouldNotBeNull();
        addInput.InputFields.ShouldNotBeNull().Select(field => field.Name)
            .ShouldContain("weeklySelectedDays");
        details.Fields.ShouldNotBeNull().Select(field => field.Name)
            .ShouldContain("weeklySelectedDays");
    }
}
