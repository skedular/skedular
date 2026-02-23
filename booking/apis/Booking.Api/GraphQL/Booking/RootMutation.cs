using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<BookingPayload> AddPrivateBookingAsync(
        AddPrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> UpdatePrivateBookingAsync(
        UpdatePrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> DeletePrivateBookingAsync(
        DeletePrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> AddPrivateRecurringBookingAsync(
        AddPrivateRecurringBookingInput input,
        [Service] IPrivateRecurringBookingService privateRecurringBookingService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await privateRecurringBookingService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new RecurringBookingPayload { ClientMutationId = input.ClientMutationId, RecurringBooking = mapper.MapTo(recurringBooking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> AddMarketplaceBookingAsync(
        AddMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await marketplaceBookingService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> UpdateMarketplaceBookingAsync(
        UpdateMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await marketplaceBookingService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> DeleteMarketplaceBookingAsync(
        DeleteMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await marketplaceBookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
