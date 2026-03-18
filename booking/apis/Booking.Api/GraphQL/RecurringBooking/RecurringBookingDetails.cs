using Booking.Api.GraphQL.Booking;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("RecurringBookingDetails")]
public class RecurringBookingDetails : Node
{
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("category")] public BookingCategoryDetails Category { get; set; } = new();
    [GraphQLName("channel")] public BookingChannelDetails Channel { get; set; } = new();
    [GraphQLName("frequency")] public BookingFrequencyDetails Frequency { get; set; } = new();
    [GraphQLName("interval")] public int Interval { get; set; }
    [GraphQLName("byMonthDay")] public int? ByMonthDay { get; set; }
    [GraphQLName("bySetPosition")] public int? BySetPosition { get; set; }
    [GraphQLName("byWeekDays")] public IEnumerable<DayOfWeekDetails> ByWeekDays { get; set; } = [];
    [GraphQLName("endType")] public BookingRecurrenceEndTypeDetails EndType { get; set; } = new();
    [GraphQLName("startDate")] public DateTimeOffset StartDate { get; set; }
    [GraphQLName("endDate")] public DateTimeOffset? EndDate { get; set; }
    [GraphQLName("occurrenceCount")] public int? OccurrenceCount { get; set; }
    [GraphQLName("skippedDates")] public IEnumerable<DateTimeOffset> SkippedDates { get; set; } = [];

    [GraphQLName("involvedCustomerIds")] public IEnumerable<string> InvolvedCustomerIds { get; set; } = [];

    [GraphQLName("involvedOrganizationIds")]
    public IEnumerable<(string Id, string UniqueAlphanumericName)> InvolvedOrganizationIds { get; set; } = [];

    [GraphQLName("involvedTeamIds")] public IEnumerable<string> InvolvedTeamIds { get; set; } = [];
    [GraphQLName("createdByCustomerId")] public string? CreatedByCustomerId { get; set; }

    [GraphQLName("lastModifiedByCustomerId")]
    public string? LastModifiedByCustomerId { get; set; }

    [GraphQLName("deletedByCustomerId")] public string? DeletedByCustomerId { get; set; }
    [GraphQLName("marketplaceBooking")] public MarketplaceBookingDetails? MarketplaceBooking { get; set; }
}

[ObjectType<RecurringBookingDetails>]
public static partial class RecurringBookingDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<RecurringBookingDetails> descriptor)
    {
        descriptor.Ignore(item => item.InvolvedCustomerIds);
        descriptor.Ignore(item => item.CreatedByCustomerId);
        descriptor.Ignore(item => item.LastModifiedByCustomerId);
        descriptor.Ignore(item => item.DeletedByCustomerId);
        descriptor.Ignore(item => item.InvolvedOrganizationIds);
        descriptor.Ignore(item => item.InvolvedTeamIds);
    }

    public static IEnumerable<CustomerDetails> GetInvolvedCustomers([Parent] RecurringBookingDetails item) =>
        item.InvolvedCustomerIds.Select(id => new CustomerDetails(id));

    public static CustomerDetails? GetCreatedByCustomer([Parent] RecurringBookingDetails item) =>
        string.IsNullOrWhiteSpace(item.CreatedByCustomerId) ? null : new CustomerDetails(item.CreatedByCustomerId);

    public static CustomerDetails? GetLastModifiedByCustomer([Parent] RecurringBookingDetails item) =>
        string.IsNullOrWhiteSpace(item.LastModifiedByCustomerId) ? null : new CustomerDetails(item.LastModifiedByCustomerId);

    public static CustomerDetails? GetDeletedByCustomer([Parent] RecurringBookingDetails item) =>
        string.IsNullOrWhiteSpace(item.DeletedByCustomerId) ? null : new CustomerDetails(item.DeletedByCustomerId);

    public static IEnumerable<OrganizationDetails> GetInvolvedOrganizations([Parent] RecurringBookingDetails item) =>
        item.InvolvedOrganizationIds.Select(tuple => new OrganizationDetails(tuple.Id, tuple.UniqueAlphanumericName));

    public static IEnumerable<TeamDetails> GetInvolvedTeams([Parent] RecurringBookingDetails item) =>
        item.InvolvedTeamIds.Select(id => new TeamDetails(id));
}
