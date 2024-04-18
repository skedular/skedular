using Api.Shared.Clients.Events.UnityHub.Organization.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Organization.Shared.Mappers;
using Organization.Shared.Models;
using Event = Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationOutboxPublisher
{
    Task PublishOrganizationAsync(
        IEnumerable<Models.Organization> organizations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

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
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizations.Select(organization =>
            publisher.PublishAsync(
                new Key { OrganizationId = organization.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        organization.IsNotDeleted() ? Type.OrganizationUpserted : Type.OrganizationDeleted,
                        context.PropertyBag.CorrelationId),
                    Data = new Data { OrganizationAfterState = mapper.MapTo(organization) }
                }, unitOfWork, cancellationToken)));

    public async Task PublishInvitesToJoinOrganizationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(joinInvitations.Select(
            joinInvitation => publisher.PublishAsync(
                new Key { OrganizationId = joinInvitation.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        joinInvitation.IsNotDeleted() ? Type.NotificationUpserted : Type.NotificationDeleted,
                        context.PropertyBag.CorrelationId),
                    Data = new Data { NotificationAfterState = mapper.MapTo(joinInvitation, null) }
                },
                unitOfWork,
                cancellationToken)));
}
