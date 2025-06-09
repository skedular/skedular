using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationOutboxPublisher
{
    void PublishOrganizations(IEnumerable<Models.Organization> organizations, IUnitOfWork unitOfWork);
    void PublishInvitesToJoinOrganizationNotification(IEnumerable<JoinInvitation> joinInvitations, IUnitOfWork unitOfWork);
    void ExecuteWorkflowAddOrganizationStripePaymentMethod(AddOrganizationStripePaymentMethodInput args, IUnitOfWork unitOfWork);
}

public class OrganizationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher,
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<AddOrganizationStripePaymentMethod, AddOrganizationStripePaymentMethodInput>
        temporalOutboxAddOrganizationStripePaymentMethodWorkflowExecutor) : IOrganizationOutboxPublisher
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
                        organization.IsNotDeleted() ? Type.OrganizationUpserted : Type.OrganizationDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { Organization = mapper.MapTo(organization) }
                },
                unitOfWork);
        }
    }

    public void PublishInvitesToJoinOrganizationNotification(IEnumerable<JoinInvitation> joinInvitations, IUnitOfWork unitOfWork)
    {
        foreach (var joinInvitation in joinInvitations)
        {
            publisher.Publish(
                new Key { OrganizationId = joinInvitation.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        joinInvitation.IsNotDeleted()
                            ? Type.InvitationToJoinOrganizationUpserted
                            : Type.InvitationToJoinOrganizationDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { InvitationToJoinOrganization = mapper.MapTo(joinInvitation, null) }
                },
                unitOfWork);
        }
    }

    public void ExecuteWorkflowAddOrganizationStripePaymentMethod(AddOrganizationStripePaymentMethodInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxAddOrganizationStripePaymentMethodWorkflowExecutor.Execute(
            new AddOrganizationStripePaymentMethodInput(args.OrganizationId, args.ClientSecret, args.SetupIntentId),
            new WorkflowOptions
            {
                Id = args.ClientSecret,
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
