using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Organization.Shared.Services;

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork);
    void StartWorkflowInviteToJoin(InviteToJoinOrganizationInput args, IUnitOfWork unitOfWork);
    void StartWorkflowOrganizationDailyAnalytics(GenerateOrganizationDailyAnalyticsInput args, IUnitOfWork unitOfWork);
    void StartWorkflowMaintainOrganizationXeroConnection(MaintainOrganizationXeroConnectionInput args, IUnitOfWork unitOfWork);
    void StartWorkflowReSyncAzureTenant(ReSyncAzureTenantInput args, IUnitOfWork unitOfWork);
    void StartWorkflowNewOrganizationJoined(NewOrganizationJoinedInput args, IUnitOfWork unitOfWork);

    void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork);
    void SignalWorkflowInviteToJoinInvitationStatusChanged(string joinInvitationId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxService(
    ITemporalClient temporalClient,
    IWorkflowIdService workflowIdService,
    ITemporalHelperService temporalHelperService,
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor) : ITemporalOutboxService
{
    private static readonly string s_renewOrganizationOfferingType = typeof(ScheduleRenewOrganizationOffering).ToWorkflowType();
    private static readonly string s_addOrganizationStripePaymentMethodType = typeof(AddOrganizationStripePaymentMethod).ToWorkflowType();
    private static readonly string s_inviteToJoinOrganizationExistingCustomer = typeof(InviteToJoinOrganization).ToWorkflowType();
    private static readonly string s_generateOrganizationDailyAnalytics = typeof(GenerateOrganizationDailyAnalytics).ToWorkflowType();
    private static readonly string s_maintainOrganizationXeroConnection = typeof(MaintainOrganizationXeroConnection).ToWorkflowType();
    private static readonly string s_reSyncAzureTenant = typeof(ReSyncAzureTenant).ToWorkflowType();
    private static readonly string s_newOrganizationJoined = typeof(NewOrganizationJoined).ToWorkflowType();

    private static readonly string s_scheduleRenewOrganizationOfferingCancelOfferingAsync =
        typeof(ScheduleRenewOrganizationOffering).GetMethod(nameof(ScheduleRenewOrganizationOffering.CancelOfferingAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_inviteToJoinOrganizationInvitationStatusChangedAsync =
        typeof(InviteToJoinOrganization).GetMethod(nameof(InviteToJoinOrganization.InvitationStatusChangedAsync))!
            .ToWorkflowSignalType();

    public void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<ScheduleRenewOrganizationOffering, ScheduleRenewOrganizationOfferingInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.ScheduleRenewOrganizationOffering(args.OrganizationOfferingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void StartWorkflowInviteToJoin(InviteToJoinOrganizationInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinOrganization, InviteToJoinOrganizationInput>(
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

    public void StartWorkflowOrganizationDailyAnalytics(GenerateOrganizationDailyAnalyticsInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<GenerateOrganizationDailyAnalytics, GenerateOrganizationDailyAnalyticsInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.GenerateOrganizationDailyAnalytics(args.OrganizationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void StartWorkflowMaintainOrganizationXeroConnection(MaintainOrganizationXeroConnectionInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<MaintainOrganizationXeroConnection, MaintainOrganizationXeroConnectionInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.MaintainOrganizationXeroConnection(args.OrganizationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void StartWorkflowReSyncAzureTenant(ReSyncAzureTenantInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<ReSyncAzureTenant, ReSyncAzureTenantInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.ReSyncAzureTenant(args.TenantId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void StartWorkflowNewOrganizationJoined(NewOrganizationJoinedInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<NewOrganizationJoined, NewOrganizationJoinedInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.NewOrganizationJoined(args.OrganizationId, args.OrganizationCustomDomain),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.ScheduleRenewOrganizationOffering(offeringId),
            s_scheduleRenewOrganizationOfferingCancelOfferingAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowInviteToJoinInvitationStatusChanged(string joinInvitationId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.InviteToJoin(joinInvitationId),
            s_inviteToJoinOrganizationInvitationStatusChangedAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_renewOrganizationOfferingType)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<ScheduleRenewOrganizationOfferingInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (ScheduleRenewOrganizationOffering workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_addOrganizationStripePaymentMethodType)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<AddOrganizationStripePaymentMethodInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (AddOrganizationStripePaymentMethod workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_inviteToJoinOrganizationExistingCustomer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<InviteToJoinOrganizationInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (InviteToJoinOrganization workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_generateOrganizationDailyAnalytics)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<GenerateOrganizationDailyAnalyticsInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((GenerateOrganizationDailyAnalytics workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_maintainOrganizationXeroConnection)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<MaintainOrganizationXeroConnectionInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((MaintainOrganizationXeroConnection workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_reSyncAzureTenant)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<ReSyncAzureTenantInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((ReSyncAzureTenant workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_newOrganizationJoined)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<NewOrganizationJoinedInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((NewOrganizationJoined workflow) => workflow.ExecuteAsync(input), workflowOptions);
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
        if (signalType == s_scheduleRenewOrganizationOfferingCancelOfferingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<ScheduleRenewOrganizationOffering>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<ScheduleRenewOrganizationOffering>(workflowId)
                .SignalAsync(workflow => workflow.CancelOfferingAsync(), workflowSignalOptions);
        }
        else if (signalType == s_inviteToJoinOrganizationInvitationStatusChangedAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<InviteToJoinOrganization>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<InviteToJoinOrganization>(workflowId)
                .SignalAsync(workflow => workflow.InvitationStatusChangedAsync(), workflowSignalOptions);
        }
    }
}
