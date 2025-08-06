namespace Booking.Shared.Workflows.LocationResource;

public record LocationResourceSlotGenerationInput(string LocationId, DateTimeOffset? RegenerateTime);
