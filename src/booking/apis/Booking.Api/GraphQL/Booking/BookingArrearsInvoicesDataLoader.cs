using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Metrics;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingArrearsInvoicesDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>(batchScheduler, options, serviceScopeFactory, instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var invoicesByBookingId =
            await ExecuteInScopeAsync<IBookingService, IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>>(keys, service =>
                service.GetArrearsInvoicesAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            bookingId => bookingId,
            bookingId => (IReadOnlyList<OrganizationArrearsInvoiceDetails>)(
                invoicesByBookingId.TryGetValue(bookingId, out var invoices)
                    ? invoices.Select(graphQlMapper.MapTo).ToList()
                    : []));
    }
}
