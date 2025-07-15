using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Mappers;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationOutboxPublisher
{
    void PublishOrganizations(IEnumerable<Models.Organization> organizations, IUnitOfWork unitOfWork);
    void StartWorkflowScheduleRenewOrganizationOffering(ScheduleRenewOrganizationOfferingInput args, IUnitOfWork unitOfWork);
    void SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(string offeringId, IUnitOfWork unitOfWork);
}

public class OrganizationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher,
    TemporalConfiguration temporalConfiguration,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<ScheduleRenewOrganizationOffering> temporalOutboxRenewOrganizationOfferingExecutor)
    : IOrganizationOutboxPublisher
{
    public void PublishOrganizations(IEnumerable<Models.Organization> organizations, IUnitOfWork unitOfWork)
    {
        foreach (var organization in organizations)
        {
            publisher.Publish(
                new Key { OrganizationId = organization.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        organization.IsDeleted() ? Type.OrganizationDeleted : Type.OrganizationUpserted,
                        context.GetCorrelationId()),
                    Data = new Data { Organization = mapper.MapTo(organization) }
                },
                unitOfWork);
        }
    }

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
