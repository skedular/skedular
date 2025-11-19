using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationExistingCustomer;

public record InviteToJoinOrganizationExistingCustomerInput(string OrganizationId, string InviterCustomerId, string InviteeCustomerId);

[Workflow]
public class InviteToJoinOrganizationExistingCustomer
{
    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinOrganizationExistingCustomerInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendInviteCustomerToJoinOrganizationExistingCustomerAsync(
                    new SendInviteCustomerToJoinOrganizationExistingCustomerInput(args.OrganizationId, args.InviterCustomerId,
                        args.InviteeCustomerId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
}
