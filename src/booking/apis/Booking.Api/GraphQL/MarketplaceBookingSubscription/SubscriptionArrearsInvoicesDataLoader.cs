using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using GreenDonut;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

public sealed class SubscriptionArrearsInvoicesDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var invoicesBySubscriptionId =
            await
                ExecuteInScopeAsync<IMarketplaceBookingSubscriptionService,
                    IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>>(service =>
                    service.GetArrearsInvoicesAsync(keys, cancellationToken));

        return keys.Distinct().ToDictionary(
            subscriptionId => subscriptionId,
            subscriptionId => (IReadOnlyList<OrganizationArrearsInvoiceDetails>)(
                invoicesBySubscriptionId.TryGetValue(subscriptionId, out var invoices)
                    ? invoices.Select(graphQlMapper.MapTo).ToList()
                    : []));
    }
}
