using Booking.Api.GraphQL.EntitlementPurchase;
using Booking.Shared.Models.Entitlements;

namespace Booking.Api.Mappers;

public interface IEntitlementPurchaseGraphQlMapper
{
    EntitlementPurchaseDetails Map(EntitlementPurchase source);
}

public sealed class EntitlementPurchaseGraphQlMapper : IEntitlementPurchaseGraphQlMapper
{
    public EntitlementPurchaseDetails Map(EntitlementPurchase source) => new()
    {
        Id = source.Id,
        PaymentStatus = source.PaymentStatus,
        LifecycleState = source.LifecycleState.ToString(),
        PaymentMethod = source.PaymentMethod,
        PaymentExpiry = source.PaymentExpiry,
        ServiceStartAt = source.ServiceStartAt,
        Amount = source.Amount,
        Currency = source.Currency,
        PricingId = source.ProductPricing.Id,
        CreditQuantity = source.ProductPricing.EntitlementCreditQuantity ?? 0,
        ValidityDays = source.ProductPricing.EntitlementValidityDays ?? 0,
        CustomerId = source.CustomerId,
        OrganizationId = source.OrganizationId,
        ProductVersionId = source.ProductVersionId,
        EntitlementId = source.EntitlementId,
        CheckoutReturnUrl = source.CheckoutReturnUrl,
        InvoiceNumber = source.InvoiceNumber,
        InvoiceUrl = source.InvoiceUrl,
        PaymentInstructions = source.PaymentInstructions,
        PaymentAction = source.PaymentAction ?? source.StripeCheckoutUrl ?? source.PaymentInstructions,
        InvoiceEmailList = source.InvoiceEmailList,
    };
}
