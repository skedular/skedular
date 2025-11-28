using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationExistingCustomer;

public record InviteToJoinOrganizationExistingCustomerInput(string OrganizationId, string InviteeCustomerId, string InviterCustomerId);

public record InviteToJoinOrganizationExistingCustomerState(bool InvitationStateChanged);

[Workflow]
public class InviteToJoinOrganizationExistingCustomer
{
    private InviteToJoinOrganizationExistingCustomerState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinOrganizationExistingCustomerInput args)
    {
        _state = new InviteToJoinOrganizationExistingCustomerState(false);

        // Step 1: Send invitation email
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendInviteCustomerToJoinOrganizationExistingCustomerAsync(
                    args.OrganizationId,
                    args.InviteeCustomerId,
                    args.InviterCustomerId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromSeconds(30),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });

        // Step 2: Wait for response (accept/reject/cancel) or a week
        var responded = await Workflow.WaitConditionAsync(() => _state.InvitationStateChanged, TimeSpan.FromDays(7));

        // Step 3: If no response after 2 weeks, expire the invitation
        if (!responded)
        {
            await Workflow.ExecuteActivityAsync(
                (InvitationIntegrations activity) =>
                    activity.ExpireInvitationAsync(args.OrganizationId, args.InviterCustomerId, args.InviteeCustomerId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                });
        }
    }

    [WorkflowSignal]
    public Task InvitationStatusChangedAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { InvitationStateChanged = true };
        return Task.CompletedTask;
    }
}
