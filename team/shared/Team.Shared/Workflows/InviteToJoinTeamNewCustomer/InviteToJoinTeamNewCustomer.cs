using Team.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Team.Shared.Workflows.InviteToJoinTeamNewCustomer;

public record InviteToJoinTeamNewCustomerInput(string TeamId, string InviterCustomerId, string InviteeCustomerEmail);

[Workflow]
public class InviteToJoinTeamNewCustomer
{
    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinTeamNewCustomerInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendInviteCustomerToJoinTeamNewCustomerAsync(
                    new SendInviteCustomerToJoinTeamNewCustomerInput(args.TeamId, args.InviterCustomerId, args.InviteeCustomerEmail)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
}
