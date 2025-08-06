namespace Booking.Shared.Activities;

public record ExecuteAllLocationResourcesSlotGenerationWorkflowsResponse(bool ShallContinue, ICollection<string> ResourceIds);