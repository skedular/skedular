using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

public sealed class SubscriptionArrearsInvoicesDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
    IGraphQlMapper graphQlMapper)
    : BatchDataLoader<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var invoicesBySubscriptionId = await marketplaceBookingSubscriptionService
            .GetArrearsInvoicesAsync(keys, cancellationToken);

        return keys.Distinct().ToDictionary(
            subscriptionId => subscriptionId,
            subscriptionId => (IReadOnlyList<OrganizationArrearsInvoiceDetails>)(
                invoicesBySubscriptionId.TryGetValue(subscriptionId, out var invoices)
                    ? invoices.Select(graphQlMapper.MapTo).ToList()
                    : []));
    }
}
