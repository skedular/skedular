using Api.Shared.Services.Models;
using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("AddMarketplaceRecurringBookingInput")]
public class AddMarketplaceRecurringBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationCustomDomains")]
    public IEnumerable<string>? OrganizationCustomDomains { get; set; }

    [GraphQLName("teamIds")] public IEnumerable<string>? TeamIds { get; set; } = [];
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("frequency")] public BookingFrequency Frequency { get; set; }
    [GraphQLName("interval")] public int Interval { get; set; }
    [GraphQLName("byMonthDay")] public int? ByMonthDay { get; set; }
    [GraphQLName("bySetPosition")] public int? BySetPosition { get; set; }
    [GraphQLName("byWeekDays")] public IEnumerable<DayOfWeek> ByWeekDays { get; set; } = [];
    [GraphQLName("endType")] public RecurringBookingEndType EndType { get; set; }
    [GraphQLName("startDate")] public DateTimeOffset StartDate { get; set; }
    [GraphQLName("endDate")] public DateTimeOffset? EndDate { get; set; }
    [GraphQLName("occurrenceCount")] public int? OccurrenceCount { get; set; }
    [GraphQLName("skippedDates")] public IEnumerable<DateTimeOffset>? SkippedDates { get; set; } = [];
    [GraphQLName("category")] public BookingCategory? Category { get; set; }
    [GraphQLName("paymentMethod")] public PaymentMethod PaymentMethod { get; set; }
    [GraphQLName("invoiceEmailList")] public IEnumerable<string>? InvoiceEmailList { get; set; } = [];
    [GraphQLName("quantity")] public int Quantity { get; set; }
    [GraphQLName("productVersionId")] public string ProductVersionId { get; set; } = string.Empty;
    [GraphQLName("pricingId")] public string PricingId { get; set; } = string.Empty;
}
