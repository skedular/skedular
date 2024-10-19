using Api.Shared.Clients.Events.UnityHub.Location.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Location.V1.Value;
using Customer.Processors.Mappers;
using Customer.Shared.Database.Entities;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using Location = Customer.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Type;

namespace Customer.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICustomerPublisher customerPublisher) : IEventSubscriber<Key, Event>
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
                    var existingLocation =
                        await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
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
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                location.Organization.Id,
                cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        existingLocation = existingLocation is null
            ? repositoryFactory.LocationRepository.Add(mapper.MapToEntity(location, organization))
            : repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation,
                organization));

        existingLocation = RebuildLocationTags(location, existingLocation);
        existingLocation = RebuildDesks(location, existingLocation);
        _ = await RebuildLocationMembersAsync(location, existingLocation, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationTagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        await UpdateCustomerDefaultLocationsAsync(existingLocation, cancellationToken);
        await UpdateLocationMembersDefaultLocationsAsync(
            existingLocation,
            existingLocation.LocationMembers,
            cancellationToken);
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private Location RebuildLocationTags(Shared.Models.Location location, Location existingLocation)
    {
        var itemsToRemove = existingLocation.Tags
            .Where(identity => location.Tags.All(item => item.Id != identity.Id)).ToList();
        var updatedItems = existingLocation.Tags
            .Where(locationTag => location.Tags.Any(item => item.Id == locationTag.Id))
            .Select(locationTag => repositoryFactory.LocationTagRepository.Update(mapper.MergeToEntity(
                location.Tags.Single(item => item.Id == locationTag.Id),
                locationTag, existingLocation)))
            .ToList();
        var addedItems = location.Tags
            .Where(locationTag => existingLocation.Tags.All(item => item.Id != locationTag.Id))
            .Select(locationTag =>
                repositoryFactory.LocationTagRepository.Add(mapper.MapToEntity(locationTag, existingLocation)))
            .ToList();

        repositoryFactory.LocationTagRepository.RemoveRange(itemsToRemove);
        existingLocation.Tags = addedItems.Concat(updatedItems).ToList();

        return existingLocation;
    }

    private Location RebuildDesks(Shared.Models.Location location, Location existingLocation)
    {
        var itemsToRemove = existingLocation.Desks
            .Where(identity => location.Desks.All(item => item.Id != identity.Id)).ToList();
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

    private async Task<Location> RebuildLocationMembersAsync(
        Shared.Models.Location location,
        Location existingLocation,
        CancellationToken cancellationToken)
    {
        var itemsToRemove = existingLocation.LocationMembers
            .Where(locationMember => location.LocationMembers.All(item => item.Id != locationMember.Id))
            .ToList();
        var updatedItems = new List<LocationMember>();
        foreach (var locationMember in existingLocation.LocationMembers
                     .Where(locationMember => location.LocationMembers.Any(item => item.Id == locationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id,
                    cancellationToken);
            await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            updatedItems.Add(repositoryFactory.LocationMemberRepository.Update(mapper.MergeToEntity(
                location.LocationMembers.Single(item => item.Id == locationMember.Id),
                locationMember,
                existingLocation,
                customer)));
        }

        var addedItems = new List<LocationMember>();
        foreach (var locationMember in location.LocationMembers
                     .Where(locationMember =>
                         existingLocation.LocationMembers.All(item => item.Id != locationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(locationMember.Customer.Id,
                    cancellationToken);
            await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            addedItems.Add(repositoryFactory.LocationMemberRepository.Add(
                mapper.MapToEntity(locationMember, existingLocation, customer)));
        }

        repositoryFactory.LocationMemberRepository.RemoveRange(itemsToRemove);
        existingLocation.LocationMembers = addedItems.Concat(updatedItems).ToList();
        await UpdateLocationMembersDefaultLocationsAsync(existingLocation, itemsToRemove, cancellationToken);

        return existingLocation;
    }

    private async Task UpdateLocationMembersDefaultLocationsAsync(
        Location existingLocation,
        IEnumerable<LocationMember> locationMembersToRemove,
        CancellationToken cancellationToken)
    {
        var locationMemberIds = locationMembersToRemove.Select(locationMember => locationMember.Id).ToList();
        foreach (var locationMemberId in locationMemberIds)
        {
            var member = await repositoryFactory.LocationMemberRepository
                .Query(new Specification<LocationMember> { Criteria = query => query.Id == locationMemberId }
                    .AddInclude(query => query.Customer))
                .FirstAsync(cancellationToken);

            var customer =
                await repositoryFactory.CustomerRepository.GetByIdAsync(member.Customer.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

            customer.DefaultLocations =
                customer.DefaultLocations.Where(item => item.Id != existingLocation.Id).ToList();

            customer.PreferredLocationTags =
                customer.PreferredLocationTags.Where(item => item.Location.Id != existingLocation.Id).ToList();

            customer.PreferredDesks =
                customer.PreferredDesks.Where(item => item.Location.Id != existingLocation.Id).ToList();

            customer = repositoryFactory.CustomerRepository.Update(customer);
            await customerPublisher.PublishCustomerAsync([mapper.MapTo(customer)!], cancellationToken);
        }
    }

    private async Task UpdateCustomerDefaultLocationsAsync(Location location, CancellationToken cancellationToken)
    {
        var customerIds = location.DefaultedByCustomers.Select(customer => customer.Id).ToList();
        foreach (var customerId in customerIds)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

            customer.DefaultLocations = customer.DefaultLocations.Where(item => item.Id != location.Id).ToList();
            customer.PreferredLocationTags =
                customer.PreferredLocationTags.Where(item => item.Location.Id != location.Id).ToList();
            customer.PreferredDesks = customer.PreferredDesks.Where(item => item.Location.Id != location.Id).ToList();
            _ = repositoryFactory.CustomerRepository.Update(customer);
        }
    }
}
