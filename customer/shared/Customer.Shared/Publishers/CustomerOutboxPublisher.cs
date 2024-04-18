using Api.Shared.Clients.Events.UnityHub.Customer.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Customer.V1.Value;
using Customer.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Type;

namespace Customer.Shared.Publishers;

public interface ICustomerOutboxPublisher
{
    Task PublishCustomerAsync(
        IEnumerable<Models.Customer> customers,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class CustomerOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : ICustomerOutboxPublisher
{
    public async Task PublishCustomerAsync(
        IEnumerable<Models.Customer> customers,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(customers.Select(customer =>
            publisher.PublishAsync(
                new Key { CustomerId = customer.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        customer.IsNotDeleted() ? Type.CustomerUpserted : Type.CustomerDeleted,
                        context.PropertyBag.CorrelationId),
                    Data = new Data { AfterState = mapper.MapTo(customer) }
                }, unitOfWork, cancellationToken)));
}
