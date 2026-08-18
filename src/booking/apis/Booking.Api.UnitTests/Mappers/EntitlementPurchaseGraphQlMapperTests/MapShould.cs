using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Shared.Models.Entitlements;

namespace Booking.Api.UnitTests.Mappers.EntitlementPurchaseGraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void PreservePaymentIdentityAndSettlementFields(string purchaseId, string customerId, string organizationId, string productVersionId,
        string entitlementId)
    {
        var source = new EntitlementPurchase
        {
            Id = purchaseId,
            PaymentStatus = "CONFIRMED",
            PaymentMethod = "CARD",
            PaymentExpiry = DateTimeOffset.Parse("2026-08-12T00:00:00Z"),
            Amount = 100,
            Currency = "NZD",
            ProductPricing = ProductPricing.Empty("pricing-1") with
            {
                EntitlementCreditQuantity = 5,
                EntitlementValidityDays = 30,
            },
            CustomerId = customerId,
            OrganizationId = organizationId,
            ProductVersionId = productVersionId,
            EntitlementId = entitlementId,
            CheckoutReturnUrl = "https://example.test/return",
            PaymentAction = "https://checkout.stripe.test/session",
            InvoiceEmailList = ["billing@example.test"],
        };

        var result = new EntitlementPurchaseGraphQlMapper().Map(source);

        result.Id.ShouldBe(source.Id);
        result.PaymentStatus.ShouldBe(source.PaymentStatus);
        result.PaymentMethod.ShouldBe(source.PaymentMethod);
        result.Amount.ShouldBe(source.Amount);
        result.Currency.ShouldBe(source.Currency);
        result.PricingId.ShouldBe("pricing-1");
        result.CreditQuantity.ShouldBe(5);
        result.ValidityDays.ShouldBe(30);
        result.EntitlementId.ShouldBe(source.EntitlementId);
        result.PaymentAction.ShouldBe(source.PaymentAction);
        result.InvoiceEmailList.ShouldBe(source.InvoiceEmailList);
    }
}
