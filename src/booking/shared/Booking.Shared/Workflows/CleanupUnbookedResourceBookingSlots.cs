using Booking.Shared.Activities;
using Booking.Shared.Repositories;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

[Workflow]
public sealed class CleanupUnbookedResourceBookingSlots
{
    [WorkflowRun]
    public async Task ExecuteAsync()
    {
        while (true)
        {
            var cleanupResult = await Workflow.ExecuteActivityAsync(
                (ResourceBookingSlotCleanupIntegrations activity) => activity.DeleteUnbookedResourceBookingSlotsAsync(),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(5),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        InitialInterval = TimeSpan.FromSeconds(10),
                        MaximumAttempts = 0,
                        MaximumInterval = TimeSpan.FromMinutes(1),
                    },
                });

            if (cleanupResult >= IResourceBookingSlotRepository.UnbookedSlotCleanupBatchSize)
            {
                if (Workflow.ContinueAsNewSuggested)
                {
                    throw Workflow.CreateContinueAsNewException<CleanupUnbookedResourceBookingSlots>(
                        workflow => workflow.ExecuteAsync(),
                        new ContinueAsNewOptions
                        {
                            TaskQueue = Workflow.Info.TaskQueue,
                        });
                }

                continue;
            }

            // Keep this cleanup running on the same daily loop as the other recurring booking maintenance workflows.
            await Workflow.DelayAsync(TimeSpan.FromHours(24));

            if (Workflow.ContinueAsNewSuggested)
            {
                throw Workflow.CreateContinueAsNewException<CleanupUnbookedResourceBookingSlots>(
                    workflow => workflow.ExecuteAsync(),
                    new ContinueAsNewOptions
                    {
                        TaskQueue = Workflow.Info.TaskQueue,
                    });
            }
        }
    }
}
