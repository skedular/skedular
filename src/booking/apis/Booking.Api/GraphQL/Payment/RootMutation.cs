using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.RecurringBooking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Payment;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<BookingPayload> ConfirmBookingPaymentAsync(
        ConfirmBookingPaymentInput input,
        [Service]
        IBookingPaymentService bookingPaymentService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.ConfirmPaymentAsync(input.Id, cancellationToken);

        return new BookingPayload
        {
            ClientMutationId = input.ClientMutationId,
            Booking = graphQlMapper.MapTo(booking),
        };
    }

    [UseResolverScope]
    public async Task<BookingPayload> RejectBookingPaymentAsync(
        RejectBookingPaymentInput input,
        [Service]
        IBookingPaymentService bookingPaymentService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.RejectPaymentAsync(input.Id, cancellationToken);

        return new BookingPayload
        {
            ClientMutationId = input.ClientMutationId,
            Booking = graphQlMapper.MapTo(booking),
        };
    }

    [UseResolverScope]
    public async Task<BookingPayload> MakeBookingPaymentNotRequiredAsync(
        MakeBookingPaymentNotRequiredInput input,
        [Service]
        IBookingPaymentService bookingPaymentService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingPaymentService.MakePaymentNotRequiredAsync(input.Id, cancellationToken);

        return new BookingPayload
        {
            ClientMutationId = input.ClientMutationId,
            Booking = graphQlMapper.MapTo(booking),
        };
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> ConfirmRecurringBookingPaymentAsync(
        ConfirmRecurringBookingPaymentInput input,
        [Service]
        IRecurringBookingPaymentService recurringBookingPaymentService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await recurringBookingPaymentService.ConfirmPaymentAsync(input.Id, cancellationToken);

        return new RecurringBookingPayload
        {
            ClientMutationId = input.ClientMutationId,
            RecurringBooking = graphQlMapper.MapTo(recurringBooking)!,
        };
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> RejectRecurringBookingPaymentAsync(
        RejectRecurringBookingPaymentInput input,
        [Service]
        IRecurringBookingPaymentService recurringBookingPaymentService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await recurringBookingPaymentService.RejectPaymentAsync(input.Id, cancellationToken);

        return new RecurringBookingPayload
        {
            ClientMutationId = input.ClientMutationId,
            RecurringBooking = graphQlMapper.MapTo(recurringBooking)!,
        };
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> MakeRecurringBookingPaymentNotRequiredAsync(
        MakeRecurringBookingPaymentNotRequiredInput input,
        [Service]
        IRecurringBookingPaymentService recurringBookingPaymentService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await recurringBookingPaymentService.MakePaymentNotRequiredAsync(input.Id, cancellationToken);

        return new RecurringBookingPayload
        {
            ClientMutationId = input.ClientMutationId,
            RecurringBooking = graphQlMapper.MapTo(recurringBooking)!,
        };
    }
}
