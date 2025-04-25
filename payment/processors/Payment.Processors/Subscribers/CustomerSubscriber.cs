using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Payment.Processors.Mappers;
using Payment.Shared.Database.Entities;
using Payment.Shared.Repositories;
using Stripe;
using Customer = Payment.Shared.Models.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class CustomerSubscriber(
    ILogger<CustomerSubscriber> logger,
    IMapper mapper,
    IRandomHelper randomHelper,
    IRepositoryFactory repositoryFactory,
    ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService,
    IUpdatable<Stripe.Customer, CustomerUpdateOptions> customerUpdateService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.CustomerUpserted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
                    if (existingCustomer is not null && existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleCustomerUpsertedEventAsync(@event, customer, existingCustomer, cancellationToken);
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
        Event @event,
        Customer customer,
        Shared.Database.Entities.Customer? existingCustomer,
        CancellationToken cancellationToken)
    {
        if (existingCustomer is null)
        {
            existingCustomer = new Shared.Database.Entities.Customer { Id = customer.Id };

            var stripeCustomer = await customerCreateService.CreateAsync(
                mapper.MapTo(customer),
                new RequestOptions { IdempotencyKey = customer.Id },
                cancellationToken);

            var stripeCustomerEntity = repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
            {
                Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id
            });

            existingCustomer.StripeCustomer = stripeCustomerEntity;
            existingCustomer = RebuildIdentities(customer, existingCustomer);
            _ = repositoryFactory.CustomerRepository.Add(mapper.MergeToEntity(customer, existingCustomer, existingCustomer.Identities));
        }
        else
        {
            if (existingCustomer.StripeCustomer is null)
            {
                var stripeCustomer = await customerCreateService.CreateAsync(
                    mapper.MapTo(customer),
                    new RequestOptions { IdempotencyKey = customer.Id },
                    cancellationToken);
                existingCustomer.StripeCustomer = repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
                {
                    Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id
                });
            }
            else
            {
                var stripeCustomer = await customerUpdateService.UpdateAsync(
                    existingCustomer.StripeCustomer.StripeCustomerId,
                    mapper.MergeTo(customer),
                    new RequestOptions { IdempotencyKey = @event.Metadata.Id },
                    cancellationToken);

                existingCustomer.StripeCustomer.StripeCustomerId = stripeCustomer.Id;
                existingCustomer.StripeCustomer = repositoryFactory.StripeCustomerRepository.Update(existingCustomer.StripeCustomer);
            }

            existingCustomer = RebuildIdentities(customer, existingCustomer);
            _ = repositoryFactory.CustomerRepository.Update(mapper.MergeToEntity(customer, existingCustomer, existingCustomer.Identities));
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleCustomerDeletedEventAsync(Shared.Database.Entities.Customer existingCustomer, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.CustomerRepository.Remove(existingCustomer);
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
