using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.RecurringBooking;

[MutationType]
public class RootMutation(IMapper mapper)
{
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
    public async Task<RecurringBookingPayload> UpdatePrivateRecurringBookingAsync(
        UpdatePrivateRecurringBookingInput input,
        [Service] IPrivateRecurringBookingService privateRecurringBookingService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await privateRecurringBookingService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new RecurringBookingPayload { ClientMutationId = input.ClientMutationId, RecurringBooking = mapper.MapTo(recurringBooking) };
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> DeletePrivateRecurringBookingAsync(
        DeletePrivateRecurringBookingInput input,
        [Service] IPrivateRecurringBookingService privateRecurringBookingService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await privateRecurringBookingService.DeleteAsync(input.Id, cancellationToken);
        return new RecurringBookingPayload { ClientMutationId = input.ClientMutationId, RecurringBooking = mapper.MapTo(recurringBooking) };
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> AddMarketplaceRecurringBookingAsync(
        AddMarketplaceRecurringBookingInput input,
        [Service] IMarketplaceRecurringBookingService marketplaceRecurringBookingService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await marketplaceRecurringBookingService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new RecurringBookingPayload { ClientMutationId = input.ClientMutationId, RecurringBooking = mapper.MapTo(recurringBooking) };
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> DeleteMarketplaceRecurringBookingAsync(
        DeleteMarketplaceRecurringBookingInput input,
        [Service] IMarketplaceRecurringBookingService marketplaceRecurringBookingService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await marketplaceRecurringBookingService.DeleteAsync(input.Id, cancellationToken);
        return new RecurringBookingPayload { ClientMutationId = input.ClientMutationId, RecurringBooking = mapper.MapTo(recurringBooking) };
    }
}
