using Team.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Team.Shared.Workflows.Invitation.InviteToJoinTeamExistingCustomer;

public record InviteToJoinTeamExistingCustomerInput(string JoinInvitationId, string TeamId, string InviterCustomerId, string InviteeCustomerId);

public record InviteToJoinTeamExistingCustomerState(bool InvitationStateChanged);

[Workflow]
public class InviteToJoinTeamExistingCustomer
{
    private InviteToJoinTeamExistingCustomerState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinTeamExistingCustomerInput args)
    {
        _state = new InviteToJoinTeamExistingCustomerState(false);

        // Step 1: Send invitation email
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) =>
                activity.SendInviteCustomerToJoinTeamExistingCustomerAsync(args.TeamId, args.InviterCustomerId, args.InviteeCustomerId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });

        // Step 2: Wait for response (accept/reject/cancel) or a week
        var responded = await Workflow.WaitConditionAsync(() => _state.InvitationStateChanged, TimeSpan.FromDays(7));

        // Step 3: If no response after 2 weeks, expire the invitation
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
