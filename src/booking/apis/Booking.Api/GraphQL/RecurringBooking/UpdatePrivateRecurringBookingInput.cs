using Api.Shared.Services.Models;
using Booking.Api.Models;
using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("UpdatePrivateRecurringBookingInput")]
public class UpdatePrivateRecurringBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("fieldsToUpdate")] public HashSet<PrivateRecurringBookingPatchField> FieldsToUpdate { get; set; } = [];
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationCustomDomains")]
    public IEnumerable<string>? OrganizationCustomDomains { get; set; }

    [GraphQLName("teamIds")] public IEnumerable<string>? TeamIds { get; set; } = [];
    [GraphQLName("requestedResourceIds")] public IEnumerable<string>? RequestedResourceIds { get; set; } = [];
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
}
