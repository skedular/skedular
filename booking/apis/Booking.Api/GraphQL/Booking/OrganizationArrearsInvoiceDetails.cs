using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("OrganizationArrearsInvoiceDetails")]
public class OrganizationArrearsInvoiceDetails
{
    [GraphQLName("invoiceNumber")] public string InvoiceNumber { get; set; } = string.Empty;
    [GraphQLName("invoiceUrl")] public string InvoiceUrl { get; set; } = string.Empty;

    [GraphQLName("billingPeriodStartInclusive")]
    public DateTimeOffset BillingPeriodStartInclusive { get; set; }

    [GraphQLName("billingPeriodEndExclusive")]
    public DateTimeOffset BillingPeriodEndExclusive { get; set; }

    [GraphQLName("currency")] public string Currency { get; set; } = string.Empty;
    [GraphQLName("totalAmount")] public decimal TotalAmount { get; set; }
    [GraphQLName("totalAmountToDisplay")] public string TotalAmountToDisplay { get; set; } = string.Empty;
    [GraphQLName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
}
