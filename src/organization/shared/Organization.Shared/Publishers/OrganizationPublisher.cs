using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Organization.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationPublisher
{
    Task PublishOrganizationsAsync(IReadOnlyList<Models.Organization> organizations, CancellationToken cancellationToken);
}

public class OrganizationPublisher(
    ApplicationConfiguration applicationConfiguration,
    IEventMapper eventMapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IOrganizationPublisher
{
    public async Task PublishOrganizationsAsync(IReadOnlyList<Models.Organization> organizations,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizations.Select(organization => publisher.PublishAsync(
            new Key
            {
                OrganizationId = organization.Id,
            },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    organization.IsDeleted() ? Type.OrganizationDeleted : Type.OrganizationUpserted,
                    context.GetCorrelationId()),
                Data = new Data
                {
                    Organization = eventMapper.MapTo(organization),
                },
            },
            cancellationToken)));
}
