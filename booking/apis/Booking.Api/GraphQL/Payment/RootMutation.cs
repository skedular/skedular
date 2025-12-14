using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.GraphQl;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Payment;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<BookingPayload> ConfirmBookingPaymentAsync(
        ConfirmBookingPaymentInput input,
        [Service] IBookingPaymentService bookingPaymentService,
        [Service] ITopicEventSender topicEventSender,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.ConfirmPaymentAsync(input.Id, cancellationToken);

        await topicEventSender.SendAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> RejectBookingPaymentAsync(
        RejectBookingPaymentInput input,
        [Service] IBookingPaymentService bookingPaymentService,
        [Service] ITopicEventSender topicEventSender,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.RejectPaymentAsync(input.Id, cancellationToken);

        await topicEventSender.SendAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> MakeBookingPaymentNotRequiredAsync(
        MakeBookingPaymentNotRequiredInput input,
        [Service] IBookingPaymentService bookingPaymentService,
        [Service] ITopicEventSender topicEventSender,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.MakePaymentNotRequiredAsync(input.Id, cancellationToken);

        await topicEventSender.SendAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
