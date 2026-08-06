using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Payment;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
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
        IMarketplaceRefundReadService marketplaceRefundReadService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        MapRefundAsync(marketplaceRefundReadService.GetByMarketplaceBookingIdAsync(item.Id, cancellationToken), graphQlMapper);

    private static async Task<MarketplaceRefundDetails?> MapRefundAsync(Task<MarketplaceRefundReadModel?> task, IGraphQlMapper mapper)
    {
        var model = await task;
        return model is null ? null : mapper.MapTo(model);
    }

    public static async Task<MarketplaceBookingFailureDetails?> GetFailure(
        [Parent]
        MarketplaceBookingDetails item,
        [Service]
        IMarketplaceBookingFailureReadService failureReadService,
        [Service]
        IMarketplaceBookingService marketplaceBookingService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var bookingId = await marketplaceBookingService.GetBookingIdAsync(item.Id, cancellationToken);
        var failure = bookingId is null
            ? null
            : await failureReadService.GetByBookingIdAsync(bookingId, cancellationToken);
        return failure is null ? null : graphQlMapper.MapTo(failure);
    }
}
