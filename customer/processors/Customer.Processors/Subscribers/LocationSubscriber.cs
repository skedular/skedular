using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Customer.Processors.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using Location = Customer.Shared.Database.Entities.Location;
using LocationMember = Customer.Shared.Database.Entities.LocationMember;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

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
                        await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, true, cancellationToken);
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
                true,
                cancellationToken);

        existingLocation = existingLocation is null
            ? repositoryFactory.LocationRepository.Add(mapper.MapToEntity(location, organization))
            : repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation,
                organization));

        existingLocation = await RebuildDesks(location, existingLocation, cancellationToken);
        _ = await RebuildLocationMembersAsync(location, existingLocation, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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

    private async Task<Location> RebuildDesks(
        Shared.Models.Location location,
        Location existingLocation,
        CancellationToken cancellationToken)
    {
        var desks = await repositoryFactory.DeskRepository.GetByLocationIdAsync(
            existingLocation.Id,
            cancellationToken);
        var itemsToRemove = desks
            .Where(desk => location.Desks.All(item => item.Id != desk.Id)).ToList();
        var updatedItems = desks
            .Where(desk => location.Desks.Any(item => item.Id == desk.Id))
            .Select(desk =>
            {
                var updatedDesk = mapper.MergeToEntity(
                    location.Desks.First(item => item.Id == desk.Id),
                    desk,
                    existingLocation);
                updatedDesk.DeletedAt = null;
                return repositoryFactory.DeskRepository.Update(updatedDesk);
            })
            .ToList();
        var addedItems = location.Desks
            .Where(desk => desks.All(item => item.Id != desk.Id))
            .Select(desk =>
                repositoryFactory.DeskRepository.Add(mapper.MapToEntity(desk, existingLocation)))
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
                await repositoryFactory.CustomerRepository.GetByIdAsync(locationMember.Customer.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

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
                await repositoryFactory.CustomerRepository.GetByIdAsync(locationMember.Customer.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

            addedItems.Add(repositoryFactory.LocationMemberRepository.Add(
                mapper.MapToEntity(locationMember, existingLocation, customer)));
        }

        repositoryFactory.LocationMemberRepository.RemoveRange(itemsToRemove);
        existingLocation.LocationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();
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

            var existingLocationIds = customer.DefaultLocations.Select(item => item.Id).Distinct().ToList();
            customer.DefaultLocations =
                customer.DefaultLocations.Where(item => item.Id != existingLocation.Id).ToList();
            var newLocationIds = customer.DefaultLocations.Select(item => item.Id).Distinct().ToList();

            var existingDeskIds = customer.PreferredDesks.Select(item => item.Id).Distinct().ToList();
            customer.PreferredDesks =
                customer.PreferredDesks.Where(item => item.Location.Id != existingLocation.Id).ToList();
            var newDeskIds = customer.PreferredDesks.Select(item => item.Id).Distinct().ToList();

            customer = repositoryFactory.CustomerRepository.Update(customer);

            if (newLocationIds.Count != existingLocationIds.Count ||
                newLocationIds.Except(existingLocationIds).Any() ||
                newDeskIds.Count != existingDeskIds.Count ||
                newDeskIds.Except(existingDeskIds).Any())
            {
                await customerPublisher.PublishCustomerAsync([mapper.MapTo(customer)!], cancellationToken);
            }
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
            customer.PreferredDesks = customer.PreferredDesks.Where(item => item.Location.Id != location.Id).ToList();
            _ = repositoryFactory.CustomerRepository.Update(customer);
        }
    }
}
