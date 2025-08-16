using Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationInternalPublisher
{
    Task PublishRecordOrganizationDailyMemberCountAsync(IEnumerable<string> organizationIds, CancellationToken cancellationToken);
    Task PublishStripeConnectAccountWebhookEventReceivedAsync(string id, string payload, CancellationToken cancellationToken);
}

public class OrganizationInternalPublisher(ApplicationConfiguration applicationConfiguration, IContext context, IKafkaPublisher<Key, Event> publisher)
    : IOrganizationInternalPublisher
{
    public async Task PublishRecordOrganizationDailyMemberCountAsync(IEnumerable<string> organizationIds, CancellationToken cancellationToken) =>
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

    public async Task PublishStripeConnectAccountWebhookEventReceivedAsync(string id, string payload, CancellationToken cancellationToken)
    {
        var key = new Key { StripeConnectAccountWebhookKey = id };
        var @event = new Event
        {
            Metadata = Event.NewMetadata(
                applicationConfiguration.DomainSource,
                applicationConfiguration.AppSource,
                Type.StripeConnectAccountWebhookEventReceived,
                context.GetCorrelationId()),
            StripeConnectAccountWebhookEventPayload = payload
        };

        await publisher.PublishAsync(key, @event, cancellationToken);
    }
}
