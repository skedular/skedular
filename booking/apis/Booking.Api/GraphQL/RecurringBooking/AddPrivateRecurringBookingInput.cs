using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("AddPrivateRecurringBookingInput")]
public class AddPrivateRecurringBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationUniqueAlphanumericNames")]
    public IEnumerable<string>? OrganizationUniqueAlphanumericNames { get; set; }

    [GraphQLName("teamIds")] public IEnumerable<string> TeamIds { get; set; } = [];
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("frequency")] public BookingFrequency Frequency { get; set; }
    [GraphQLName("interval")] public int Interval { get; set; }
    [GraphQLName("byMonthDay")] public int? ByMonthDay { get; set; }
    [GraphQLName("bySetPosition")] public int? BySetPosition { get; set; }
    [GraphQLName("byWeekDays")] public ICollection<DayOfWeek> ByWeekDays { get; set; } = [];
    [GraphQLName("endType")] public RecurringBookingEndType EndType { get; set; }
    [GraphQLName("startDate")] public DateTimeOffset StartDate { get; set; }
    [GraphQLName("endDate")] public DateTimeOffset? EndDate { get; set; }
    [GraphQLName("occurrenceCount")] public int? OccurrenceCount { get; set; }
    [GraphQLName("skippedDates")] public ICollection<DateTimeOffset> SkippedDates { get; set; } = [];
}
