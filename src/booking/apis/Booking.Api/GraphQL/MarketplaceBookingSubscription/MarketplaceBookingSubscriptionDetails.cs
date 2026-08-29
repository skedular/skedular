using System.Text;
using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.MarketplacePurchaseHistory;
using Booking.Api.GraphQL.RecurringBooking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionDetails")]
public class MarketplaceBookingSubscriptionDetails : Node
{
    [GraphQLName("startedAt")]
    public DateTimeOffset StartedAt { get; set; }

    [GraphQLName("cancelledAt")]
    public DateTimeOffset? CancelledAt { get; set; }

    [GraphQLName("nextRenewalAt")]
    public DateTimeOffset? NextRenewalAt { get; set; }

    [GraphQLName("status")]
    public MarketplaceBookingSubscriptionStatusDetails Status { get; set; } = new();

    [GraphQLName("autoRenew")]
    public bool AutoRenew { get; set; }

    [GraphQLName("cancelAtPeriodEnd")]
    public bool CancelAtPeriodEnd { get; set; }

    [GraphQLName("cancellationPolicyOverridden")]
    public bool CancellationPolicyOverridden { get; set; }

    [GraphQLName("cancellationOverrideReason")]
    public string? CancellationOverrideReason { get; set; }

    [GraphQLName("weeklySelectedDays")]
    public IEnumerable<DayOfWeek> WeeklySelectedDays { get; set; } = [];

    [GraphQLName("marketplaceBooking")]
    public MarketplaceBookingDetails MarketplaceBooking { get; set; } = new();

    [GraphQLName("recurringBookings")]
    public IEnumerable<RecurringBookingDetails> RecurringBookings { get; set; } = [];

    [GraphQLName("involvedCustomerIds")]
    public IEnumerable<string> InvolvedCustomerIds { get; set; } = [];

    [GraphQLName("involvedOrganizationIds")]
    public IEnumerable<(string Id, string CustomDomain)> InvolvedOrganizationIds { get; set; } = [];

    [GraphQLName("involvedTeamIds")]
    public IEnumerable<string> InvolvedTeamIds { get; set; } = [];

    [GraphQLName("createdByCustomerId")]
    public string? CreatedByCustomerId { get; set; }

    [GraphQLName("lastModifiedByCustomerId")]
    public string? LastModifiedByCustomerId { get; set; }

    [GraphQLName("deletedByCustomerId")]
    public string? DeletedByCustomerId { get; set; }

    [GraphQLName("history")]
    public async Task<Connection<MarketplacePurchaseHistoryEventEdge>> GetHistoryAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        [Service]
        IMarketplacePurchaseHistoryService historyService,
        CancellationToken cancellationToken) =>
        ToConnection(await historyService.GetEventsAsync(MarketplacePurchaseHistoryEligibleSourceType.Subscription, Id, cancellationToken), after,
            first, before, last);

    private static Connection<MarketplacePurchaseHistoryEventEdge> ToConnection(
        IReadOnlyList<MarketplacePurchaseHistoryEventModel> events,
        string? after,
        int? first,
        string? before,
        int? last)
    {
        var offset = after is null ? 0 : int.TryParse(after, out var parsedAfter) ? parsedAfter + 1 : 0;
        var count = first ?? Math.Max(events.Count - offset, 0);
        if (before is not null && int.TryParse(before, out var parsedBefore))
        {
            var endExclusive = Math.Clamp(parsedBefore, 0, events.Count);
            count = Math.Min(last ?? endExclusive, endExclusive);
            offset = Math.Max(endExclusive - count, 0);
        }

        var page = events.Skip(Math.Max(offset, 0)).Take(Math.Max(count, 0)).ToList();
        return new Connection<MarketplacePurchaseHistoryEventEdge>
        {
            Edges = page.Select((item, index) => new MarketplacePurchaseHistoryEventEdge(
                MarketplacePurchaseHistoryEventDetails.From(item), (offset + index).ToString())).ToList(),
            TotalCount = events.Count,
            PageInfo = new PageInfo
            {
                HasPreviousPage = offset > 0,
                HasNextPage = offset + page.Count < events.Count,
                StartCursor = page.Count == 0 ? null : offset.ToString(),
                EndCursor = page.Count == 0 ? null : (offset + page.Count - 1).ToString(),
            },
        };
    }

    [GraphQLName("bookingInstances")]
    [UseResolverScope]
    public async Task<Connection<MarketplaceBookingInstanceEdge>> GetBookingInstances(
        string? after,
        int? first,
        string? before,
        int? last,
        DateTimeOffset? from,
        DateTimeOffset? until,
        [Service]
        IMarketplaceBookingSubscriptionService service,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, entries, totalCount) = await service.GetPaginatedBookingInstancesAsync(
            Id, new PaginationInputParam(after, first, before, last), from, until, cancellationToken);
        var edges = entries.Select(item => new MarketplaceBookingInstanceEdge(graphQlMapper.MapTo(item.Node)!, item.Cursor)).ToList();
        return new Connection<MarketplaceBookingInstanceEdge>
        {
            Edges = edges,
            TotalCount = totalCount,
            PageInfo = new PageInfo
            {
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                HasNextPage = paginatedInfo.HasNextPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
        };
    }

    [GraphQLName("linkedBookings")]
    [UseResolverScope]
    public async Task<Connection<BookingEdge>> GetLinkedBookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        [Service]
        IBookingService bookingService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var organization = InvolvedOrganizationIds.FirstOrDefault();
        var recurringBookingIds = RecurringBookings
            .Select(item => item.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToArray();

        if (recurringBookingIds.Length == 0)
        {
            return new Connection<BookingEdge>
            {
                Edges = [],
                TotalCount = 0,
                PageInfo = new PageInfo
                {
                    HasPreviousPage = false,
                    HasNextPage = false,
                },
            };
        }

        var (paginatedInfo, entries, totalCount) = await bookingService.GetPaginatedBookingsAsync(
            new PaginationInputParam(after, first, before, last),
            new BookingSearchCriteria(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                [],
                null,
                null,
                organization.Id,
                organization.CustomDomain,
                [],
                [],
                [],
                recurringBookingIds,
                null),
            [new BookingOrder(OrderDirection.Ascending, BookingOrderField.From)],
            false,
            cancellationToken);

        return new Connection<BookingEdge>
        {
            Edges = entries.Select(graphQlMapper.MapTo),
            TotalCount = totalCount,
            PageInfo = new PageInfo
            {
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                HasNextPage = paginatedInfo.HasNextPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
        };
    }

    // Kept as a non-GraphQL compatibility helper for callers that construct this
    // details object directly. The GraphQL field above always uses the repository.
    [GraphQLIgnore]
    public Connection<MarketplaceBookingInstanceEdge> GetBookingInstances(
        string? after, int? first, string? before, int? last, DateTimeOffset? from, DateTimeOffset? until)
    {
        var entries = RecurringBookings
            .Where(item => !from.HasValue || item.EndDate >= from || item.StartDate >= from)
            .Where(item => !until.HasValue || item.StartDate <= until)
            .ToList();
        var start = string.IsNullOrWhiteSpace(after)
            ? 0
            : Math.Clamp(int.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(after)), out var index) ? index + 1 : 0, 0, entries.Count);
        var count = first ?? last ?? entries.Count;
        var page = entries.Skip(start).Take(count).ToList();
        var edges = page.Select((item, index) =>
            new MarketplaceBookingInstanceEdge(item, Convert.ToBase64String(Encoding.UTF8.GetBytes((start + index).ToString())))).ToList();
        return new Connection<MarketplaceBookingInstanceEdge>
        {
            Edges = edges,
            TotalCount = entries.Count,
            PageInfo = new PageInfo
            {
                HasPreviousPage = start > 0,
                HasNextPage = start + page.Count < entries.Count,
                StartCursor = edges.FirstOrDefault()?.Cursor,
                EndCursor = edges.LastOrDefault()?.Cursor,
            },
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<OrganizationArrearsInvoiceDetails>> GetArrearsInvoices(
        [Service]
        IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        [.. (await marketplaceBookingSubscriptionService.GetArrearsInvoicesAsync(Id, cancellationToken)).Select(graphQlMapper.MapTo)];
}

[ObjectType<MarketplaceBookingSubscriptionDetails>]
public static partial class MarketplaceBookingSubscriptionDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<MarketplaceBookingSubscriptionDetails> descriptor)
    {
        descriptor.Ignore(item => item.InvolvedCustomerIds);
        descriptor.Ignore(item => item.CreatedByCustomerId);
        descriptor.Ignore(item => item.LastModifiedByCustomerId);
        descriptor.Ignore(item => item.DeletedByCustomerId);
        descriptor.Ignore(item => item.InvolvedOrganizationIds);
        descriptor.Ignore(item => item.InvolvedTeamIds);
    }

    public static IEnumerable<CustomerDetails> GetInvolvedCustomers([Parent] MarketplaceBookingSubscriptionDetails item) =>
        item.InvolvedCustomerIds.Select(id => new CustomerDetails(id));

    public static CustomerDetails? GetCreatedByCustomer([Parent] MarketplaceBookingSubscriptionDetails item) =>
        string.IsNullOrWhiteSpace(item.CreatedByCustomerId) ? null : new CustomerDetails(item.CreatedByCustomerId);

    public static CustomerDetails? GetLastModifiedByCustomer([Parent] MarketplaceBookingSubscriptionDetails item) =>
        string.IsNullOrWhiteSpace(item.LastModifiedByCustomerId) ? null : new CustomerDetails(item.LastModifiedByCustomerId);

    public static CustomerDetails? GetDeletedByCustomer([Parent] MarketplaceBookingSubscriptionDetails item) =>
        string.IsNullOrWhiteSpace(item.DeletedByCustomerId) ? null : new CustomerDetails(item.DeletedByCustomerId);

    public static IEnumerable<OrganizationDetails> GetInvolvedOrganizations([Parent] MarketplaceBookingSubscriptionDetails item) =>
        item.InvolvedOrganizationIds.Select(tuple => new OrganizationDetails(tuple.Id, tuple.CustomDomain));

    public static IEnumerable<TeamDetails> GetInvolvedTeams([Parent] MarketplaceBookingSubscriptionDetails item) =>
        item.InvolvedTeamIds.Select(id => new TeamDetails(id));

    [UseResolverScope]
    public static Task<MarketplaceRefundDetails?> GetRefund(
        [Parent]
        MarketplaceBookingSubscriptionDetails item,
        [Service]
        IMarketplaceRefundReadService marketplaceRefundReadService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        MapRefundAsync(marketplaceRefundReadService.GetByMarketplaceBookingSubscriptionIdAsync(item.Id, cancellationToken), graphQlMapper);

    private static async Task<MarketplaceRefundDetails?> MapRefundAsync(Task<MarketplaceRefundReadModel?> task, IGraphQlMapper mapper)
    {
        var model = await task;
        return model is null ? null : mapper.MapTo(model);
    }

    [UseResolverScope]
    public static async Task<MarketplaceBookingFailureDetails?> GetFailure(
        [Parent]
        MarketplaceBookingSubscriptionDetails item,
        [Service]
        IMarketplaceBookingFailureReadService failureReadService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var failure = await failureReadService.GetBySubscriptionIdAsync(item.Id, cancellationToken);
        return failure is null ? null : graphQlMapper.MapTo(failure);
    }

    [UseResolverScope]
    public static async Task<MarketplaceSubscriptionCancellationAvailabilityDetails> GetCancellationAvailabilityAsync(
        [Parent]
        MarketplaceBookingSubscriptionDetails item,
        [Service]
        IMarketplaceCancellationAvailabilityService cancellationAvailabilityService,
        CancellationToken cancellationToken)
    {
        var availability = await cancellationAvailabilityService.GetSubscriptionAsync(item.Id, cancellationToken);
        return new MarketplaceSubscriptionCancellationAvailabilityDetails
        {
            Immediate = new MarketplaceCancellationAvailabilityDetails
            {
                CanCancel = availability.Immediate.CanCancel,
                RequiresReason = availability.Immediate.RequiresReason,
                IsPolicyOverride = availability.Immediate.IsPolicyOverride,
                UnavailableReason = availability.Immediate.UnavailableReason,
            },
            AtPeriodEnd = new MarketplaceCancellationAvailabilityDetails
            {
                CanCancel = availability.AtPeriodEnd.CanCancel,
                RequiresReason = availability.AtPeriodEnd.RequiresReason,
                IsPolicyOverride = availability.AtPeriodEnd.IsPolicyOverride,
                UnavailableReason = availability.AtPeriodEnd.UnavailableReason,
            },
        };
    }
}
