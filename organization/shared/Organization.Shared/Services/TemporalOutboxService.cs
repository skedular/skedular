using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows;
using Organization.Shared.Workflows.AddPayment;
using Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationExistingCustomer;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationNewCustomer;
using Organization.Shared.Workflows.NewOrganizationJoined;
using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Organization.Shared.Services;

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork);
    void StartWorkflowInviteToJoinOrganizationExistingCustomer(InviteToJoinOrganizationExistingCustomerInput args, IUnitOfWork unitOfWork);
    void StartWorkflowOrganizationDailyAnalytics(GenerateOrganizationDailyAnalyticsInput args, IUnitOfWork unitOfWork);
    void StartWorkflowReSyncAzureTenant(ReSyncAzureTenantInput args, IUnitOfWork unitOfWork);
    void StartWorkflowInviteToJoinOrganizationNewCustomer(InviteToJoinOrganizationNewCustomerInput args, IUnitOfWork unitOfWork);
    void StartWorkflowNewOrganizationJoined(NewOrganizationJoinedInput args, IUnitOfWork unitOfWork);
    void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork);

    void SignalWorkflowInviteToJoinOrganizationExistingCustomerInvitationStatusChanged(
        string organizationId,
        string inviteeCustomerId,
        string inviterCustomerId,
        IUnitOfWork unitOfWork);

    void SignalWorkflowInviteToJoinOrganizationNewCustomerInvitationStatusChanged(
        string organizationId,
        string inviterCustomerId,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork);
}

public class TemporalOutboxService(
    ITemporalClient temporalClient,
    ITemporalHelperService temporalHelperService,
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor) : ITemporalOutboxService
{
    private static readonly string s_renewOrganizationOfferingType = typeof(ScheduleRenewOrganizationOffering).ToWorkflowType();
    private static readonly string s_addOrganizationStripePaymentMethodType = typeof(AddOrganizationStripePaymentMethod).ToWorkflowType();
    private static readonly string s_inviteToJoinOrganizationExistingCustomer = typeof(InviteToJoinOrganizationExistingCustomer).ToWorkflowType();
    private static readonly string s_inviteToJoinOrganizationNewCustomer = typeof(InviteToJoinOrganizationNewCustomer).ToWorkflowType();
    private static readonly string s_generateOrganizationDailyAnalytics = typeof(GenerateOrganizationDailyAnalytics).ToWorkflowType();
    private static readonly string s_reSyncAzureTenant = typeof(ReSyncAzureTenant).ToWorkflowType();
    private static readonly string s_newOrganizationJoined = typeof(NewOrganizationJoined).ToWorkflowType();

    private static readonly string s_scheduleRenewOrganizationOfferingCancelOfferingAsync =
        typeof(ScheduleRenewOrganizationOffering).GetMethod(nameof(ScheduleRenewOrganizationOffering.CancelOfferingAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_inviteToJoinOrganizationExistingCustomerInvitationStatusChangedAsync =
        typeof(InviteToJoinOrganizationExistingCustomer).GetMethod(nameof(InviteToJoinOrganizationExistingCustomer.InvitationStatusChangedAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_inviteToJoinOrganizationNewCustomerInvitationStatusChangedAsync =
        typeof(InviteToJoinOrganizationNewCustomer).GetMethod(nameof(InviteToJoinOrganizationNewCustomer.InvitationStatusNewCustomerChangedAsync))!
            .ToWorkflowSignalType();

    public void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<ScheduleRenewOrganizationOffering, ScheduleRenewOrganizationOfferingInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.OrganizationOfferingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void StartWorkflowInviteToJoinOrganizationExistingCustomer(InviteToJoinOrganizationExistingCustomerInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinOrganizationExistingCustomer, InviteToJoinOrganizationExistingCustomerInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(
                    $"{Constants.InviteToOrganizationExistingCustomerPrefix}-{args.OrganizationId}-{args.InviteeCustomerId}-{args.InviterCustomerId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void StartWorkflowOrganizationDailyAnalytics(GenerateOrganizationDailyAnalyticsInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<GenerateOrganizationDailyAnalytics, GenerateOrganizationDailyAnalyticsInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.GenerateOrganizationDailyAnalyticsPrefix}-{args.OrganizationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void StartWorkflowReSyncAzureTenant(ReSyncAzureTenantInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<ReSyncAzureTenant, ReSyncAzureTenantInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.ReSyncAzureTenantPrefix}-{args.TenantId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void StartWorkflowInviteToJoinOrganizationNewCustomer(InviteToJoinOrganizationNewCustomerInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinOrganizationNewCustomer, InviteToJoinOrganizationNewCustomerInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(
                    $"{Constants.InviteToOrganizationNewCustomerPrefix}-{args.OrganizationId}-{args.InviterCustomerId}-{args.InviteeCustomerEmail}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void StartWorkflowNewOrganizationJoined(NewOrganizationJoinedInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<NewOrganizationJoined, NewOrganizationJoinedInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(
                    $"{Constants.NewOrganizationJoinedPrefix}-{args.OrganizationId ?? string.Empty}-{args.OrganizationUniqueAlphanumericName ?? string.Empty}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(offeringId),
            s_scheduleRenewOrganizationOfferingCancelOfferingAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowInviteToJoinOrganizationExistingCustomerInvitationStatusChanged(
        string organizationId,
        string inviteeCustomerId,
        string inviterCustomerId,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(
                $"{Constants.InviteToOrganizationExistingCustomerPrefix}-{organizationId}-{inviteeCustomerId}-{inviterCustomerId}"),
            s_inviteToJoinOrganizationExistingCustomerInvitationStatusChangedAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowInviteToJoinOrganizationNewCustomerInvitationStatusChanged(
        string organizationId,
        string inviterCustomerId,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(
                $"{Constants.InviteToOrganizationNewCustomerPrefix}-{organizationId}-{inviterCustomerId}-{inviteeCustomerEmail}"),
            s_inviteToJoinOrganizationNewCustomerInvitationStatusChangedAsync,
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
                var input = JsonSerializer.Deserialize<InviteToJoinOrganizationExistingCustomerInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (InviteToJoinOrganizationExistingCustomer workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_inviteToJoinOrganizationNewCustomer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<InviteToJoinOrganizationNewCustomerInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (InviteToJoinOrganizationNewCustomer workflow) => workflow.ExecuteAsync(input),
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
        else if (signalType == s_inviteToJoinOrganizationExistingCustomerInvitationStatusChangedAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<InviteToJoinOrganizationExistingCustomer>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<InviteToJoinOrganizationExistingCustomer>(workflowId)
                .SignalAsync(workflow => workflow.InvitationStatusChangedAsync(), workflowSignalOptions);
        }
        else if (signalType == s_inviteToJoinOrganizationNewCustomerInvitationStatusChangedAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<InviteToJoinOrganizationNewCustomer>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<InviteToJoinOrganizationNewCustomer>(workflowId)
                .SignalAsync(workflow => workflow.InvitationStatusNewCustomerChangedAsync(), workflowSignalOptions);
        }
    }
}
