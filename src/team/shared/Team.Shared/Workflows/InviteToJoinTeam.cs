using Team.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Team.Shared.Workflows;

public record InviteToJoinTeamInput(string JoinInvitationId, bool IsNewCustomer);

public record InviteToJoinTeamState(bool InvitationStateChanged);

[Workflow]
public class InviteToJoinTeam
{
    private InviteToJoinTeamState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(InviteToJoinTeamInput args)
    {
        _state = new InviteToJoinTeamState(false);

        // Step 1: Send invitation email
        if (args.IsNewCustomer)
        {
            await Workflow.ExecuteActivityAsync(
                (EmailIntegrations activity) => activity.SendInviteCustomerToJoinTeamNewCustomerAsync(args.JoinInvitationId),
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
        else
        {
            await Workflow.ExecuteActivityAsync(
                (EmailIntegrations activity) => activity.SendInviteCustomerToJoinTeamExistingCustomerAsync(args.JoinInvitationId),
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

        // Step 2: Wait for response (accept/reject/cancel) or a week
        var responded = await Workflow.WaitConditionAsync(() => _state.InvitationStateChanged, TimeSpan.FromDays(7));

        // Step 3: If no response after 2 weeks, expire the invitation
        if (!responded)
        {
            await Workflow.ExecuteActivityAsync((InvitationIntegrations activity) => activity.ExpireTeamInvitationAsync(args.JoinInvitationId),
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
    }

    [WorkflowSignal]
    public Task InvitationStatusChangedAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with
        {
            InvitationStateChanged = true,
        };
        return Task.CompletedTask;
    }
}
