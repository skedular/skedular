using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows;
using Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationExistingCustomer;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationNewCustomer;
using Organization.Shared.Workflows.NewOrganizationJoined;
using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.Publishers;

public interface ITemporalOutboxPublisher
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
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor)
    : ITemporalOutboxPublisher
{
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
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate
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
                    $"{Constants.InviteToOrganizationNewCustomerPrefix}-{args.OrganizationId}-{args.InviteeCustomerEmail}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
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
            typeof(ScheduleRenewOrganizationOffering).GetMethod(nameof(ScheduleRenewOrganizationOffering.CancelOfferingAsync))!
                .ToWorkflowSignalType(),
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
            typeof(InviteToJoinOrganizationExistingCustomer).GetMethod(nameof(InviteToJoinOrganizationExistingCustomer.InvitationStatusChangedAsync))!
                .ToWorkflowSignalType(),
            new WorkflowSignalOptions(),
            unitOfWork);
}
