using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Team.Shared.Workflows;
using Team.Shared.Workflows.Invitation.InviteToJoinTeamExistingCustomer;
using Team.Shared.Workflows.Invitation.InviteToJoinTeamNewCustomer;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Team.Shared.Services;

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    void StartWorkflowInviteToJoinTeamExistingCustomer(InviteToJoinTeamExistingCustomerInput args, IUnitOfWork unitOfWork);
    void StartWorkflowInviteToJoinTeamNewCustomer(InviteToJoinTeamNewCustomerInput args, IUnitOfWork unitOfWork);

    void SignalWorkflowInviteToJoinTeamExistingCustomerInvitationStatusChanged(
        string teamId,
        string inviteeCustomerId,
        string inviterCustomerId,
        IUnitOfWork unitOfWork);
}

public class TemporalOutboxService(
    ITemporalClient temporalClient,
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor) : ITemporalOutboxService
{
    private static readonly string s_inviteToJoinTeamExistingCustomer = typeof(InviteToJoinTeamExistingCustomer).ToWorkflowType();
    private static readonly string s_inviteToJoinTeamNewCustomer = typeof(InviteToJoinTeamNewCustomer).ToWorkflowType();

    private static readonly string s_inviteToJoinTeamExistingCustomerInvitationStatusChangedAsync =
        typeof(InviteToJoinTeamExistingCustomer).GetMethod(nameof(InviteToJoinTeamExistingCustomer.InvitationStatusChangedAsync))!
            .ToWorkflowSignalType();

    public void StartWorkflowInviteToJoinTeamExistingCustomer(InviteToJoinTeamExistingCustomerInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinTeamExistingCustomer, InviteToJoinTeamExistingCustomerInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.InviteToTeamExistingCustomerPrefix}-{args.TeamId}-{args.InviteeCustomerId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowInviteToJoinTeamNewCustomer(InviteToJoinTeamNewCustomerInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinTeamNewCustomer, InviteToJoinTeamNewCustomerInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.InviteToTeamExistingCustomerPrefix}-{args.TeamId}-{args.InviteeCustomerEmail}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void SignalWorkflowInviteToJoinTeamExistingCustomerInvitationStatusChanged(
        string teamId,
        string inviteeCustomerId,
        string inviterCustomerId,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(
                $"{Constants.InviteToTeamExistingCustomerPrefix}-{teamId}-{inviteeCustomerId}-{inviterCustomerId}"),
            s_inviteToJoinTeamExistingCustomerInvitationStatusChangedAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_inviteToJoinTeamExistingCustomer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<InviteToJoinTeamExistingCustomerInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (InviteToJoinTeamExistingCustomer workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_inviteToJoinTeamNewCustomer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<InviteToJoinTeamNewCustomerInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((InviteToJoinTeamNewCustomer workflow) => workflow.ExecuteAsync(input), workflowOptions);
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
        if (signalType == s_inviteToJoinTeamExistingCustomerInvitationStatusChangedAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<InviteToJoinTeamExistingCustomer>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<InviteToJoinTeamExistingCustomer>(workflowId)
                .SignalAsync(workflow => workflow.InvitationStatusChangedAsync(), workflowSignalOptions);
        }
    }
}
