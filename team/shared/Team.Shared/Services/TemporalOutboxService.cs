using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Team.Shared.Workflows.InviteToJoinTeam;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Team.Shared.Services;

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    void StartWorkflowInviteToJoin(InviteToJoinTeamInput args, IUnitOfWork unitOfWork);
    void SignalWorkflowInviteToJoinInvitationStatusChanged(string joinInvitationId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxService(
    ITemporalClient temporalClient,
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor) : ITemporalOutboxService
{
    private static readonly string s_inviteToJoinTeam = typeof(InviteToJoinTeam).ToWorkflowType();

    private static readonly string s_inviteToJoinTeamInvitationStatusChangedAsync =
        typeof(InviteToJoinTeam).GetMethod(nameof(InviteToJoinTeam.InvitationStatusChangedAsync))!.ToWorkflowSignalType();

    public void StartWorkflowInviteToJoin(InviteToJoinTeamInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinTeam, InviteToJoinTeamInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.JoinInvitationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void SignalWorkflowInviteToJoinInvitationStatusChanged(string joinInvitationId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(joinInvitationId),
            s_inviteToJoinTeamInvitationStatusChangedAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_inviteToJoinTeam)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<InviteToJoinTeamInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (InviteToJoinTeam workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }

    public async Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();
        if (signalType == s_inviteToJoinTeamInvitationStatusChangedAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<InviteToJoinTeam>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<InviteToJoinTeam>(workflowId)
                .SignalAsync(workflow => workflow.InvitationStatusChangedAsync(), workflowSignalOptions);
        }
    }
}
