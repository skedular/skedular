using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.RecurringBooking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionDetails")]
public class MarketplaceBookingSubscriptionDetails : Node
{
    [GraphQLName("startedAt")] public DateTimeOffset StartedAt { get; set; }
    [GraphQLName("cancelledAt")] public DateTimeOffset? CancelledAt { get; set; }
    [GraphQLName("nextRenewalAt")] public DateTimeOffset? NextRenewalAt { get; set; }
    [GraphQLName("status")] public MarketplaceBookingSubscriptionStatusDetails Status { get; set; } = new();
    [GraphQLName("autoRenew")] public bool AutoRenew { get; set; }
    [GraphQLName("cancelAtPeriodEnd")] public bool CancelAtPeriodEnd { get; set; }
    [GraphQLName("weeklySelectedDays")] public IEnumerable<DayOfWeek> WeeklySelectedDays { get; set; } = [];
    [GraphQLName("marketplaceBooking")] public MarketplaceBookingDetails MarketplaceBooking { get; set; } = new();
    [GraphQLName("recurringBookings")] public IEnumerable<RecurringBookingDetails> RecurringBookings { get; set; } = [];
    [GraphQLName("involvedCustomerIds")] public IEnumerable<string> InvolvedCustomerIds { get; set; } = [];

    [GraphQLName("involvedOrganizationIds")]
    public IEnumerable<(string Id, string CustomDomain)> InvolvedOrganizationIds { get; set; } = [];

    [GraphQLName("involvedTeamIds")] public IEnumerable<string> InvolvedTeamIds { get; set; } = [];
    [GraphQLName("createdByCustomerId")] public string? CreatedByCustomerId { get; set; }

    [GraphQLName("lastModifiedByCustomerId")]
    public string? LastModifiedByCustomerId { get; set; }

    [GraphQLName("deletedByCustomerId")] public string? DeletedByCustomerId { get; set; }

    public async Task<IEnumerable<OrganizationArrearsInvoiceDetails>> GetArrearsInvoices(
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        [Service] IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        (await marketplaceBookingSubscriptionService.GetArrearsInvoicesAsync(Id, cancellationToken)).Select(graphQlMapper.MapTo).ToList();
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

    public static Task<MarketplaceRefundDetails?> GetRefund(
        [Parent] MarketplaceBookingSubscriptionDetails item,
        [Service] IMarketplaceRefundReadService marketplaceRefundReadService,
        [Service] IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        MapRefundAsync(marketplaceRefundReadService.GetByMarketplaceBookingSubscriptionIdAsync(item.Id, cancellationToken), graphQlMapper);

    private static async Task<MarketplaceRefundDetails?> MapRefundAsync(Task<MarketplaceRefundReadModel?> task, IGraphQlMapper mapper)
    {
        var model = await task;
        return model is null ? null : mapper.MapTo(model);
    }

    public static async Task<MarketplaceBookingFailureDetails?> GetFailure(
        [Parent] MarketplaceBookingSubscriptionDetails item,
        [Service] IMarketplaceBookingFailureReadService failureReadService,
        [Service] IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var failure = await failureReadService.GetBySubscriptionIdAsync(item.Id, cancellationToken);
        return failure is null ? null : graphQlMapper.MapTo(failure);
    }
}
