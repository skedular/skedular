using Api.Shared.Clients.Events.Skedular.Customer.V1;
using Booking.Processors.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Kafka.Consume;
using Customer = Booking.Shared.Models.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using OrganizationTag = Booking.Shared.Database.Entities.OrganizationTag;
using Resource = Booking.Shared.Database.Entities.Resource;
using Type = Api.Shared.Clients.Events.Skedular.Customer.V1.Type;

namespace Booking.Processors.Subscribers;

public class CustomerSubscriber(
    ILogger<CustomerSubscriber> logger,
    IEventMapper eventMapper,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICustomerReadinessPublisher customerReadinessPublisher)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.CustomerUpserted:
                {
                    var customer = eventMapper.MapTo(@event);
                    var existingCustomer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customer.Id, false, cancellationToken);
                    if (existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleCustomerUpsertedEventAsync(customer, existingCustomer, cancellationToken);
                    await customerReadinessPublisher.PublishProvisionedAsync(customer.Id, @event.Metadata.CorrelationId, cancellationToken);
                }
                break;

            case Type.CustomerDeleted:
                {
                    var customer = eventMapper.MapTo(@event);
                    var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, false, cancellationToken);
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
        var defaultOrganization = customer.DefaultOrganization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(customer.DefaultOrganization.Id, cancellationToken);

        var preferredLocations = new List<Location>();
        foreach (var item in customer.PreferredLocations)
        {
            preferredLocations.Add(await repositoryFactory.LocationRepository.UpsertNakedAsync(item.Id, null, cancellationToken));
        }

        var preferredResources = new List<Resource>();
        foreach (var item in customer.PreferredResources)
        {
            if (item.Location is not null)
            {
                var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(item.Location.Id, null, cancellationToken);
                preferredResources.Add(await repositoryFactory.ResourceRepository.UpsertNakedAsync(item.Id, location, cancellationToken));
            }
        }

        var preferredOrganizationTags = new List<OrganizationTag>();
        foreach (var item in customer.PreferredOrganizationTags)
        {
            var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(item.Organization.Id, cancellationToken);
            preferredOrganizationTags.Add(
                await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(item.Id, organization, cancellationToken));
        }

        _ = RebuildIdentities(customer, existingCustomer);
        existingCustomer = repositoryFactory.CustomerRepository.Update(
            eventMapper.MergeToEntity(
                customer,
                existingCustomer,
                existingCustomer.Identities,
                defaultOrganization,
                preferredLocations,
                preferredResources,
                preferredOrganizationTags));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await cachedCustomerService.RemoveAsync([existingCustomer], cancellationToken);
    }

    private async Task HandleCustomerDeletedEventAsync(Shared.Database.Entities.Customer existingCustomer, CancellationToken cancellationToken)
    {
        existingCustomer = repositoryFactory.CustomerRepository.Remove(existingCustomer);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await cachedCustomerService.RemoveAsync([existingCustomer], cancellationToken);
    }

    private Shared.Database.Entities.Customer RebuildIdentities(Customer customer, Shared.Database.Entities.Customer existingCustomer)
    {
        var itemsToRemove = existingCustomer.Identities.Where(identity => customer.Identities.All(item => item.Id != identity.Id)).ToList();
        var updatedItems = existingCustomer.Identities
            .Where(identity => customer.Identities.Any(item => item.Id == identity.Id))
            .Select(identity =>
                repositoryFactory.IdentityRepository.Update(
                    eventMapper.MergeToEntity(customer.Identities.First(item => item.Id == identity.Id), identity, existingCustomer)))
            .ToList();
        var addedItems = customer.Identities
            .Where(identity => existingCustomer.Identities.All(item => item.Id != identity.Id))
            .Select(identity => repositoryFactory.IdentityRepository.Add(eventMapper.MapToEntity(identity, existingCustomer)))
            .ToList();

        repositoryFactory.IdentityRepository.RemoveRange(itemsToRemove);
        existingCustomer.Identities = addedItems.Concat(updatedItems).ToList();

        return existingCustomer;
    }
}
