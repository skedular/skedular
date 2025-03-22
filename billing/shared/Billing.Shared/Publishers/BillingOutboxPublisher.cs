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

    Task PublishOrganizationsBillingInfoAsync(IEnumerable<Organization> organizations, IUnitOfWork unitOfWork, CancellationToken cancellationToken);
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
        CancellationToken cancellationToken)
    {
        foreach (var organizationOffering in organizationOfferings)
        {
            await publisher.PublishAsync(
                new Key { OrganizationOfferingId = organizationOffering.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.BillingOrganizationOfferingUpserted,
                        context.GetCorrelationId()),
                    Data = new Data { OrganizationOfferingBilling = mapper.MapTo(organizationOffering) }
                },
                unitOfWork,
                cancellationToken);
        }
    }

    public async Task PublishOrganizationsBillingInfoAsync(
        IEnumerable<Organization> organizations,
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
                        Type.OrganizationBillingInfoUpdated,
                        context.GetCorrelationId()),
                    Data = new Data { OrganizationBillingInfo = mapper.MapTo(organization) }
                },
                unitOfWork,
                cancellationToken);
        }
    }
}
