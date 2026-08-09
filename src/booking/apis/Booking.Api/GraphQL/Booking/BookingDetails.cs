using Api.Shared.Services;
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
    [GraphQLName("entityFrameworkVersion")]
    public uint EntityFrameworkVersion { get; set; }

    [GraphQLName("from")]
    public DateTimeOffset From { get; set; }

    [GraphQLName("until")]
    public DateTimeOffset Until { get; set; }

    [GraphQLName("notes")]
    public string? Notes { get; set; }

    [GraphQLName("category")]
    public BookingCategoryDetails Category { get; set; } = new();

    [GraphQLName("channel")]
    public BookingChannelDetails Channel { get; set; } = new();

    [GraphQLName("bookingResources")]
    public IEnumerable<BookingResourceDetails> BookingResources { get; set; } = [];

    [GraphQLName("involvedCustomerIds")]
    public IEnumerable<string> InvolvedCustomerIds { get; set; } = [];

    [GraphQLName("involvedOrganizationIds")]
    public IEnumerable<(string Id, string CustomDomain)> InvolvedOrganizationIds { get; set; } = [];

    [GraphQLName("involvedLocations")]
    public IEnumerable<LocationDetails> InvolvedLocations { get; set; } = [];

    [GraphQLName("involvedTeamIds")]
    public IEnumerable<string> InvolvedTeamIds { get; set; } = [];

    [GraphQLName("createdByCustomerId")]
    public string? CreatedByCustomerId { get; set; }

    [GraphQLName("lastModifiedByCustomerId")]
    public string? LastModifiedByCustomerId { get; set; }

    [GraphQLName("deletedByCustomerId")]
    public string? DeletedByCustomerId { get; set; }

    [GraphQLName("recurringBooking")]
    public RecurringBookingDetails? RecurringBooking { get; set; }

    [GraphQLName("marketplaceBooking")]
    public MarketplaceBookingDetails? MarketplaceBooking { get; set; }

    [GraphQLName("cancellationPolicyOverridden")]
    public bool CancellationPolicyOverridden { get; set; }

    [GraphQLName("cancellationOverrideReason")]
    public string? CancellationOverrideReason { get; set; }

    [GraphQLName("hasRecurringInstanceOverrides")]
    public bool? HasRecurringInstanceOverrides { get; set; }

    [UseResolverScope]
    public async Task<IEnumerable<OrganizationArrearsInvoiceDetails>> GetArrearsInvoicesAsync(
        [Service]
        IBookingService bookingService,
        [Service]
        IGraphQlMapper graphQlMapper,
        [Parent]
        BookingDetails booking,
        CancellationToken cancellationToken) =>
        (await bookingService.GetArrearsInvoicesAsync(booking.Id, cancellationToken)).Select(graphQlMapper.MapTo);

    [UseResolverScope]
    public async Task<IReadOnlyList<MarketplaceBookingModificationDetails>> GetMarketplaceBookingModificationsAsync(
        [Service]
        IMarketplaceBookingModificationService marketplaceBookingModificationService,
        [Service]
        IGraphQlMapper graphQlMapper,
        [Parent]
        BookingDetails booking,
        CancellationToken cancellationToken)
    {
        try
        {
            return [.. (await marketplaceBookingModificationService.GetHistoryAsync(booking.Id, cancellationToken)).Select(graphQlMapper.MapTo)];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }
        catch (CustomerNotFound)
        {
            return [];
        }
    }

    [UseResolverScope]
    public async Task<MarketplaceBookingResourceSelectionDetails> GetMarketplaceBookingResourceSelectionAsync(
        DateTimeOffset? from,
        DateTimeOffset? until,
        string? locationId,
        [Service]
        IMarketplaceBookingModificationService marketplaceBookingModificationService,
        [Service]
        IGraphQlMapper graphQlMapper,
        [Parent]
        BookingDetails booking,
        CancellationToken cancellationToken)
    {
        MarketplaceBookingResourceSelection selection;
        try
        {
            selection = await marketplaceBookingModificationService.GetResourceSelectionAsync(booking.Id, from, until, locationId,
                cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            return new MarketplaceBookingResourceSelectionDetails();
        }
        catch (CustomerNotFound)
        {
            return new MarketplaceBookingResourceSelectionDetails();
        }

        return new MarketplaceBookingResourceSelectionDetails
        {
            CanSelectResources = selection.CanSelectResources,
            MaximumResourceCount = selection.MaximumResourceCount,
            EligibleResources = [.. graphQlMapper.MapTo(selection.EligibleResources)],
            AvailableResourceIds = [.. selection.AvailableResourceIds],
            EligibleLocations =
            [
                .. selection.EligibleLocations.Select(location => new LocationDetails
                {
                    Id = location.Id,
                    Name = location.Name ?? string.Empty,
                }),
            ],
        };
    }
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

    public static async Task<MarketplaceBookingFailureDetails?> GetFailureAsync(
        [Service]
        IGraphQlMapper graphQlMapper,
        [Service]
        IMarketplaceBookingFailureReadService failureReadService,
        [Parent]
        BookingDetails item,
        CancellationToken cancellationToken)
    {
        var failure = await failureReadService.GetByBookingIdAsync(item.Id, cancellationToken);
        return failure is null ? null : graphQlMapper.MapTo(failure);
    }


    public static async Task<MarketplaceCancellationAvailabilityDetails> GetCancellationAvailabilityAsync(
        [Parent]
        BookingDetails item,
        [Service]
        IMarketplaceCancellationAvailabilityService cancellationAvailabilityService,
        CancellationToken cancellationToken)
    {
        var availability = await cancellationAvailabilityService.GetBookingAsync(item.Id, cancellationToken);
        return new MarketplaceCancellationAvailabilityDetails
        {
            CanCancel = availability.CanCancel,
            RequiresReason = availability.RequiresReason,
            IsPolicyOverride = availability.IsPolicyOverride,
            UnavailableReason = availability.UnavailableReason,
        };
    }
}
