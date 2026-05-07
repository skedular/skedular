using Booking.Api.GraphQL.RecurringBooking;
using Booking.Api.Mappers;
using Booking.Api.Services;
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
    [GraphQLName("channel")] public BookingChannelDetails Channel { get; set; } = new();
    [GraphQLName("bookingResources")] public IEnumerable<BookingResourceDetails> BookingResources { get; set; } = [];
    [GraphQLName("involvedCustomerIds")] public IEnumerable<string> InvolvedCustomerIds { get; set; } = [];

    [GraphQLName("involvedOrganizationIds")]
    public IEnumerable<(string Id, string CustomDomain)> InvolvedOrganizationIds { get; set; } = [];

    [GraphQLName("involvedLocations")] public IEnumerable<LocationDetails> InvolvedLocations { get; set; } = [];
    [GraphQLName("involvedTeamIds")] public IEnumerable<string> InvolvedTeamIds { get; set; } = [];
    [GraphQLName("createdByCustomerId")] public string? CreatedByCustomerId { get; set; }

    [GraphQLName("lastModifiedByCustomerId")]
    public string? LastModifiedByCustomerId { get; set; }

    [GraphQLName("deletedByCustomerId")] public string? DeletedByCustomerId { get; set; }
    [GraphQLName("recurringBooking")] public RecurringBookingDetails? RecurringBooking { get; set; }
    [GraphQLName("marketplaceBooking")] public MarketplaceBookingDetails? MarketplaceBooking { get; set; }

    [GraphQLName("hasRecurringInstanceOverrides")]
    public bool? HasRecurringInstanceOverrides { get; set; }

    public async Task<IEnumerable<OrganizationArrearsInvoiceDetails>> GetArrearsInvoicesAsync(
        [Service] IBookingService bookingService,
        [Service] IGraphQlMapper graphQlMapper,
        [Parent] BookingDetails booking,
        CancellationToken cancellationToken) =>
        (await bookingService.GetArrearsInvoicesAsync(booking.Id, cancellationToken)).Select(graphQlMapper.MapTo);
}

[ObjectType<BookingDetails>]
public static partial class BookingDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<BookingDetails> descriptor)
    {
        descriptor.Ignore(item => item.InvolvedCustomerIds);
        descriptor.Ignore(item => item.CreatedByCustomerId);
        descriptor.Ignore(item => item.LastModifiedByCustomerId);
        descriptor.Ignore(item => item.DeletedByCustomerId);
        descriptor.Ignore(item => item.InvolvedOrganizationIds);
        descriptor.Ignore(item => item.InvolvedTeamIds);
    }

    public static IEnumerable<CustomerDetails> GetInvolvedCustomers([Parent] BookingDetails item) =>
        item.InvolvedCustomerIds.Select(id => new CustomerDetails(id));

    public static CustomerDetails? GetCreatedByCustomer([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.CreatedByCustomerId) ? null : new CustomerDetails(item.CreatedByCustomerId);

    public static CustomerDetails? GetLastModifiedByCustomer([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.LastModifiedByCustomerId) ? null : new CustomerDetails(item.LastModifiedByCustomerId);

    public static CustomerDetails? GetDeletedByCustomer([Parent] BookingDetails item) =>
        string.IsNullOrWhiteSpace(item.DeletedByCustomerId) ? null : new CustomerDetails(item.DeletedByCustomerId);

    public static IEnumerable<OrganizationDetails> GetInvolvedOrganizations([Parent] BookingDetails item) =>
        item.InvolvedOrganizationIds.Select(tuple => new OrganizationDetails(tuple.Id, tuple.CustomDomain));

    public static IEnumerable<TeamDetails> GetInvolvedTeams([Parent] BookingDetails item) =>
        item.InvolvedTeamIds.Select(id => new TeamDetails(id));
}
