using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Customer.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Type;

namespace Customer.Shared.Publishers;

public interface ICustomerPublisher
{
    Task PublishCustomersAsync(IEnumerable<Models.Customer> customers, CancellationToken cancellationToken);
}

public class CustomerPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ICustomerPublisher
{
    public async Task PublishCustomersAsync(IEnumerable<Models.Customer> customers, CancellationToken cancellationToken) =>
        await Task.WhenAll(customers.Select(customer => publisher.PublishAsync(
            new Key { CustomerId = customer.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    customer.IsNotDeleted() ? Type.CustomerUpserted : Type.CustomerDeleted,
                    context.GetCorrelationId()),
                Data = new Data { Customer = mapper.MapTo(customer) }
            },
            cancellationToken)));
}
