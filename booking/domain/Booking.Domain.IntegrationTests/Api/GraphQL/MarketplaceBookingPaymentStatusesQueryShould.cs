using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingPaymentStatusesQueryShould(IMarketplaceBookingPaymentStatusesQuery marketplaceBookingPaymentStatusesQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Payment_Status_Options(CancellationToken cancellationToken)
    {
        var result = await marketplaceBookingPaymentStatusesQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.MarketplaceBookingPaymentStatuses.ShouldNotBeNull();
        result.Data.MarketplaceBookingPaymentStatuses.ShouldNotBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Payment_Status_Options_With_Non_Empty_Names(CancellationToken cancellationToken)
    {
        var result = await marketplaceBookingPaymentStatusesQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        foreach (var status in result.Data.MarketplaceBookingPaymentStatuses)
        {
            status.Name.ShouldNotBeNullOrWhiteSpace();
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Exclude_RecordNeverCreated_From_Payment_Status_Options(CancellationToken cancellationToken)
    {
        var result = await marketplaceBookingPaymentStatusesQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.MarketplaceBookingPaymentStatuses
            .ShouldNotContain(s => s.Type == PaymentStatus.RecordNeverCreated);
    }
}
