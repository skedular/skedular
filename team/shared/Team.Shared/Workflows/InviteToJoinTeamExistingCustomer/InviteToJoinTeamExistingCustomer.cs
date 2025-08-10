using Team.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Team.Shared.Workflows.InviteToJoinTeamExistingCustomer;

public record InviteToJoinTeamExistingCustomerInput(string TeamId, string InviterCustomerId, string InviteeCustomerId);

[Workflow]
public class InviteToJoinTeamExistingCustomer
{
    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinTeamExistingCustomerInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendInviteCustomerToJoinTeamExistingCustomerAsync(
                    new SendInviteCustomerToJoinTeamExistingCustomerInput(args.TeamId, args.InviterCustomerId, args.InviteeCustomerId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
}
