using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Payment;
using Booking.Api.Services;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingDetails")]
public class BookingDetails : Node
{
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("category")] public BookingCategoryDetails Category { get; set; } = new();
    [GraphQLName("bookingResources")] public IEnumerable<BookingResourceDetails> BookingResources { get; set; } = [];
    [GraphQLName("lineItems")] public IEnumerable<LineItemDetails> LineItems { get; set; } = [];
    [GraphQLName("involvedCustomerIds")] public IEnumerable<string> InvolvedCustomerIds { get; set; } = [];

    [GraphQLName("involvedOrganizationIds")]
    public IEnumerable<(string Id, string UniqueAlphanumericName)> InvolvedOrganizationIds { get; set; } = [];

    [GraphQLName("involvedLocationIds")] public IEnumerable<string> InvolvedLocationIds { get; set; } = [];
    [GraphQLName("involvedTeamIds")] public IEnumerable<string> InvolvedTeamIds { get; set; } = [];
    [GraphQLName("paidByCustomerId")] public string? PaidByCustomerId { get; set; }
    [GraphQLName("paidByOrganizationId")] public string? PaidByOrganizationId { get; set; }

    [GraphQLName("paidByOrganizationUniqueAlphanumericName")]
    public string? PaidByOrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("createdByCustomerId")] public string? CreatedByCustomerId { get; set; }
    [GraphQLName("paymentMethod")] public PaymentMethodTypeDetails? PaymentMethod { get; set; }
    [GraphQLName("totalAmountExcludeTax")] public decimal? TotalAmountExcludeTax { get; set; }

    [GraphQLName("totalAmountExcludeTaxToDisplay")]
    public string TotalAmountExcludeTaxToDisplay { get; set; } = string.Empty;

    [GraphQLName("taxAmount")] public decimal? TaxAmount { get; set; }
    [GraphQLName("taxAmountToDisplay")] public string TaxAmountToDisplay { get; set; } = string.Empty;
    [GraphQLName("taxRatePercentage")] public decimal? TaxRatePercentage { get; set; }

    [GraphQLName("taxRatePercentageToDisplay")]
    public string TaxRatePercentageToDisplay { get; set; } = string.Empty;

    [GraphQLName("totalAmount")] public decimal? TotalAmount { get; set; }
    [GraphQLName("totalAmountToDisplay")] public string TotalAmountToDisplay { get; set; } = string.Empty;
    [GraphQLName("currency")] public string? Currency { get; set; }
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;
    [GraphQLName("invoiceUrl")] public string? InvoiceUrl { get; set; }
    [GraphQLName("invoiceNumber")] public string? InvoiceNumber { get; set; }
    [GraphQLName("invoiceEmailList")] public IEnumerable<string>? InvoiceEmailList { get; set; }

    [GraphQLName("lastModifiedByCustomerId")]
    public string? LastModifiedByCustomerId { get; set; }

    [GraphQLName("deletedByCustomerId")] public string? DeletedByCustomerId { get; set; }
    [GraphQLName("isPaymentRequired")] public bool IsPaymentRequired { get; set; }

    [GraphQLName("bookingCheckoutSession")]
    public BookingCheckoutSessionDetails? BookingCheckoutSession { get; set; }

    [GraphQLName("paymentExpiry")] public DateTimeOffset PaymentExpiry { get; set; }
    [GraphQLName("bookedOnMarketplace")] public bool BookedOnMarketplace { get; set; }

    [UseResolverScope]
    public async Task<PaymentStatusDetails> PaymentStatusAsync(
        [Parent] BookingDetails booking,
        [Service] IBookingPaymentService bookingPaymentService,
        CancellationToken cancellationToken)
    {
        var paymentStatus = await bookingPaymentService.GetPaymentStatusAsync(booking.Id, cancellationToken);

        return new PaymentStatusDetails { Type = paymentStatus, Name = paymentStatus.ToPaymentStatusName() };
    }
}

[ObjectType<BookingDetails>]
public static partial class BookingDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<BookingDetails> descriptor)
    {
        descriptor.Ignore(item => item.InvolvedCustomerIds);
        descriptor.Ignore(item => item.PaidByCustomerId);
        descriptor.Ignore(item => item.CreatedByCustomerId);
        descriptor.Ignore(item => item.LastModifiedByCustomerId);
        descriptor.Ignore(item => item.DeletedByCustomerId);
        descriptor.Ignore(item => item.InvolvedOrganizationIds);
        descriptor.Ignore(item => item.PaidByOrganizationId);
        descriptor.Ignore(item => item.PaidByOrganizationUniqueAlphanumericName);
        descriptor.Ignore(item => item.InvolvedLocationIds);
        descriptor.Ignore(item => item.InvolvedTeamIds);
    }

    public static IEnumerable<CustomerDetails> GetInvolvedCustomers([Parent] BookingDetails item) =>
        item.InvolvedCustomerIds.Select(id => new CustomerDetails(id));

    public static CustomerDetails? GetPaidByCustomer([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.PaidByCustomerId) ? null : new CustomerDetails(item.PaidByCustomerId);

    public static CustomerDetails? GetCreatedByCustomer([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.CreatedByCustomerId) ? null : new CustomerDetails(item.CreatedByCustomerId);

    public static CustomerDetails? GetLastModifiedByCustomer([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.LastModifiedByCustomerId) ? null : new CustomerDetails(item.LastModifiedByCustomerId);

    public static CustomerDetails? GetDeletedByCustomer([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.DeletedByCustomerId) ? null : new CustomerDetails(item.DeletedByCustomerId);

    public static IEnumerable<OrganizationDetails> GetInvolvedOrganizations([Parent] BookingDetails item) =>
        item.InvolvedOrganizationIds.Select(tuple => new OrganizationDetails(tuple.Id, tuple.UniqueAlphanumericName));

    public static OrganizationDetails? GetPaidByOrganization([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.PaidByOrganizationId)
            ? null
            : new OrganizationDetails(item.PaidByOrganizationId, item.PaidByOrganizationUniqueAlphanumericName.ToSafeString());

    public static IEnumerable<LocationDetails> GetInvolvedLocations([Parent] BookingDetails item) =>
        item.InvolvedLocationIds.Select(id => new LocationDetails(id));

    public static IEnumerable<TeamDetails> GetInvolvedTeams([Parent] BookingDetails item) =>
        item.InvolvedTeamIds.Select(id => new TeamDetails(id));
}
