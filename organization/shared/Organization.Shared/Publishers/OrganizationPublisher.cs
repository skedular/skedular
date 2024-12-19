using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Organization.Shared.Mappers;
using Organization.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationPublisher
{
    Task PublishOrganizationAsync(IEnumerable<Models.Organization> organizations, CancellationToken cancellationToken);

    Task PublishInvitesToJoinOrganizationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        string? inviteeIdToOverride,
        CancellationToken cancellationToken);
}

public class OrganizationPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IOrganizationPublisher
{
    public async Task PublishOrganizationAsync(IEnumerable<Models.Organization> organizations,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizations.Select(
            organization => publisher.PublishAsync(
                new Key { OrganizationId = organization.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        organization.IsNotDeleted() ? Type.OrganizationUpserted : Type.OrganizationDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { OrganizationAfterState = mapper.MapTo(organization) }
                },
                cancellationToken)));

    public async Task PublishInvitesToJoinOrganizationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        string? inviteeIdToOverride,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(joinInvitations.Select(
            joinInvitation => publisher.PublishAsync(
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
                    Data = new Data
                    {
                        InvitationToJoinOrganizationAfterState = mapper.MapTo(joinInvitation, inviteeIdToOverride)
                    }
                },
                cancellationToken)));
}
