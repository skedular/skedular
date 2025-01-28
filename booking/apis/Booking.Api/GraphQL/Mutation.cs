using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<BookingPayload?> AddBookingAsync(
        AddBookingInput input,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.AddAsync(mapper.MapTo(input), false, false, false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload?> UpdateBookingAsync(
        UpdateBookingInput input,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.UpdateAsync(mapper.MapTo(input), false, false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload?> DeleteBookingAsync(
        DeleteBookingInput input,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
