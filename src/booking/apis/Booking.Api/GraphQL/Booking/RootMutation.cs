using Booking.Api.Mappers;
using Booking.Api.Models;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<BookingPayload> AddPrivateBookingAsync(
        AddPrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.AddAsync(graphQlMapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> UpdatePrivateBookingAsync(
        UpdatePrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.UpdateAsync(
            new PrivateBookingPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
            cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> DeletePrivateBookingAsync(
        DeletePrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> AddMarketplaceBookingAsync(
        AddMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await marketplaceBookingService.AddAsync(graphQlMapper.MapTo(input), cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> UpdateMarketplaceBookingAsync(
        UpdateMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await marketplaceBookingService.UpdateAsync(
            new MarketplaceBookingPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
            cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> DeleteMarketplaceBookingAsync(
        DeleteMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await marketplaceBookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
    }
}
