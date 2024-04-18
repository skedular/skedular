using Api.Shared.Services.GraphQL.UnityHub.V1.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Enterprise.Shared.Context;

namespace Booking.Api.GraphQL;

public class BookingMutation(IMapper mapper) : Mutation
{
    public override async Task<BookingPayload?> AddBookingAsync(
        AddBookingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var booking = await service.AddAsync(mapper.MapTo(input), false, false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    public override async Task<BookingPayload?> UpdateBookingAsync(
        UpdateBookingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var booking = await service.UpdateAsync(mapper.MapTo(input), false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    public override async Task<BookingPayload?> DeleteBookingAsync(
        DeleteBookingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var booking = await service.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
