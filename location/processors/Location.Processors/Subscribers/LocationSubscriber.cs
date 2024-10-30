using Api.Shared.Clients.Events.UnityHub.Location.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Location.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Location.Processors.Mappers;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Type = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Type;

namespace Location.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        if (@event.Metadata.DomainSource == applicationConfiguration.DomainSource)
        {
            // Event raised previously by this domain, ignoring it.
            return EventSubscriberResults.Success;
        }

        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation =
                        await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.ModifiedAt > location.ModifiedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationUpsertedEventAsync(location, existingLocation, cancellationToken);
                }
                break;

            case Type.LocationDeleted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation =
                        await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.ModifiedAt > location.ModifiedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingLocation is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationDeletedEventAsync(existingLocation, cancellationToken);
                }
                break;

            case Type.InvitationToJoinLocationUpserted:
            case Type.InvitationToJoinLocationDeleted:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Shared.Database.Entities.Location? existingLocation,
        CancellationToken cancellationToken)
    {
        var organization = location.Organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization.Id,
                cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        existingLocation = existingLocation is null
            ? repositoryFactory.LocationRepository.Add(mapper.MapToEntity(location, organization))
            : repositoryFactory.LocationRepository.Update(
                mapper.MergeToEntity(location, existingLocation, organization));

        existingLocation = RebuildTags(location, existingLocation);
        existingLocation = RebuildDesks(location, existingLocation);
        _ = await RebuildLocationMembersAsync(location, existingLocation, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(
        Shared.Database.Entities.Location existingLocation,
        CancellationToken cancellationToken)
    {
        repositoryFactory.LocationMemberRepository.RemoveRange(existingLocation.LocationMembers);
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Shared.Database.Entities.Location> RebuildLocationMembersAsync(
        Shared.Models.Location location,
        Shared.Database.Entities.Location existingLocation,
        CancellationToken cancellationToken)
    {
        var itemsToRemove = existingLocation.LocationMembers
            .Where(locationMember => location.LocationMembers.All(item => item.Id != locationMember.Id))
            .ToList();
        var updatedItems = new List<LocationMember>();
        foreach (var locationMember in existingLocation.LocationMembers
                     .Where(locationMember =>
                         location.LocationMembers.Any(item => item.Id == locationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id,
                    cancellationToken);
            updatedItems.Add(repositoryFactory.LocationMemberRepository.Update(
                mapper.MergeToEntity(
                    location.LocationMembers.Single(item => item.Id == locationMember.Id),
                    locationMember,
                    existingLocation,
                    customer)));
        }

        var addedItems = new List<LocationMember>();
        foreach (var locationMember in location.LocationMembers.Where(locationMember =>
                     existingLocation.LocationMembers.All(item => item.Id != locationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id,
                    cancellationToken);
            addedItems.Add(repositoryFactory.LocationMemberRepository.Add(
                mapper.MapToEntity(locationMember, existingLocation, customer)));
        }

        repositoryFactory.LocationMemberRepository.RemoveRange(itemsToRemove);
        existingLocation.LocationMembers = addedItems.Concat(updatedItems).ToList();

        return existingLocation;
    }

    private Shared.Database.Entities.Location RebuildTags(
        Shared.Models.Location location,
        Shared.Database.Entities.Location existingLocation)
    {
        var itemsToRemove = existingLocation.Tags
            .Where(tag => location.Tags.All(item => item.Id != tag.Id)).ToList();
        var updatedItems = existingLocation.Tags
            .Where(locationTag => location.Tags.Any(item => item.Id == locationTag.Id))
            .Select(locationTag => repositoryFactory.TagRepository.Update(mapper.MergeToEntity(
                location.Tags.Single(item => item.Id == locationTag.Id),
                locationTag, existingLocation)))
            .ToList();
        var addedItems = location.Tags
            .Where(locationTag => existingLocation.Tags.All(item => item.Id != locationTag.Id))
            .Select(locationTag =>
                repositoryFactory.TagRepository.Add(mapper.MapToEntity(locationTag, existingLocation)))
            .ToList();

        repositoryFactory.TagRepository.RemoveRange(itemsToRemove);
        existingLocation.Tags = addedItems.Concat(updatedItems).ToList();

        return existingLocation;
    }

    private Shared.Database.Entities.Location RebuildDesks(Shared.Models.Location location,
        Shared.Database.Entities.Location existingLocation)
    {
        var itemsToRemove = existingLocation.Desks
            .Where(desk => location.Desks.All(item => item.Id != desk.Id)).ToList();
        var updatedItems = existingLocation.Desks
            .Where(desk => location.Desks.Any(item => item.Id == desk.Id))
            .Select(desk =>
            {
                var locationTags = existingLocation.Tags.Where(existingLocationTag =>
                        location.Desks.Single(item => item.Id == desk.Id).Tags
                            .Any(locationTag => locationTag.Id == existingLocationTag.Id))
                    .ToList();
                return repositoryFactory.DeskRepository.Update(mapper.MergeToEntity(
                    location.Desks.Single(item => item.Id == desk.Id), desk, existingLocation, locationTags));
            })
            .ToList();
        var addedItems = location.Desks
            .Where(desk => existingLocation.Desks.All(item => item.Id != desk.Id))
            .Select(desk =>
            {
                var locationTags = existingLocation.Tags.Where(existingLocationTag =>
                        desk.Tags.Any(locationTag => locationTag.Id == existingLocationTag.Id))
                    .ToList();
                return repositoryFactory.DeskRepository.Add(mapper.MapToEntity(desk, existingLocation,
                    locationTags));
            })
            .ToList();

        repositoryFactory.DeskRepository.RemoveRange(itemsToRemove);
        existingLocation.Desks = addedItems.Concat(updatedItems).ToList();

        return existingLocation;
    }
}
