using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Api.Shared.Clients.Events.Skedular.Booking.V1.Value;
using Enterprise.Shared.Kafka;

namespace Infrastructure.Shared.Services;

public interface IMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken);
}

public class MigrationService(IKafkaHelper kafkaHelper) : IMigrationService
{
    public async Task MigrateAsync(CancellationToken cancellationToken) => await CreateTopicsAsync();

    private async Task CreateTopicsAsync()
    {
        await kafkaHelper.CreateTopicForEventAsync<Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Location.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Organization.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Team.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event>();
    }
}
