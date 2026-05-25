using Api.Shared.Clients.Events.Skedular.Booking.V1;
using Enterprise.Shared.Kafka;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Event;
using Location_V1_Value_Event = Api.Shared.Clients.Events.Skedular.Location.V1.Event;
using Marketplace_V1_Value_Event = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Event;
using Organization_V1_Value_Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Event;
using OrganizationInternal_V1_Value_Event = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Event;
using OrganizationMember_V1_Value_Event = Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Event;
using Team_V1_Value_Event = Api.Shared.Clients.Events.Skedular.Team.V1.Event;
using V1_Value_Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Event;
using Value_Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Event;
using CustomerReadiness_V1_Value_Event = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Event;

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

        await kafkaHelper.CreateTopicForEventAsync<Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Customer.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<V1_Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<CustomerReadiness_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<CustomerReadiness_V1_Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<Location_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Location.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Location_V1_Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<Marketplace_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Marketplace_V1_Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<Organization_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Organization.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Organization_V1_Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<OrganizationMember_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<OrganizationMember_V1_Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<OrganizationInternal_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<OrganizationInternal_V1_Value_Event>();

        await kafkaHelper.CreateTopicForEventAsync<Team_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Team.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Team_V1_Value_Event>();
    }
}
