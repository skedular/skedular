using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows.LocationResource;

public record ResourceSlotGenerationInput(ICollection<string> ResourceIds);

[Workflow]
public class ResourceSlotGeneration
{
    [WorkflowRun]
    public async Task ExecuteAsync(ResourceSlotGenerationInput args)
    {
        foreach (var resourceId in args.ResourceIds)
        {
            await Workflow.ExecuteActivityAsync(
                (LocationResourceSlot activity) => activity.GenerateMissingResourceSlotsAsync(resourceId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
        }
    }
}
