using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Microsoft.Extensions.Logging;
using Team.Shared.Workflows;
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
    IWorkflowIdService workflowIdService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ILogger<TemporalOutboxService> logger) : ITemporalOutboxService
{
    private static readonly string s_inviteToJoinTeam = typeof(InviteToJoinTeam).ToWorkflowType();

    private static readonly string s_inviteToJoinTeamInvitationStatusChangedAsync =
        typeof(InviteToJoinTeam).GetMethod(nameof(InviteToJoinTeam.InvitationStatusChangedAsync))!.ToWorkflowSignalType();

    public void StartWorkflowInviteToJoin(InviteToJoinTeamInput args, IUnitOfWork unitOfWork)
    {
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinTeam, InviteToJoinTeamInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.InviteToJoin(args.JoinInvitationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

        logger.LogInformation("Temporal outbox enqueued invite-to-join workflow for invitation {JoinInvitationId}", args.JoinInvitationId);
    }

    public void SignalWorkflowInviteToJoinInvitationStatusChanged(string joinInvitationId, IUnitOfWork unitOfWork)
    {
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.InviteToJoin(joinInvitationId),
            s_inviteToJoinTeamInvitationStatusChangedAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

        logger.LogInformation("Temporal outbox enqueued invitation-status signal for invitation {JoinInvitationId}", joinInvitationId);
    }

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

                logger.LogInformation("Temporal workflow start dispatched for workflow {WorkflowId}", workflowOptions.Id);
            }
            catch (WorkflowAlreadyStartedException)
            {
                logger.LogInformation("Temporal workflow start skipped because workflow {WorkflowId} already exists", workflowOptions.Id);
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
                logger.LogInformation("Temporal signal skipped because workflow {WorkflowId} is not running", workflowId);
                return;
            }

            await temporalClient
                .GetWorkflowHandle<InviteToJoinTeam>(workflowId)
                .SignalAsync(workflow => workflow.InvitationStatusChangedAsync(), workflowSignalOptions);

            logger.LogInformation("Temporal signal dispatched for workflow {WorkflowId}", workflowId);
        }
    }
}
