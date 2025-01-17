using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Kafka.Consume;
using IMapper = Booking.Processors.Mappers.IMapper;
using Location = Booking.Shared.Database.Entities.Location;
using LocationMember = Booking.Shared.Database.Entities.LocationMember;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    var organization = location.Organization is null
                        ? null
                        : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                            location.Organization.Id,
                            cancellationToken);
                    var existingLocation = await repositoryFactory.LocationRepository.UpsertNakedAsync(
                        location.Id,
                        organization,
                        cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
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
                        await repositoryFactory.LocationRepository.GetByIdAsync(
                            location.Id,
                            true,
                            cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
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
        Location? existingLocation,
        CancellationToken cancellationToken)
    {
        var organization = location.Organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.GetByIdAsync(
                location.Organization.Id,
                true,
                cancellationToken);

        existingLocation = existingLocation is null
            ? repositoryFactory.LocationRepository.Add(mapper.MapToEntity(location, organization))
            : repositoryFactory.LocationRepository.Update(
                mapper.MergeToEntity(location, existingLocation, organization));

        existingLocation = await RebuildDesksAsync(location, existingLocation, organization, cancellationToken);
        _ = await RebuildLocationMembersAsync(location, existingLocation, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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
            var organizationTagIds =
                location.Desks.SelectMany(item => item.OrganizationTags.Select(tag => tag.Id)).ToList();

            foreach (var tagId in organizationTagIds)
            {
                organizationTags.Add(
                    await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(
                        tagId,
                        organization,
                        cancellationToken));
            }
        }

        var desks = await repositoryFactory.DeskRepository.GetByLocationIdAsync(
            existingLocation.Id,
            cancellationToken);
        var itemsToRemove = desks
            .Where(desk => location.Desks.All(item => item.Id != desk.Id)).ToList();
        var updatedItems = desks
            .Where(desk => location.Desks.Any(item => item.Id == desk.Id))
            .Select(desk =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag =>
                        location.Desks
                            .First(item => item.Id == desk.Id).OrganizationTags
                            .Any(organizationTag => organizationTag.Id == tag.Id))
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

                return repositoryFactory.DeskRepository.Add(
                    mapper.MapToEntity(desk, existingLocation, filteredOrganizationTags));
            })
            .ToList();

        repositoryFactory.DeskRepository.RemoveRange(itemsToRemove);
        existingLocation.Desks = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingLocation;
    }

    private async Task<Location> RebuildLocationMembersAsync(
        Shared.Models.Location location,
        Location existingLocation,
        CancellationToken cancellationToken)
    {
        var locationMembers = await repositoryFactory.LocationMemberRepository.GetByLocationIdAsync(
            existingLocation.Id,
            cancellationToken);
        var itemsToRemove = locationMembers
            .Where(locationMember => location.LocationMembers.All(item => item.Id != locationMember.Id))
            .ToList();
        var updatedItems = new List<LocationMember>();
        foreach (var locationMember in locationMembers
                     .Where(locationMember => location.LocationMembers.Any(item => item.Id == locationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id,
                    cancellationToken);
            await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            var updatedLocationMember = mapper.MergeToEntity(
                location.LocationMembers.First(item => item.Id == locationMember.Id),
                locationMember,
                existingLocation,
                customer);
            updatedLocationMember.DeletedAt = null;
            updatedItems.Add(repositoryFactory.LocationMemberRepository.Update(updatedLocationMember));
        }

        var addedItems = new List<LocationMember>();
        foreach (var locationMember in location.LocationMembers
                     .Where(locationMember =>
                         locationMembers.All(item => item.Id != locationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id,
                    cancellationToken);
            await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            addedItems.Add(repositoryFactory.LocationMemberRepository.Add(
                mapper.MapToEntity(locationMember, existingLocation, customer)));
        }

        repositoryFactory.LocationMemberRepository.RemoveRange(itemsToRemove);
        existingLocation.LocationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingLocation;
    }
}
