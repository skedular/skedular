using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using IMapper = Booking.Processors.Mappers.IMapper;
using Location = Booking.Shared.Database.Entities.Location;
using LocationMember = Booking.Shared.Database.Entities.LocationMember;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IBookingInternalOutboxPublisher bookingInternalOutboxPublisher) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    var organization = location.Organization is null
                        ? null
                        : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization.Id, cancellationToken);
                    var existingLocation = await repositoryFactory.LocationRepository.UpsertNakedAsync(
                        location.Id,
                        organization,
                        cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
                    await HandleLocationUpsertedEventAsync(location, existingLocation, cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                break;

            case Type.LocationDeleted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation =
                        await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, true, true, true, true, cancellationToken);
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

            case Type.InvitationToJoinLocationUpserted:
            case Type.InvitationToJoinLocationDeleted:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location existingLocation,
        CancellationToken cancellationToken)
    {
        var organization = location.Organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.GetByIdAsync(location.Organization.Id, true, true, cancellationToken);

        var locationOpeningHoursChanged = !location.OpeningHours.IsEqual(existingLocation.OpeningHours);
        existingLocation = repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation, organization));

        (existingLocation, var resourceIds) = await RebuildResourcesAsync(location, existingLocation, organization, cancellationToken);
        existingLocation = await RebuildDesksAsync(location, existingLocation, organization, cancellationToken);
        existingLocation = await RebuildRoomsAsync(location, existingLocation, organization, cancellationToken);
        _ = await RebuildLocationMembersAsync(location, existingLocation, cancellationToken);

        if (locationOpeningHoursChanged)
        {
            // Regenerate all
            await bookingInternalOutboxPublisher.PublishGenerateResourceBookingSlotAsync(
                existingLocation.Resources.Where(item => item.DeletedAt is null).Select(item => item.Id),
                repositoryFactory.UnitOfWork,
                cancellationToken);
        }
        else
        {
            // Regenerate those changed
            await bookingInternalOutboxPublisher.PublishGenerateResourceBookingSlotAsync(
                resourceIds,
                repositoryFactory.UnitOfWork,
                cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Location, ICollection<string>)> RebuildResourcesAsync(
        Shared.Models.Location location,
        Location existingLocation,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        if (organization is null)
        {
            return (existingLocation, []);
        }

        var organizationTags = new List<OrganizationTag>();
        var organizationTagIds = location.Resources.SelectMany(item => item.OrganizationTags.Select(tag => tag.Id)).Distinct().ToList();
        foreach (var tagId in organizationTagIds)
        {
            organizationTags.Add(await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(tagId, organization, cancellationToken));
        }

        var resourceIdsToRegenerateBookingSlots = new List<string>();
        var resources = await repositoryFactory.ResourceRepository.GetByLocationIdAsync(existingLocation.Id, cancellationToken);
        var itemsToRemove = resources.Where(resource => location.Resources.All(item => item.Id != resource.Id)).ToList();
        var updatedItems = resources
            .Where(resource => location.Resources.Any(item => item.Id == resource.Id))
            .Select(existingResource =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag =>
                        location.Resources
                            .First(item => item.Id == existingResource.Id).OrganizationTags
                            .Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();


                var resource = location.Resources.First(item => item.Id == existingResource.Id);

                if (resource.IsAvailableHoursOverridden != existingResource.IsAvailableHoursOverridden)
                {
                    resourceIdsToRegenerateBookingSlots.Add(resource.Id);
                }
                else if (!resource.AvailableHours.IsEqual(existingResource.AvailableHours))
                {
                    resourceIdsToRegenerateBookingSlots.Add(resource.Id);
                }

                var updatedResource = mapper.MergeToEntity(resource, existingResource, existingLocation, filteredOrganizationTags);
                updatedResource.DeletedAt = null;

                return repositoryFactory.ResourceRepository.Update(updatedResource);
            })
            .ToList();
        var addedItems = location.Resources
            .Where(resource => resources.All(item => item.Id != resource.Id))
            .Select(resource =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag => resource.OrganizationTags.Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();

                return repositoryFactory.ResourceRepository.Add(mapper.MapToEntity(resource, existingLocation, filteredOrganizationTags));
            })
            .ToList();

        repositoryFactory.ResourceRepository.RemoveRange(itemsToRemove);
        existingLocation.Resources = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        resourceIdsToRegenerateBookingSlots.AddRange(addedItems.Select(item => item.Id));

        return (existingLocation, resourceIdsToRegenerateBookingSlots.Distinct().ToList());
    }

    private async Task<Location> RebuildDesksAsync(
        Shared.Models.Location location,
        Location existingLocation,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        var organizationTags = new List<OrganizationTag>();
        if (organization is not null)
        {
            var organizationTagIds = location.Desks.SelectMany(item => item.OrganizationTags.Select(tag => tag.Id)).Distinct().ToList();
            foreach (var tagId in organizationTagIds)
            {
                organizationTags.Add(await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(tagId, organization, cancellationToken));
            }
        }

        var desks = await repositoryFactory.DeskRepository.GetByLocationIdAsync(existingLocation.Id, cancellationToken);
        var itemsToRemove = desks.Where(desk => location.Desks.All(item => item.Id != desk.Id)).ToList();
        var updatedItems = desks
            .Where(desk => location.Desks.Any(item => item.Id == desk.Id))
            .Select(desk =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag =>
                        location.Desks.First(item => item.Id == desk.Id).OrganizationTags.Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();

                var updatedDesk = mapper.MergeToEntity(
                    location.Desks.First(item => item.Id == desk.Id),
                    desk,
                    existingLocation,
                    filteredOrganizationTags);
                updatedDesk.DeletedAt = null;
                return repositoryFactory.DeskRepository.Update(updatedDesk);
            })
            .ToList();
        var addedItems = location.Desks
            .Where(desk => desks.All(item => item.Id != desk.Id))
            .Select(desk =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag => desk.OrganizationTags.Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();

                return repositoryFactory.DeskRepository.Add(mapper.MapToEntity(desk, existingLocation, filteredOrganizationTags));
            })
            .ToList();

        repositoryFactory.DeskRepository.RemoveRange(itemsToRemove);
        existingLocation.Desks = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingLocation;
    }

    private async Task<Location> RebuildRoomsAsync(
        Shared.Models.Location location,
        Location existingLocation,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        var organizationTags = new List<OrganizationTag>();
        if (organization is not null)
        {
            var organizationTagIds = location.Rooms.SelectMany(item => item.OrganizationTags.Select(tag => tag.Id)).Distinct().ToList();
            foreach (var tagId in organizationTagIds)
            {
                organizationTags.Add(await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(tagId, organization, cancellationToken));
            }
        }

        var rooms = await repositoryFactory.RoomRepository.GetByLocationIdAsync(existingLocation.Id, cancellationToken);
        var itemsToRemove = rooms.Where(room => location.Rooms.All(item => item.Id != room.Id)).ToList();
        var updatedItems = rooms
            .Where(room => location.Rooms.Any(item => item.Id == room.Id))
            .Select(room =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag =>
                        location.Rooms.First(item => item.Id == room.Id).OrganizationTags.Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();

                var updatedRoom = mapper.MergeToEntity(
                    location.Rooms.First(item => item.Id == room.Id),
                    room,
                    existingLocation,
                    filteredOrganizationTags);
                updatedRoom.DeletedAt = null;
                return repositoryFactory.RoomRepository.Update(updatedRoom);
            })
            .ToList();
        var addedItems = location.Rooms
            .Where(room => rooms.All(item => item.Id != room.Id))
            .Select(room =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag => room.OrganizationTags.Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();

                return repositoryFactory.RoomRepository.Add(mapper.MapToEntity(room, existingLocation, filteredOrganizationTags));
            })
            .ToList();

        repositoryFactory.RoomRepository.RemoveRange(itemsToRemove);
        existingLocation.Rooms = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingLocation;
    }

    private async Task<Location> RebuildLocationMembersAsync(
        Shared.Models.Location location,
        Location existingLocation,
        CancellationToken cancellationToken)
    {
        var locationMembers = await repositoryFactory.LocationMemberRepository.GetByLocationIdAsync(existingLocation.Id, cancellationToken);
        var itemsToRemove = locationMembers
            .Where(locationMember => location.LocationMembers.All(item => item.Id != locationMember.Id))
            .ToList();
        var updatedItems = new List<LocationMember>();
        foreach (var locationMember in locationMembers.Where(locationMember => location.LocationMembers.Any(item => item.Id == locationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id, false, cancellationToken);

            var updatedLocationMember = mapper.MergeToEntity(
                location.LocationMembers.First(item => item.Id == locationMember.Id),
                locationMember,
                existingLocation,
                customer);
            updatedLocationMember.DeletedAt = null;
            updatedItems.Add(repositoryFactory.LocationMemberRepository.Update(updatedLocationMember));
        }

        var addedItems = new List<LocationMember>();
        foreach (var locationMember in location.LocationMembers.Where(locationMember => locationMembers.All(item => item.Id != locationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id, false, cancellationToken);
            addedItems.Add(repositoryFactory.LocationMemberRepository.Add(mapper.MapToEntity(locationMember, existingLocation, customer)));
        }

        repositoryFactory.LocationMemberRepository.RemoveRange(itemsToRemove);
        existingLocation.LocationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingLocation;
    }
}
