using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<BookingPayload> AddBookingAsync(
        AddBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> UpdateBookingAsync(
        UpdateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> DeleteBookingAsync(
        DeleteBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> BookProductAsync(
        BookProductInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await marketplaceBookingService.BookProductAsync(mapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
