using Api.Shared.Clients.Events.Skedular.Customer.V1;
using Customer.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Customer.V1.Type;

namespace Customer.Shared.Publishers;

public interface ICustomerPublisher
{
    Task PublishCustomersAsync(IReadOnlyList<Models.Customer> customers, CancellationToken cancellationToken);
}

public class CustomerPublisher(
    ApplicationConfiguration applicationConfiguration,
    IEventMapper eventMapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ICustomerPublisher
{
    public async Task PublishCustomersAsync(IReadOnlyList<Models.Customer> customers, CancellationToken cancellationToken) =>
        await Task.WhenAll(customers.Select(customer => publisher.PublishAsync(
            new Key
            {
                CustomerId = customer.Id,
            },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    customer.IsDeleted() ? Type.CustomerDeleted : Type.CustomerUpserted,
                    context.GetCorrelationId()),
                Data = new Data
                {
                    Customer = eventMapper.MapTo(customer),
                },
            },
            cancellationToken)));
}
