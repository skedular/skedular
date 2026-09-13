using Api.Shared.Services.Models;
using Marketplace.Api.Services;

namespace Marketplace.Api.UnitTests.Services.ProductServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class ValidateEntitlementAutoRenewalShould
{
    [Fact]
    public void Allow_auto_renewal_for_credit_entitlement_pricing()
    {
        var pricing = ProductPricing.Empty("entitlement") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            SupportsSubscriptionAutoRenewal = true,
            BillingMode = ProductPricingBillingMode.Upfront,
            AcceptedPaymentMethods = [PaymentMethod.Card],
            CancellationPolicyType = ProductPricingCancellationPolicyType.NoCancellation,
        };

        Should.NotThrow(() => ProductService.Validate(ProductType.Resource, pricing, false));
    }

    [Fact]
    public void Allow_auto_renewal_for_in_arrears_pricing()
    {
        var pricing = ProductPricing.Empty("reservation") with
        {
            BillingMode = ProductPricingBillingMode.InArrears,
            SupportsSubscriptionAutoRenewal = true,
            AcceptedPaymentMethods = [PaymentMethod.Card],
            CancellationPolicyType = ProductPricingCancellationPolicyType.NoCancellation,
        };

        Should.NotThrow(() => ProductService.Validate(ProductType.Resource, pricing, false));
    }
}
