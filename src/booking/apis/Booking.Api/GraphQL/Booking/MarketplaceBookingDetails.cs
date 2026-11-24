using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Payment;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceBookingDetails")]
public class MarketplaceBookingDetails : Node
{
    [GraphQLName("quantity")]
    public int Quantity { get; set; }

    [GraphQLName("productVersionId")]
    public string ProductVersionId { get; set; } = string.Empty;

    [GraphQLName("entitlementId")]
    public string? EntitlementId { get; set; }

    [GraphQLName("consumingCreditLedgerEntryId")]
    public string? ConsumingCreditLedgerEntryId { get; set; }

    [GraphQLName("productPricing")]
    public ProductPricing ProductPricing { get; set; } = ProductPricing.Empty(string.Empty);

    [GraphQLName("paidByCustomerId")]
    public string? PaidByCustomerId { get; set; }

    [GraphQLName("paidByOrganizationId")]
    public string? PaidByOrganizationId { get; set; }

    [GraphQLName("paidByOrganizationUniqueCustomDomain")]
    public string? PaidByOrganizationUniqueCustomDomain { get; set; }

    [GraphQLName("paymentMethod")]
    public PaymentMethodTypeDetails PaymentMethod { get; set; } = new();

    [GraphQLName("totalAmountExcludeTax")]
    public decimal? TotalAmountExcludeTax { get; set; }

    [GraphQLName("totalAmountExcludeTaxToDisplay")]
    public string TotalAmountExcludeTaxToDisplay { get; set; } = string.Empty;

    [GraphQLName("taxAmount")]
    public decimal? TaxAmount { get; set; }

    [GraphQLName("taxAmountToDisplay")]
    public string TaxAmountToDisplay { get; set; } = string.Empty;

    [GraphQLName("taxRatePercentage")]
    public decimal? TaxRatePercentage { get; set; }

    [GraphQLName("taxRatePercentageToDisplay")]
    public string TaxRatePercentageToDisplay { get; set; } = string.Empty;

    [GraphQLName("totalAmount")]
    public decimal? TotalAmount { get; set; }

    [GraphQLName("hostCommissionRatePercentage")]
    public decimal? HostCommissionRatePercentage { get; set; }

    [GraphQLName("hostCommissionAmount")]
    public decimal? HostCommissionAmount { get; set; }

    [GraphQLName("hostPayoutAmount")]
    public decimal? HostPayoutAmount { get; set; }

    // This is the amount due to the host before Stripe processing fees. Keep
    // hostPayoutAmount for compatibility while clients move to the precise name.
    [GraphQLName("hostGrossProceedsAmount")]
    public decimal? HostGrossProceedsAmount { get; set; }

    [GraphQLName("totalAmountToDisplay")]
    public string TotalAmountToDisplay { get; set; } = string.Empty;

    [GraphQLName("currency")]
    public CurrencyDetails? Currency { get; set; }

    [GraphQLName("currencyToDisplay")]
    public string CurrencyToDisplay { get; set; } = string.Empty;

    [GraphQLName("invoiceUrl")]
    public string? InvoiceUrl { get; set; }

    [GraphQLName("invoiceNumber")]
    public string? InvoiceNumber { get; set; }

    [GraphQLName("invoiceEmailList")]
    public IEnumerable<string> InvoiceEmailList { get; set; } = [];

    [GraphQLName("billingMode")]
    public ProductPricingBillingMode BillingMode { get; set; }

    [GraphQLName("isPaymentRequired")]
    public bool IsPaymentRequired { get; set; }

    [GraphQLName("bookingCheckoutSession")]
    public BookingCheckoutSessionDetails? BookingCheckoutSession { get; set; }

    [GraphQLName("paymentExpiry")]
    public DateTimeOffset PaymentExpiry { get; set; }

    [GraphQLName("paymentStatus")]
    public PaymentStatusDetails PaymentStatus { get; set; } = new();
}

[ObjectType<MarketplaceBookingDetails>]
public static partial class MarketplaceBookingDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<MarketplaceBookingDetails> descriptor)
    {
        descriptor.Ignore(item => item.PaidByCustomerId);
        descriptor.Ignore(item => item.PaidByOrganizationId);
        descriptor.Ignore(item => item.PaidByOrganizationUniqueCustomDomain);
        descriptor.Ignore(item => item.ProductVersionId);
    }

    public static Task<string?> GetEntitlementPurchaseId(
        [Parent]
        MarketplaceBookingDetails item,
        [Service]
        EntitlementPurchaseReferenceDataLoader dataLoader,
        CancellationToken cancellationToken) =>
        string.IsNullOrWhiteSpace(item.EntitlementId)
            ? Task.FromResult<string?>(null)
            : dataLoader.LoadAsync(item.EntitlementId, cancellationToken);

    public static CustomerDetails? GetPaidByCustomer([Parent] MarketplaceBookingDetails item) => string.IsNullOrWhiteSpace(item.PaidByCustomerId)
        ? null
        : new CustomerDetails(item.PaidByCustomerId);

    public static OrganizationDetails? GetPaidByOrganization([Parent] MarketplaceBookingDetails item) =>
        string.IsNullOrWhiteSpace(item.PaidByOrganizationId)
            ? null
            : new OrganizationDetails(item.PaidByOrganizationId, item.PaidByOrganizationUniqueCustomDomain.ToSafeString());

    public static ProductVersionDetails GetProductVersion([Parent] MarketplaceBookingDetails item) => new(item.ProductVersionId);

    public static Task<MarketplaceRefundDetails?> GetRefund(
        [Parent]
        MarketplaceBookingDetails item,
        [Service]
        MarketplaceBookingRefundDataLoader dataLoader,
        CancellationToken cancellationToken) =>
        dataLoader.LoadAsync(item.Id, cancellationToken);

    public static Task<MarketplaceBookingFailureDetails?> GetFailure(
        [Parent]
        MarketplaceBookingDetails item,
        [Service]
        MarketplaceBookingFailureDataLoader dataLoader,
        CancellationToken cancellationToken) =>
        dataLoader.LoadAsync(item.Id, cancellationToken);
}
