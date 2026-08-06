using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record RecomputeOrganizationBookingDerivedStateInput(string OrganizationId);

[Workflow]
public class RecomputeOrganizationBookingDerivedState
{
    private bool _recomputeRequested = true;

    [WorkflowRun]
    public async Task ExecuteAsync(RecomputeOrganizationBookingDerivedStateInput args)
    {
        while (true)
        {
            _recomputeRequested = false;

            await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

            await Workflow.ExecuteActivityAsync(
                (OrganizationBookingDerivedState activity) => activity.RecomputeAsync(args.OrganizationId),
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
