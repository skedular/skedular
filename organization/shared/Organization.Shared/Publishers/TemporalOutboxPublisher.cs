using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork);
    void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<ScheduleRenewOrganizationOffering> temporalOutboxRenewOrganizationOfferingExecutor)
    : ITemporalOutboxPublisher
{
    public void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxRenewOrganizationOfferingExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = args.OrganizationOfferingId,
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);

    public void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            offeringId,
            typeof(ScheduleRenewOrganizationOffering).GetMethod(nameof(ScheduleRenewOrganizationOffering.CancelOfferingAsync))!
                .ToWorkflowSignalType(),
            new WorkflowSignalOptions(),
            unitOfWork);
}
