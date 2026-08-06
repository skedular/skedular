using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record MaintainOrganizationXeroConnectionInput(string OrganizationId, DateTimeOffset? NotBefore);

[Workflow]
public class MaintainOrganizationXeroConnection
{
    [WorkflowRun]
    public async Task ExecuteAsync(MaintainOrganizationXeroConnectionInput input)
    {
        if (input.NotBefore is not null)
        {
            var delay = input.NotBefore.Value - Workflow.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delay, Workflow.CancellationToken);
            }
        }

        var result = await Workflow.ExecuteActivityAsync(
            (XeroIntegrations activity) =>
                activity.RefreshOrganizationXeroConnectionAsync(new RefreshOrganizationXeroConnectionInput(input.OrganizationId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 3,
                    MaximumInterval = TimeSpan.FromMinutes(15),
                },
            });

        if (!result.ShouldContinue || result.NextRefreshAt is null)
        {
            return;
        }

        throw Workflow.CreateContinueAsNewException((MaintainOrganizationXeroConnection workflow) =>
            workflow.ExecuteAsync(new MaintainOrganizationXeroConnectionInput(input.OrganizationId, result.NextRefreshAt)));
    }
}
