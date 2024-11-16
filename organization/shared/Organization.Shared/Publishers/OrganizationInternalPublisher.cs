using Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationInternalPublisher
{
    Task PublishOrganizationsRequireOfferingAutoRenewAsync(
        IEnumerable<string> organizationIds,
        CancellationToken cancellationToken);

    Task PublishRecordOrganizationDailyMemberCountAsync(
        IEnumerable<string> organizationIds,
        CancellationToken cancellationToken);

    Task PublishRefreshAzureTenantMembersAsync(IEnumerable<string> azureTenantIds, CancellationToken cancellationToken);
}

public class OrganizationInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IOrganizationInternalPublisher
{
    public async Task PublishOrganizationsRequireOfferingAutoRenewAsync(
        IEnumerable<string> organizationIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizationIds.Select(async organizationId =>
        {
            var key = new Key { OrganizationId = organizationId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RenewOrganizationOffering,
                    context.GetCorrelationId()),
                OrganizationId = organizationId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishRecordOrganizationDailyMemberCountAsync(
        IEnumerable<string> organizationIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizationIds.Select(async organizationId =>
        {
            var key = new Key { OrganizationId = organizationId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RecordDailyMemberCount,
                    context.GetCorrelationId()),
                OrganizationId = organizationId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishRefreshAzureTenantMembersAsync(
        IEnumerable<string> azureTenantIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(azureTenantIds.Select(async azureTenantId =>
        {
            var key = new Key { AzureTenantId = azureTenantId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshAzureTenantMembers,
                    context.GetCorrelationId()),
                AzureTenantId = azureTenantId
            };
            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
