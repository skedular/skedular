using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

// Logging contract for this resolver class:
// LOG-001: LogInformation when a subscription list query is resolved with filter inputs — includes statuses count, paymentStatuses count, and result count.
// LOG-002: LogWarning when an unrecognised status or payment status value is submitted — includes the unrecognised value.
// LOG-003: LogInformation when filter option queries (MarketplaceBookingSubscriptionStatuses / MarketplaceBookingPaymentStatuses) resolve successfully — includes option count.

[QueryType]
public class RootQuery(IMapper mapper, ILogger<RootQuery> logger)
{
    [UseResolverScope]
    public IEnumerable<MarketplaceBookingSubscriptionCancellationModeDetails> MarketplaceBookingSubscriptionCancellationModes() =>
    [
        new()
        {
            Type = MarketplaceBookingSubscriptionCancellationMode.Immediate,
            Name = MarketplaceBookingSubscriptionCancellationMode.Immediate.ToMarketplaceBookingSubscriptionCancellationModeName()
        },
        new()
        {
            Type = MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd,
            Name = MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd.ToMarketplaceBookingSubscriptionCancellationModeName()
        }
    ];

    [UseResolverScope]
    public IEnumerable<MarketplaceBookingSubscriptionStatusDetails> MarketplaceBookingSubscriptionStatuses()
    {
        var statuses = new[]
            {
                MarketplaceBookingSubscriptionStatus.Active, MarketplaceBookingSubscriptionStatus.Cancelled,
                MarketplaceBookingSubscriptionStatus.Expired, MarketplaceBookingSubscriptionStatus.RenewalFailed,
                MarketplaceBookingSubscriptionStatus.Paused
            }
            .Select(status => new MarketplaceBookingSubscriptionStatusDetails
            {
                Type = status, Name = status.ToMarketplaceBookingSubscriptionStatusName()
            })
            .ToList();

        logger.LogInformation("MarketplaceBookingSubscriptionStatuses resolved {Count} subscription status options", statuses.Count);

        return statuses;
    }

    [UseResolverScope]
    public IEnumerable<MarketplaceBookingPaymentStatusDetails> MarketplaceBookingPaymentStatuses()
    {
        var statuses = new[]
            {
                PaymentStatus.NotSet, PaymentStatus.Pending, PaymentStatus.Rejected, PaymentStatus.Confirmed, PaymentStatus.Expired,
                PaymentStatus.NoPaymentRequired
            }
            .Select(status => new MarketplaceBookingPaymentStatusDetails { Type = status, Name = status.ToMarketplaceBookingPaymentStatusName() })
            .ToList();

        logger.LogInformation("MarketplaceBookingPaymentStatuses resolved {Count} payment status options", statuses.Count);

        return statuses;
    }

    [UseResolverScope]
    public async Task<MarketplaceBookingSubscriptionDetails?> MarketplaceBookingSubscriptionAsync(
        string id,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await marketplaceBookingSubscriptionService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<MarketplaceBookingSubscriptionDetails?> MarketplaceBookingSubscriptionByIdAsync(
        [ID] string id,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken) =>
        await MarketplaceBookingSubscriptionAsync(id, marketplaceBookingSubscriptionService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<MarketplaceBookingSubscriptionEdge>> MarketplaceBookingSubscriptionsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MarketplaceBookingSubscriptionWhereInput where,
        IEnumerable<MarketplaceBookingSubscriptionOrderInput>? orderBy,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        where.TeamIds = where.TeamIds.RemoveInvalidIds();
        where.CustomerIds = where.CustomerIds.RemoveInvalidIds();

        var statuses = where.Statuses.ToSafeCollection();
        var paymentStatuses = where.PaymentStatuses.ToSafeCollection();

        var definedStatuses = Enum.GetValues<MarketplaceBookingSubscriptionStatus>();
        var unrecognisedStatuses = statuses.Where(s => !definedStatuses.Contains(s)).ToList();
        if (unrecognisedStatuses.Count > 0)
        {
            logger.LogWarning(
                "MarketplaceBookingSubscriptionsAsync received {Count} unrecognised subscription status value(s): {Values}",
                unrecognisedStatuses.Count,
                string.Join(", ", unrecognisedStatuses));
        }

        var definedPaymentStatuses = Enum.GetValues<PaymentStatus>();
        var unrecognisedPaymentStatuses = paymentStatuses.Where(s => !definedPaymentStatuses.Contains(s)).ToList();
        if (unrecognisedPaymentStatuses.Count > 0)
        {
            logger.LogWarning(
                "MarketplaceBookingSubscriptionsAsync received {Count} unrecognised payment status value(s): {Values}",
                unrecognisedPaymentStatuses.Count,
                string.Join(", ", unrecognisedPaymentStatuses));
        }

        logger.LogInformation(
            "MarketplaceBookingSubscriptionsAsync queried with {StatusesCount} status filter(s) and {PaymentStatusesCount} payment status filter(s)",
            statuses.Count,
            paymentStatuses.Count);

        var (paginatedInfo, edges, totalCount) = await marketplaceBookingSubscriptionService.GetPaginatedMarketplaceBookingSubscriptionsAsync(
            new PaginationInputParam(after, first, before, last),
            new MarketplaceBookingSubscriptionSearchCriteria(
                where.StartedAtGt,
                where.StartedAtGte,
                where.StartedAtLt,
                where.StartedAtLte,
                where.CancelledAtGt,
                where.CancelledAtGte,
                where.CancelledAtLt,
                where.CancelledAtLte,
                where.NextRenewalAtGt,
                where.NextRenewalAtGte,
                where.NextRenewalAtLt,
                where.NextRenewalAtLte,
                where.NameContains,
                where.Status,
                where.IncludeMineOnly,
                where.OrganizationId,
                where.OrganizationCustomDomain,
                where.TeamIds.ToSafeCollection(),
                where.CustomerIds.ToSafeCollection(),
                statuses,
                paymentStatuses),
            orderBy.ToSafeCollection().Select(item => new MarketplaceBookingSubscriptionOrder(item.Direction, item.Field)),
            false,
            cancellationToken);

        logger.LogInformation(
            "MarketplaceBookingSubscriptionsAsync resolved {TotalCount} total subscriptions",
            totalCount);

        return new Connection<MarketplaceBookingSubscriptionEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<MarketplaceBookingSubscriptionDetails>> AllMarketplaceBookingSubscriptionsAsync(
        MarketplaceBookingSubscriptionWhereInput where,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        var result = await MarketplaceBookingSubscriptionsAsync(
            null,
            null,
            null,
            null,
            where,
            [],
            marketplaceBookingSubscriptionService,
            cancellationToken);
        return result.Edges.Select(item => item.Node);
    }
}
