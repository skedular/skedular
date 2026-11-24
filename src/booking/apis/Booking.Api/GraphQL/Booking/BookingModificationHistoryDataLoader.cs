using Booking.Api.Mappers;
using Booking.Api.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingModificationHistoryDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceBookingModificationService modificationService,
    IGraphQlMapper graphQlMapper)
    : BatchDataLoader<string, IReadOnlyList<MarketplaceBookingModificationDetails>>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, IReadOnlyList<MarketplaceBookingModificationDetails>>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var historiesByBookingId = await modificationService.GetHistoriesAsync(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            bookingId => bookingId,
            bookingId => (IReadOnlyList<MarketplaceBookingModificationDetails>)(
                historiesByBookingId.TryGetValue(bookingId, out var histories)
                    ? histories.Select(graphQlMapper.MapTo).ToList()
                    : []));
    }
}
