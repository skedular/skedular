using Location.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Location.Shared.Workflows;

public record RecomputeLocationBookingDerivedStateInput(string LocationId);

[Workflow]
public class RecomputeLocationBookingDerivedState
{
    private bool _recomputeRequested = true;

    [WorkflowRun]
    public async Task ExecuteAsync(RecomputeLocationBookingDerivedStateInput args)
    {
        while (true)
        {
            _recomputeRequested = false;

            await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

            await Workflow.ExecuteActivityAsync(
                (LocationBookingDerivedState activity) => activity.RecomputeAsync(args.LocationId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(10),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 3,
                        MaximumInterval = TimeSpan.FromMinutes(1),
                    },
                });

            if (_recomputeRequested)
            {
                continue;
            }

            if (!await Workflow.WaitConditionAsync(() => _recomputeRequested, TimeSpan.FromSeconds(30)))
            {
                return;
            }
        }
    }

    [WorkflowSignal]
    public Task BookingChangedAsync()
    {
        _recomputeRequested = true;

        return Task.CompletedTask;
    }
}
