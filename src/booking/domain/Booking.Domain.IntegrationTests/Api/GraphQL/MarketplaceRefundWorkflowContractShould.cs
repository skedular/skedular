using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceRefundWorkflowContractShould(IMarketplaceRefundWorkflowContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_All_Bank_Transfer_And_Cancellation_Mutations(CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var names = result.Data.ShouldNotBeNull().Mutation.MutationType?.Fields?.Select(field => field.Name).ToList();

        names.ShouldNotBeNull();
        names.ShouldContain("approveMarketplaceRefund");
        names.ShouldContain("recordBankTransferRefundSent");
        names.ShouldContain("confirmBankTransferRefundReceived");
        names.ShouldContain("cancelMarketplaceRefund");
    }
}
