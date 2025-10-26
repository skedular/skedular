using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event;
using IMapper = Marketplace.Processors.Mappers.IMapper;
using Location = Marketplace.Shared.Database.Entities.Location;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Marketplace.Processors.Subscribers;

public class LocationSubscriber(ILogger<LocationSubscriber> logger, IMapper mapper, IRepositoryFactory repositoryFactory)
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
                    var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
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
        var organizationTags = new List<OrganizationTag>();
        var organizationTagIds = location.OrganizationTags.Select(item => item.Id).Distinct().ToList();
        foreach (var tagId in organizationTagIds)
        {
            organizationTags.Add(await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(tagId, organization, cancellationToken));
        }

        LocationPhysicalAddress? physicalAddress = null;
        if (existingLocation.PhysicalAddress is null)
        {
            if (location.PhysicalAddress is not null)
            {
                physicalAddress = mapper.MapToEntity(location.PhysicalAddress, existingLocation);
                repositoryFactory.LocationPhysicalAddressRepository.Add(physicalAddress);
            }
        }
        else
        {
            if (location.PhysicalAddress is null)
            {
                repositoryFactory.LocationPhysicalAddressRepository.Remove(existingLocation.PhysicalAddress);
            }
            else
            {
                if (existingLocation.PhysicalAddress.Id != location.PhysicalAddress.Id)
                {
                    repositoryFactory.LocationPhysicalAddressRepository.Remove(existingLocation.PhysicalAddress);
                    physicalAddress =
                        repositoryFactory.LocationPhysicalAddressRepository.Add(mapper.MapToEntity(location.PhysicalAddress, existingLocation));
                }
                else
                {
                    physicalAddress = repositoryFactory.LocationPhysicalAddressRepository.Update(
                        mapper.MergeToEntity(
                            location.PhysicalAddress,
                            existingLocation.PhysicalAddress,
                            existingLocation));
                }
            }
        }

        _ = repositoryFactory.LocationRepository.Update(
            mapper.MergeToEntity(
                location,
                existingLocation,
                organization,
                organizationTags,
                physicalAddress));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
