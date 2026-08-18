using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class EntitlementQueryContractShould(IEntitlementQueryContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Expose_Entitlement_History_And_Refund_Fields(CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var fields = result.Data.ShouldNotBeNull().__type?.Fields?.Select(field => field.Name).ToHashSet();
        fields.ShouldNotBeNull();
        fields.ShouldContain("ledger");
        fields.ShouldContain("refund");
        fields.ShouldContain("status");
        fields.ShouldContain("pricingId");
        fields.ShouldContain("linkedBookingIds");

        var purchaseFields = result.Data.PurchaseDetails?.Fields?.Select(field => field.Name).ToHashSet();
        purchaseFields.ShouldNotBeNull();
        purchaseFields.ShouldContain("paymentStatus");
        purchaseFields.ShouldContain("paymentExpiry");
        purchaseFields.ShouldContain("entitlementId");
        purchaseFields.ShouldContain("pricingId");
        purchaseFields.ShouldContain("creditQuantity");
        purchaseFields.ShouldContain("validityDays");

        var ledgerFields = result.Data.LedgerDetails?.Fields?.Select(field => field.Name).ToHashSet();
        ledgerFields.ShouldNotBeNull();
        ledgerFields.ShouldContain("transactionType");
        ledgerFields.ShouldContain("bookingId");

        var mutationFields = result.Data.MutationType?.Fields?.ToDictionary(field => field.Name);
        mutationFields.ShouldNotBeNull();
        mutationFields.ShouldContainKey("consumeEntitlementCredit");
        mutationFields["consumeEntitlementCredit"].Args.ShouldContain(argument => argument.Name == "input");
    }
}
