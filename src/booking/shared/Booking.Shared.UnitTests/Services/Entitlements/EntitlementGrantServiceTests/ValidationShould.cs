using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Services.Entitlements;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementGrantServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ValidationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectReservationPricingAndIncompleteEntitlementPricing(EntitlementService sut, CancellationToken cancellationToken)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GrantAsync("purchase", "customer", "organization",
            ProductPricing.Empty("pricing"), TimeProvider.System.GetUtcNow(), "NZD", cancellationToken));

        await Assert.ThrowsAsync<EntitlementPricingConfigurationInvalid>(() => sut.GrantAsync("purchase", "customer", "organization",
            ProductPricing.Empty("pricing") with
            {
                FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            }, TimeProvider.System.GetUtcNow(), "NZD", cancellationToken));
    }
}
