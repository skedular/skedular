using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationNewCustomer;

public record InviteToJoinOrganizationNewCustomerInput(
    string JoinInvitationId,
    string OrganizationId,
    string InviterCustomerId,
    string InviteeCustomerEmail);

public record InviteToJoinOrganizationNewCustomerState(bool InvitationNewCustomerStateChanged);

[Workflow]
public class InviteToJoinOrganizationNewCustomer
{
    private InviteToJoinOrganizationNewCustomerState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinOrganizationNewCustomerInput args)
    {
        _state = new InviteToJoinOrganizationNewCustomerState(false);
        // Step 1: Send invitation email
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendInviteCustomerToJoinOrganizationNewCustomerAsync(args.OrganizationId, args.InviterCustomerId, args.InviteeCustomerEmail),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });

        // Step 2: Wait for response (accept/reject/cancel) or a week
        var responded = await Workflow.WaitConditionAsync(() => _state.InvitationNewCustomerStateChanged, TimeSpan.FromDays(7));

        //Step3: If no response after a week, expire the invitation
        if (!responded)
        {
            await Workflow.ExecuteActivityAsync((InvitationIntegrations activity) => activity.ExpireInvitationAsync(args.JoinInvitationId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                });
        }
    }

    [WorkflowSignal]
    public Task InvitationStatusNewCustomerChangedAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);
        _state = _state with { InvitationNewCustomerStateChanged = true };
        return Task.CompletedTask;
    }
}
