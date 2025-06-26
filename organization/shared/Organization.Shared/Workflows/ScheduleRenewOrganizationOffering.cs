using Organization.Shared.Workflows.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record ScheduleRenewOrganizationOfferingInput(string OrganizationId, string OrganizationOfferingId, DateTimeOffset RenewalDate);

[Workflow]
public class ScheduleRenewOrganizationOffering
{
    [WorkflowRun]
    public async Task ExecuteAsync(ScheduleRenewOrganizationOfferingInput args)
    {
        var delayDuration = args.RenewalDate - TimeProvider.System.GetUtcNow();
        if (delayDuration > TimeSpan.Zero)
        {
            await Workflow.DelayAsync(delayDuration);
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
}
