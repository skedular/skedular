using Api.Shared.Clients.Events.Skedular.Billing.V1.Key;
using Api.Shared.Clients.Events.Skedular.Billing.V1.Value;
using Billing.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.Skedular.Billing.V1.Value.Event;
using Organization = Billing.Shared.Models.Organization;
using OrganizationOffering = Billing.Shared.Models.OrganizationOffering;
using Type = Api.Shared.Clients.Events.Skedular.Billing.V1.Value.Type;

namespace Billing.Shared.Publishers;

public interface IBillingOutboxPublisher
{
    Task PublishBillingOrganizationsOfferingsAsync(
        IEnumerable<OrganizationOffering> organizationOfferings,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

    Task PublishOrganizationsBillingInfoAsync(
        IEnumerable<Organization> organizations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class BillingOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IBillingOutboxPublisher
{
    public async Task PublishBillingOrganizationsOfferingsAsync(
        IEnumerable<OrganizationOffering> organizationOfferings,
        IUnitOfWork unitOfWork,
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
                    context.GetCorrelationId()),
                Data = new Data { OrganizationOfferingBillingAfterState = mapper.MapTo(organizationOffering) }
            };

            await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
        }));

    public async Task PublishOrganizationsBillingInfoAsync(
        IEnumerable<Organization> organizations,
        IUnitOfWork unitOfWork,
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
                    context.GetCorrelationId()),
                Data = new Data { OrganizationBillingInfoAfterState = mapper.MapTo(organization) }
            };

            await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
        }));
}
