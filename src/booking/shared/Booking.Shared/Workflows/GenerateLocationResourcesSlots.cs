using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record GenerateLocationResourcesSlotsInput(string LocationId, DateTimeOffset? RegenerateTime);

[Workflow]
public class GenerateLocationResourcesSlots
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateLocationResourcesSlotsInput args)
    {
        if (args.RegenerateTime.HasValue)
        {
            var delayDuration = args.RegenerateTime.Value - Workflow.UtcNow;
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

        throw Workflow.CreateContinueAsNewException((GenerateLocationResourcesSlots workflow) =>
            workflow.ExecuteAsync(new GenerateLocationResourcesSlotsInput(args.LocationId, Workflow.UtcNow.AddDays(1))));
    }
}
