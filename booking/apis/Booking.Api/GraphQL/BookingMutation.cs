using Booking.Api.Mappers;
using Booking.Api.Services;
using Enterprise.Shared.Context;

namespace Booking.Api.GraphQL;

public class BookingMutation(IServiceProvider serviceProvider, IMapper mapper)
{
    public async Task<BookingPayload?> AddBookingAsync(AddBookingInput input, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var booking = await service.AddAsync(mapper.MapTo(input), false, false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    public async Task<BookingPayload?> UpdateBookingAsync(UpdateBookingInput input, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var booking = await service.UpdateAsync(mapper.MapTo(input), false, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }

    public async Task<BookingPayload?> DeleteBookingAsync(DeleteBookingInput input, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var booking = await service.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = mapper.MapTo(booking) };
    }
}
