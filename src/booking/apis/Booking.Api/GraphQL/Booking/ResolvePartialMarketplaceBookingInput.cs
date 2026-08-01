namespace Booking.Api.GraphQL.Booking;

public sealed class ResolvePartialMarketplaceBookingInput
{
    public string Id { get; set; } = null!;
    public string? ClientMutationId { get; set; }
}
