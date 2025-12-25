using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Payment;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<BookingPayload> ConfirmBookingPaymentAsync(
        ConfirmBookingPaymentInput input,
        [Service] IBookingPaymentService bookingPaymentService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.ConfirmPaymentAsync(input.Id, cancellationToken);

        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> RejectBookingPaymentAsync(
        RejectBookingPaymentInput input,
        [Service] IBookingPaymentService bookingPaymentService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.RejectPaymentAsync(input.Id, cancellationToken);

        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> MakeBookingPaymentNotRequiredAsync(
        MakeBookingPaymentNotRequiredInput input,
        [Service] IBookingPaymentService bookingPaymentService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.MakePaymentNotRequiredAsync(input.Id, cancellationToken);

        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
