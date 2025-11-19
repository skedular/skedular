using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Activities;
using Organization.Shared.Workflows;
using Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationExistingCustomer;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationNewCustomer;
using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork);

    void StartWorkflowInviteToJoinOrganizationExistingCustomer(
        SendInviteCustomerToJoinOrganizationExistingCustomerInput args,
        IUnitOfWork unitOfWork);

    void StartWorkflowOrganizationDailyAnalytics(GenerateOrganizationDailyAnalyticsInput args, IUnitOfWork unitOfWork);
    void StartWorkflowReSyncAzureTenant(ReSyncAzureTenantInput args, IUnitOfWork unitOfWork);
    void StartWorkflowInviteToJoinOrganizationNewCustomer(InviteToJoinOrganizationNewCustomerInput args, IUnitOfWork unitOfWork);
    void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<ScheduleRenewOrganizationOffering> temporalOutboxRenewOrganizationOfferingExecutor,
    ITemporalOutboxWorkflowExecutor<InviteToJoinOrganizationExistingCustomer> temporalOutboxInviteToJoinOrganizationExistingCustomerExecutor,
    ITemporalOutboxWorkflowExecutor<InviteToJoinOrganizationNewCustomer> temporalOutboxInviteToJoinOrganizationNewCustomerWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<ReSyncAzureTenant> temporalOutboxReSyncAzureTenantExecutor,
    ITemporalOutboxWorkflowExecutor<GenerateOrganizationDailyAnalytics> temporalOutboxOrganizationDailyAnalyticsExecutor)
    : ITemporalOutboxPublisher
{
    public void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxRenewOrganizationOfferingExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.OrganizationOfferingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void StartWorkflowInviteToJoinOrganizationExistingCustomer(
        SendInviteCustomerToJoinOrganizationExistingCustomerInput args,
        IUnitOfWork unitOfWork) =>
        temporalOutboxInviteToJoinOrganizationExistingCustomerExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(
                    $"{Constants.InviteToOrganizationExistingCustomerPrefix}-{args.OrganizationId}-{args.InviteeCustomerId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowOrganizationDailyAnalytics(GenerateOrganizationDailyAnalyticsInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxOrganizationDailyAnalyticsExecutor.Execute(
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
        temporalOutboxReSyncAzureTenantExecutor.Execute(
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
        temporalOutboxInviteToJoinOrganizationNewCustomerWorkflowExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(
                    $"{Constants.InviteToOrganizationExistingCustomerPrefix}-{args.OrganizationId}-{args.InviteeCustomerEmail}"),
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
}
