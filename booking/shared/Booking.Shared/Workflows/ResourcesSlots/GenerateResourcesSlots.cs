using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows.ResourcesSlots;

public record GenerateResourcesSlotsInput(ICollection<string> ResourceIds);

[Workflow]
public class GenerateResourcesSlots
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateResourcesSlotsInput args)
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
