using Api.Shared.Clients.Events.UnityHub.Billing.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Billing.V1.Value;
using Billing.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.UnityHub.Billing.V1.Value.Event;
using Organization = Billing.Shared.Models.Organization;
using OrganizationOffering = Billing.Shared.Models.OrganizationOffering;
using Type = Api.Shared.Clients.Events.UnityHub.Billing.V1.Value.Type;

namespace Billing.Shared.Publishers;

public interface IBillingPublisher
{
    Task PublishBillingOrganizationsOfferingsAsync(
        IEnumerable<OrganizationOffering> organizationOfferings,
        CancellationToken cancellationToken);

    Task PublishOrganizationsBillingInfoAsync(
        IEnumerable<Organization> organizations,
        CancellationToken cancellationToken);
}

public class BillingPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IBillingPublisher
{
    public async Task PublishBillingOrganizationsOfferingsAsync(
        IEnumerable<OrganizationOffering> organizationOfferings,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizationOfferings.Select(async organizationOffering =>
        {
            var key = new Key { OrganizationOfferingId = organizationOffering.Id };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.BillingOrganizationOfferingUpserted,
                    context.PropertyBag.CorrelationId),
                Data = new Data { OrganizationOfferingBillingAfterState = mapper.MapTo(organizationOffering) }
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishOrganizationsBillingInfoAsync(
        IEnumerable<Organization> organizations,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizations.Select(async organization =>
        {
            var key = new Key { OrganizationId = organization.Id };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.OrganizationBillingInfoUpdated,
                    context.PropertyBag.CorrelationId),
                Data = new Data { OrganizationBillingInfoAfterState = mapper.MapTo(organization) }
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
