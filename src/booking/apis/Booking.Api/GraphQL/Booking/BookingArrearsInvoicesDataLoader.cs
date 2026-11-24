using Booking.Api.Mappers;
using Booking.Api.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingArrearsInvoicesDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IBookingService bookingService,
    IGraphQlMapper graphQlMapper)
    : BatchDataLoader<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoiceDetails>>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var invoicesByBookingId = await bookingService.GetArrearsInvoicesAsync(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            bookingId => bookingId,
            bookingId => (IReadOnlyList<OrganizationArrearsInvoiceDetails>)(
                invoicesByBookingId.TryGetValue(bookingId, out var invoices)
                    ? invoices.Select(graphQlMapper.MapTo).ToList()
                    : []));
    }
}
