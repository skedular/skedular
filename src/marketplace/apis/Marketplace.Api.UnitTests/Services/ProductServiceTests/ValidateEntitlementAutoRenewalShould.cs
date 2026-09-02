using Api.Shared.Services;
using Api.Shared.Services.Models;
using Marketplace.Api.Services;

namespace Marketplace.Api.UnitTests.Services.ProductServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class ValidateEntitlementAutoRenewalShould
{
    [Fact]
    public void Reject_auto_renewal_for_credit_entitlement_pricing()
    {
        var pricing = ProductPricing.Empty("entitlement") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            SupportsSubscriptionAutoRenewal = true,
        };

        Should.Throw<ProductPricingEntitlementAutoRenewalNotSupported>(() =>
            ProductService.Validate(ProductType.Resource, pricing, false));
    }
}
