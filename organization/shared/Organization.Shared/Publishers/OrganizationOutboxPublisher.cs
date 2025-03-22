using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Organization.Shared.Mappers;
using Organization.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationOutboxPublisher
{
    Task PublishOrganizationAsync(IEnumerable<Models.Organization> organizations, IUnitOfWork unitOfWork, CancellationToken cancellationToken);

    Task PublishInvitesToJoinOrganizationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class OrganizationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IOrganizationOutboxPublisher
{
    public async Task PublishOrganizationAsync(
        IEnumerable<Models.Organization> organizations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        foreach (var organization in organizations)
        {
            await publisher.PublishAsync(
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
                unitOfWork,
                cancellationToken);
        }
    }

    public async Task PublishInvitesToJoinOrganizationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        foreach (var joinInvitation in joinInvitations)
        {
            await publisher.PublishAsync(
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
                unitOfWork,
                cancellationToken);
        }
    }
}
