using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.InviteToJoinOrganizationNewCustomer;

public record InviteToJoinOrganizationNewCustomerInput(string OrganizationId, string InviterCustomerId, string InviteeCustomerEmail);

[Workflow]
public class InviteToJoinOrganizationNewCustomer
{
    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinOrganizationNewCustomerInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendInviteCustomerToJoinOrganizationNewCustomerAsync(
                    new SendInviteCustomerToJoinOrganizationNewCustomerInput(args.OrganizationId, args.InviterCustomerId, args.InviteeCustomerEmail)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
}
