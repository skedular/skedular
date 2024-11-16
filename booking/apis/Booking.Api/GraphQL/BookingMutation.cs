using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL;

public class BookingMutation
{
    [UseServiceScope]
    public async Task<BookingPayload?> AddBookingAsync(
        AddBookingInput input,
        [Service] IBookingService bookingService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.AddAsync(mapper.MapTo(input), false, false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseServiceScope]
    public async Task<BookingPayload?> UpdateBookingAsync(
        UpdateBookingInput input,
        [Service] IBookingService bookingService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.UpdateAsync(mapper.MapTo(input), false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseServiceScope]
    public async Task<BookingPayload?> DeleteBookingAsync(
        DeleteBookingInput input,
        [Service] IBookingService bookingService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
