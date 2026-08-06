using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record NewOrganizationJoinedInput(string? OrganizationId, string? OrganizationCustomDomain);

[Workflow]
public class NewOrganizationJoined
{
    [WorkflowRun]
    public async Task ExecuteAsync(NewOrganizationJoinedInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendNewOrganizationJoinedEmailAsync(args.OrganizationId, args.OrganizationCustomDomain),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 3,
                    MaximumInterval = TimeSpan.FromMinutes(1),
                },
            });
}
