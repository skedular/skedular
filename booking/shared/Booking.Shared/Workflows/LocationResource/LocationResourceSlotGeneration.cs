using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows.LocationResource;

[Workflow]
public class LocationResourceSlotGeneration
{
    [WorkflowRun]
    public async Task ExecuteAsync(LocationResourceSlotGenerationInput args)
    {
        if (args.RegenerateTime.HasValue)
        {
            var delayDuration = args.RegenerateTime.Value - TimeProvider.System.GetUtcNow();
            if (delayDuration > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delayDuration, Workflow.CancellationToken);
            }
        }

        var response = await Workflow.ExecuteActivityAsync(
            (LocationResourceSlot activity) => activity.ExecuteAllLocationResourcesSlotGenerationWorkflowsAsync(args.LocationId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromSeconds(30),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
            });

        if (!response.ShallContinue)
        {
            return;
        }

        foreach (var resourceId in response.ResourceIds)
        {
            await Workflow.ExecuteActivityAsync(
                (LocationResourceSlot activity) => activity.GenerateMissingResourceSlotsAsync(resourceId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
        }

        await Workflow.ExecuteActivityAsync(
            (LocationResourceSlot activity) => activity.ExecuteNextLocationResourcesSlotGenerationWorkflowAsync(args.LocationId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromSeconds(30),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
            });
    }
}
