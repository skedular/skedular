using Api.Shared.Clients.Events.Skedular.Billing.V1.Key;
using Api.Shared.Clients.Events.Skedular.Billing.V1.Value;
using Billing.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.Skedular.Billing.V1.Value.Event;
using Customer = Billing.Shared.Models.Customer;
using OrganizationOffering = Billing.Shared.Models.OrganizationOffering;
using Type = Api.Shared.Clients.Events.Skedular.Billing.V1.Value.Type;

namespace Billing.Shared.Publishers;

public interface IBillingOutboxPublisher
{
    void PublishBillingOrganizationsOfferings(IEnumerable<OrganizationOffering> organizationOfferings, IUnitOfWork unitOfWork);
    void PublishCustomersBillingInfo(IEnumerable<Customer> customers, IUnitOfWork unitOfWork);
}

public class BillingOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher) : IBillingOutboxPublisher
{
    public void PublishBillingOrganizationsOfferings(IEnumerable<OrganizationOffering> organizationOfferings, IUnitOfWork unitOfWork)
    {
        foreach (var organizationOffering in organizationOfferings)
        {
            publisher.Publish(
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
                unitOfWork);
        }
    }

    public void PublishCustomersBillingInfo(IEnumerable<Customer> customers, IUnitOfWork unitOfWork)
    {
        foreach (var customer in customers)
        {
            publisher.Publish(
                new Key { CustomerId = customer.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.CustomerBillingInfoUpdated,
                        context.GetCorrelationId()),
                    Data = new Data { CustomerBillingContact = mapper.MapTo(customer) }
                },
                unitOfWork);
        }
    }
}
