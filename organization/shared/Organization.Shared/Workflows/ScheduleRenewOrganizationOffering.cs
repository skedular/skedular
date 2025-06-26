using Organization.Shared.Workflows.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record ScheduleRenewOrganizationOfferingInput(string OrganizationId, string OrganizationOfferingId, DateTimeOffset RenewalDate);

public record OrganizationOfferingState(bool IsCancelled);

[Workflow]
public class ScheduleRenewOrganizationOffering
{
    private OrganizationOfferingState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(ScheduleRenewOrganizationOfferingInput args)
    {
        _state = new OrganizationOfferingState(false);

        var delayDuration = args.RenewalDate - TimeProvider.System.GetUtcNow();
        if (delayDuration > TimeSpan.Zero && await Workflow.WaitConditionAsync(() => _state.IsCancelled, delayDuration))
        {
            return;
        }

        await Workflow.ExecuteActivityAsync(
            (OrganizationOfferings activity) =>
                activity.PayForOrganizationOfferingAsync(new PayForOrganizationOffering(args.OrganizationOfferingId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromSeconds(30),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 6, MaximumInterval = TimeSpan.FromHours(4) }
            });

        await Workflow.ExecuteActivityAsync(
            (OrganizationOfferings activity) =>
                activity.RenewAutoRenewableOrganizationOfferingAsync(
                    new RenewAutoRenewableOrganizationOfferingAsyncInput(args.OrganizationId, args.OrganizationOfferingId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromSeconds(30),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
            });
    }

    [WorkflowSignal]
    public Task CancelOfferingAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = new OrganizationOfferingState(true);

        return Task.CompletedTask;
    }
}
