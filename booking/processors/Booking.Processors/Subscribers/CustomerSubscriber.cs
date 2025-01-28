using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Booking.Processors.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Kafka.Consume;
using Customer = Booking.Shared.Models.Customer;
using Desk = Booking.Shared.Database.Entities.Desk;
using Room = Booking.Shared.Database.Entities.Room;
using Location = Booking.Shared.Database.Entities.Location;
using OrganizationTag = Booking.Shared.Database.Entities.OrganizationTag;
using Team = Booking.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class CustomerSubscriber(
    ILogger<CustomerSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.CustomerUpserted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customer.Id, cancellationToken);
                    if (existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleCustomerUpsertedEventAsync(customer, existingCustomer, cancellationToken);
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
        Customer customer,
        Shared.Database.Entities.Customer? existingCustomer,
        CancellationToken cancellationToken)
    {
        var defaultOrganization = customer.DefaultOrganization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                customer.DefaultOrganization.Id,
                cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var defaultLocations = new List<Location>();
        foreach (var item in customer.DefaultLocations)
        {
            var organization = item.Organization is null
                ? null
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                    item.Organization!.Id,
                    cancellationToken);
            defaultLocations.Add(
                await repositoryFactory.LocationRepository.UpsertNakedAsync(
                    item.Id,
                    organization,
                    cancellationToken));

            await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var defaultTeams = new List<Team>();
        foreach (var item in customer.DefaultTeams)
        {
            var organization = item.Organization is null
                ? null
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(item.Organization!.Id, cancellationToken);
            defaultTeams.Add(await repositoryFactory.TeamRepository.UpsertNakedAsync(item.Id, organization, cancellationToken));

            await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var preferredDesks = new List<Desk>();
        foreach (var item in customer.PreferredDesks)
        {
            if (item.Location is not null)
            {
                var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(item.Location.Id, null, cancellationToken);
                preferredDesks.Add(await repositoryFactory.DeskRepository.UpsertNakedAsync(item.Id, location, cancellationToken));

                await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        var preferredRooms = new List<Room>();
        foreach (var item in customer.PreferredRooms)
        {
            if (item.Location is not null)
            {
                var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(item.Location.Id, null, cancellationToken);
                preferredRooms.Add(await repositoryFactory.RoomRepository.UpsertNakedAsync(item.Id, location, cancellationToken));

                await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                await repositoryFactory.RoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        var preferredOrganizationTags = new List<OrganizationTag>();
        foreach (var item in customer.PreferredOrganizationTags)
        {
            var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(item.Organization.Id, cancellationToken);
            preferredOrganizationTags.Add(
                await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(item.Id, organization, cancellationToken));

            await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.OrganizationTagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        if (existingCustomer is null)
        {
            var identities = mapper.MapToEntity(customer.Identities, null).ToList();
            existingCustomer = mapper.MapToEntity(
                customer,
                identities,
                defaultOrganization,
                defaultLocations,
                defaultTeams,
                preferredDesks,
                preferredRooms,
                preferredOrganizationTags);

            identities.ForEach(identity => identity.Customer = existingCustomer);
            repositoryFactory.IdentityRepository.AddRange(identities);
            existingCustomer.Identities = identities;
            _ = repositoryFactory.CustomerRepository.Add(existingCustomer);
        }
        else
        {
            _ = RebuildIdentities(customer, existingCustomer);
            repositoryFactory.CustomerRepository.Update(
                mapper.MergeToEntity(
                    customer,
                    existingCustomer,
                    existingCustomer.Identities,
                    defaultOrganization,
                    defaultLocations,
                    defaultTeams,
                    preferredDesks,
                    preferredRooms,
                    preferredOrganizationTags)
            );
        }

        await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleCustomerDeletedEventAsync(Shared.Database.Entities.Customer existingCustomer, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.CustomerRepository.Remove(existingCustomer);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private Shared.Database.Entities.Customer RebuildIdentities(Customer customer, Shared.Database.Entities.Customer existingCustomer)
    {
        var itemsToRemove = existingCustomer.Identities
            .Where(identity => customer.Identities.All(item => item.Id != identity.Id))
            .ToList();
        var updatedItems = existingCustomer.Identities
            .Where(identity => customer.Identities.Any(item => item.Id == identity.Id))
            .Select(identity => repositoryFactory.IdentityRepository.Update(
                mapper.MergeToEntity(
                    customer.Identities.First(item => item.Id == identity.Id),
                    identity,
                    existingCustomer)))
            .ToList();
        var addedItems = customer.Identities
            .Where(identity => existingCustomer.Identities.All(item => item.Id != identity.Id))
            .Select(identity =>
                repositoryFactory.IdentityRepository.Add(mapper.MapToEntity(identity, existingCustomer)))
            .ToList();

        repositoryFactory.IdentityRepository.RemoveRange(itemsToRemove);
        existingCustomer.Identities = addedItems.Concat(updatedItems).ToList();

        return existingCustomer;
    }
}
