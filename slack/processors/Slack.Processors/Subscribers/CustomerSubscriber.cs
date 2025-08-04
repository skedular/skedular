using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Enterprise.Shared.Kafka.Consume;
using Slack.Processors.Mappers;
using Slack.Shared.Repositories;
using Customer = Slack.Shared.Models.Customer;
using Type = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Type;

namespace Slack.Processors.Subscribers;

public class CustomerSubscriber(ILogger<CustomerSubscriber> logger, IMapper mapper, IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.CustomerUpserted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customer.Id, cancellationToken);
                    if (existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleCustomerUpsertedEventAsync(customer, existingCustomer, cancellationToken);
                }
                break;

            case Type.CustomerDeleted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
                    if (existingCustomer is not null && existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingCustomer is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleCustomerDeletedEventAsync(existingCustomer, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleCustomerUpsertedEventAsync(
        Customer customer,
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        _ = RebuildIdentities(customer, existingCustomer);
        await repositoryFactory.CustomerRepository.UpdateAsync(
            mapper.MergeToEntity(customer, existingCustomer, existingCustomer.Identities),
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleCustomerDeletedEventAsync(Shared.Database.Entities.Customer existingCustomer, CancellationToken cancellationToken)
    {
        _ = await repositoryFactory.CustomerRepository.RemoveAsync(existingCustomer, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private Shared.Database.Entities.Customer RebuildIdentities(Customer customer, Shared.Database.Entities.Customer existingCustomer)
    {
        var itemsToRemove = existingCustomer.Identities.Where(identity => customer.Identities.All(item => item.Id != identity.Id)).ToList();
        var updatedItems = existingCustomer.Identities
            .Where(identity => customer.Identities.Any(item => item.Id == identity.Id))
            .Select(identity => repositoryFactory.IdentityRepository.Update(mapper.MergeToEntity(
                customer.Identities.First(item => item.Id == identity.Id),
                identity,
                existingCustomer)))
            .ToList();
        var addedItems = customer.Identities
            .Where(identity => existingCustomer.Identities.All(item => item.Id != identity.Id))
            .Select(identity => repositoryFactory.IdentityRepository.Add(mapper.MapToEntity(identity, existingCustomer)))
            .ToList();

        repositoryFactory.IdentityRepository.RemoveRange(itemsToRemove);
        existingCustomer.Identities = addedItems.Concat(updatedItems).ToList();

        return existingCustomer;
    }
}
