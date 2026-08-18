namespace Booking.Api.GraphQL.Booking;

public sealed class ResolvePartialMarketplaceBookingInput
{
    public string Id { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
