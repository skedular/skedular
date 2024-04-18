using Api.Shared.Clients.Events.UnityHub.Customer.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Customer.V1.Value;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Payment.Processors.Mappers;
using Payment.Shared.Repositories;
using Customer = Payment.Shared.Models.Customer;
using Type = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class CustomerSubscriber(
    ILogger<CustomerSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.CustomerUpserted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer =
                        await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
                    if (existingCustomer is not null && existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    await HandleCustomerUpsertedEventAsync(customer, existingCustomer, cancellationToken);
                }
                break;

            case Type.CustomerDeleted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer =
                        await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
                    if (existingCustomer is not null && existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    if (existingCustomer is null)
                    {
                        return;
                    }

                    await HandleCustomerDeletedEventAsync(existingCustomer, cancellationToken);
                }
                break;

            default:
                return;
        }
    }

    private async Task HandleCustomerUpsertedEventAsync(
        Customer customer,
        Shared.Database.Entities.Customer? existingCustomer,
        CancellationToken cancellationToken)
    {
        if (existingCustomer is null)
        {
            var identities = mapper.MapToEntity(customer.Identities, null).ToList();
            existingCustomer = mapper.MapToEntity(customer, identities);

            identities.ForEach(identity => identity.Customer = existingCustomer);
            repositoryFactory.IdentityRepository.AddRange(identities);
            existingCustomer.Identities = identities;
            _ = repositoryFactory.CustomerRepository.Add(existingCustomer);
        }
        else
        {
            _ = await RebuildIdentitiesAsync(customer, existingCustomer, cancellationToken);
            repositoryFactory.CustomerRepository.Update(
                mapper.MergeToEntity(customer, existingCustomer, existingCustomer.Identities)
            );
        }

        await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleCustomerDeletedEventAsync(
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.CustomerRepository.Remove(existingCustomer);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Shared.Database.Entities.Customer> RebuildIdentitiesAsync(
        Customer customer,
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        var itemsToRemove = existingCustomer.Identities
            .Where(identity => customer.Identities.All(item => item.Id != identity.Id)).ToList();
        var updatedItems = existingCustomer.Identities
            .Where(identity => customer.Identities.Any(item => item.Id == identity.Id))
            .Select(identity => repositoryFactory.IdentityRepository.Update(mapper.MergeToEntity(
                customer.Identities.Single(item => item.Id == identity.Id),
                identity,
                existingCustomer)))
            .ToList();
        var addedItems = customer.Identities
            .Where(identity => existingCustomer.Identities.All(item => item.Id != identity.Id))
            .Select(identity =>
                repositoryFactory.IdentityRepository.Add(mapper.MapToEntity(identity, existingCustomer)))
            .ToList();

        repositoryFactory.IdentityRepository.RemoveRange(itemsToRemove);
        await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        existingCustomer.Identities = addedItems.Concat(updatedItems).ToList();

        return existingCustomer;
    }
}
