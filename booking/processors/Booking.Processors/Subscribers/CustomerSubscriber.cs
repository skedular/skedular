using Api.Shared.Clients.Events.UnityHub.Customer.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Customer.V1.Value;
using Booking.Processors.Mappers;
using Booking.Shared.Repositories;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Customer = Booking.Shared.Models.Customer;
using Desk = Booking.Shared.Database.Entities.Desk;
using Location = Booking.Shared.Database.Entities.Location;
using LocationTag = Booking.Shared.Database.Entities.LocationTag;
using Team = Booking.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class CustomerSubscriber(
    ILogger<CustomerSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.CustomerUpserted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer =
                        await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
                    if (existingCustomer is not null && existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    await HandleCustomerUpsertedEventAsync(customer, existingCustomer, cancellationToken);
                }
                break;

            case Type.CustomerDeleted:
                {
                    var customer = mapper.MapTo(@event);
                    var existingCustomer =
                        await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
                    if (existingCustomer is not null && existingCustomer.EventRaisedAt > customer.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Customer event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    if (existingCustomer is null)
                    {
                        return;
                    }

                    await HandleCustomerDeletedEventAsync(existingCustomer, cancellationToken);
                }
                break;

            default:
                return;
        }
    }

    private async Task HandleCustomerUpsertedEventAsync(
        Customer customer,
        Shared.Database.Entities.Customer? existingCustomer,
        CancellationToken cancellationToken)
    {
        var defaultOrganization = customer.DefaultOrganization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(customer.DefaultOrganization.Id,
                cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var defaultLocations = new List<Location>();
        foreach (var item in customer.DefaultLocations)
        {
            var organization = item.Organization is null
                ? null
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(item.Organization!.Id,
                    cancellationToken);
            defaultLocations.Add(
                await repositoryFactory.LocationRepository.UpsertNakedAsync(item.Id, organization,
                    cancellationToken));

            await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var defaultTeams = new List<Team>();
        foreach (var item in customer.DefaultTeams)
        {
            var organization = item.Organization is null
                ? null
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(item.Organization!.Id,
                    cancellationToken);
            defaultTeams.Add(await repositoryFactory.TeamRepository.UpsertNakedAsync(item.Id, organization,
                cancellationToken));

            await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var preferredLocationTags = new List<LocationTag>();
        foreach (var item in customer.PreferredLocationTags)
        {
            var location =
                await repositoryFactory.LocationRepository.UpsertNakedAsync(item.Location.Id, null,
                    cancellationToken);
            preferredLocationTags.Add(
                await repositoryFactory.LocationTagRepository.UpsertNakedAsync(item.Id, location,
                    cancellationToken));

            await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.LocationTagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var preferredDesks = new List<Desk>();
        foreach (var item in customer.PreferredDesks)
        {
            var location =
                await repositoryFactory.LocationRepository.UpsertNakedAsync(item.Location.Id, null,
                    cancellationToken);
            preferredDesks.Add(
                await repositoryFactory.DeskRepository.UpsertNakedAsync(item.Id, location,
                    cancellationToken));

            await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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
                preferredLocationTags,
                preferredDesks);

            identities.ForEach(identity => identity.Customer = existingCustomer);
            repositoryFactory.IdentityRepository.AddRange(identities);
            existingCustomer.Identities = identities;
            _ = repositoryFactory.CustomerRepository.Add(existingCustomer);
        }
        else
        {
            _ = await RebuildIdentitiesAsync(customer, existingCustomer, cancellationToken);
            repositoryFactory.CustomerRepository.Update(
                mapper.MergeToEntity(
                    customer,
                    existingCustomer,
                    existingCustomer.Identities,
                    defaultOrganization,
                    defaultLocations,
                    defaultTeams,
                    preferredLocationTags,
                    preferredDesks)
            );
        }

        await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleCustomerDeletedEventAsync(
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.CustomerRepository.Remove(existingCustomer);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Shared.Database.Entities.Customer> RebuildIdentitiesAsync(
        Customer customer,
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        var itemsToRemove = existingCustomer.Identities
            .Where(identity => customer.Identities.All(item => item.Id != identity.Id)).ToList();
        var updatedItems = existingCustomer.Identities
            .Where(identity => customer.Identities.Any(item => item.Id == identity.Id))
            .Select(identity => repositoryFactory.IdentityRepository.Update(
                mapper.MergeToEntity(
                    customer.Identities.Single(item => item.Id == identity.Id),
                    identity,
                    existingCustomer)))
            .ToList();
        var addedItems = customer.Identities
            .Where(identity => existingCustomer.Identities.All(item => item.Id != identity.Id))
            .Select(identity =>
                repositoryFactory.IdentityRepository.Add(mapper.MapToEntity(identity, existingCustomer)))
            .ToList();

        repositoryFactory.IdentityRepository.RemoveRange(itemsToRemove);
        await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        existingCustomer.Identities = addedItems.Concat(updatedItems).ToList();

        return existingCustomer;
    }
}
