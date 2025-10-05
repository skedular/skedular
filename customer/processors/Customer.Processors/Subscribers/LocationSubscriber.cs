using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Api.Shared.Services;
using Customer.Processors.Mappers;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Kafka.Consume;
using Location = Customer.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Customer.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(@event.Data.Location.OrganizationId);

                    var location = mapper.MapTo(@event);
                    var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization!.Id, cancellationToken);
                    var existingLocation = await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, organization, cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationUpsertedEventAsync(location, existingLocation, organization, cancellationToken);
                }
                break;

            case Type.LocationDeleted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, true, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingLocation is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationDeletedEventAsync(existingLocation, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location existingLocation,
        Organization organization,
        CancellationToken cancellationToken)
    {
        existingLocation = repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation, organization));

        _ = await RebuildResourcesAsync(location, existingLocation, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        await UpdateCustomerPreferredLocationsAsync(existingLocation, cancellationToken);
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Location> RebuildResourcesAsync(Shared.Models.Location location, Location existingLocation,
        CancellationToken cancellationToken)
    {
        var resources = await repositoryFactory.ResourceRepository.GetByLocationIdAsync(existingLocation.Id, cancellationToken);
        var itemsToRemove = resources.Where(resource => location.Resources.All(item => item.Id != resource.Id)).ToList();
        var updatedItems = resources
            .Where(resource => location.Resources.Any(item => item.Id == resource.Id))
            .Select(resource =>
            {
                var updatedResource = mapper.MergeToEntity(
                    location.Resources.First(item => item.Id == resource.Id),
                    resource,
                    existingLocation);
                updatedResource.DeletedAt = null;
                return repositoryFactory.ResourceRepository.Update(updatedResource);
            })
            .ToList();
        var addedItems = location.Resources
            .Where(resource => resources.All(item => item.Id != resource.Id))
            .Select(resource => repositoryFactory.ResourceRepository.Add(mapper.MapToEntity(resource, existingLocation)))
            .ToList();

        repositoryFactory.ResourceRepository.RemoveRange(itemsToRemove);
        existingLocation.Resources = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingLocation;
    }

    private async Task UpdateCustomerPreferredLocationsAsync(Location location, CancellationToken cancellationToken)
    {
        var customerIds = location.PreferredByCustomers.Select(customer => customer.Id).ToList();
        foreach (var customerId in customerIds)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken) ?? throw new CustomerNotFound();
            customer.PreferredLocations = customer.PreferredLocations.Where(item => item.Id != location.Id).ToList();
            customer.PreferredResources =
                customer.PreferredResources.Where(item => item.Location is not null && item.Location.Id != location.Id).ToList();
            _ = repositoryFactory.CustomerRepository.Update(customer);

            await cachedCustomerService.RemoveAsync([customer], cancellationToken);
        }
    }
}
