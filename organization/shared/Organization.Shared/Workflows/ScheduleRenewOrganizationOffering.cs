using Organization.Shared.Workflows.Activities;
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

        _ = await Workflow.ExecuteActivityAsync(
            (OrganizationOfferings activity) =>
                activity.RenewAutoRenewableOrganizationOfferingAsync(new RenewAutoRenewableOrganizationOfferingAsyncInput(args.OrganizationId)),
            new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });
    }
}
