using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record InviteToJoinOrganizationInput(string JoinInvitationId, bool IsNewCustomer);

public record InviteToJoinOrganizationState(bool InvitationStateChanged);

[Workflow]
public class InviteToJoinOrganization
{
    private InviteToJoinOrganizationState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinOrganizationInput args)
    {
        _state = new InviteToJoinOrganizationState(false);

        // Step 1: Send invitation email
        if (args.IsNewCustomer)
        {
            await Workflow.ExecuteActivityAsync(
                (EmailIntegrations activity) => activity.SendInviteCustomerToJoinOrganizationNewCustomerAsync(args.JoinInvitationId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                });
        }
        else
        {
            await Workflow.ExecuteActivityAsync(
                (EmailIntegrations activity) => activity.SendInviteCustomerToJoinOrganizationExistingCustomerAsync(args.JoinInvitationId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                });
        }

        // Step 2: Wait for response (accept/reject/cancel) or a week
        var responded = await Workflow.WaitConditionAsync(() => _state.InvitationStateChanged, TimeSpan.FromDays(7));

        // Step 3: If no response after a week, expire the invitation
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
    public Task InvitationStatusChangedAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { InvitationStateChanged = true };
        return Task.CompletedTask;
    }
}
