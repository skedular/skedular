using Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Produce;
using CRType = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Type;

namespace Marketplace.Shared.Publishers;

public interface ICustomerReadinessPublisher
{
    Task PublishProvisionedAsync(string customerId, string? correlationId, CancellationToken cancellationToken);
}

public class CustomerReadinessPublisher(ApplicationConfiguration applicationConfiguration, IKafkaPublisher<Key, Event> publisher)
    : ICustomerReadinessPublisher
{
    public Task PublishProvisionedAsync(string customerId, string? correlationId, CancellationToken cancellationToken) =>
        publisher.PublishAsync(
            new Key
            {
                CustomerId = customerId,
            },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    CRType.CustomerIdentityProvisioned,
                    correlationId),
                Data = new Data
                {
                    CustomerIdentityProvisioned = new CustomerIdentityProvisioned
                    {
                        CustomerId = customerId,
                        Domain = Domain.Marketplace,
                    },
                },
            },
            cancellationToken);
}
