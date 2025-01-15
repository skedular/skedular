using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Address = Location.Shared.Database.Entities.Address;
using Booking = Location.Shared.Database.Entities.Booking;
using Customer = Location.Shared.Models.Customer;
using LocationMember = Location.Shared.Database.Entities.LocationMember;
using Organization = Location.Shared.Database.Entities.Organization;

namespace Location.Api.Services;

public interface ILocationService
{
    Task<Shared.Models.Location> AddAsync(
        Shared.Models.Location location,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Location> UpdateAsync(Shared.Models.Location location, CancellationToken cancellationToken);
    Task<Shared.Models.Location> DeleteAsync(string locationId, CancellationToken cancellationToken);

    Task<Shared.Models.Location?> GetByIdAsync(
        string locationId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<ICollection<Shared.Models.Location>> GetMyLocationsAsync(
        string? organizationId,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Location>>, int )> GetPaginatedLocationsAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        ICollection<LocationOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class LocationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationAuthorizationService locationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IMapper mapper,
    TimeProvider timeProvider) : ILocationService
{
    public async Task<Shared.Models.Location> AddAsync(
        Shared.Models.Location location,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var (customer, customerEntity) = await customerService.GetNullableAsync(cancellationToken);
        Organization? organization = null;
        if (location.Organization is not null)
        {
            organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                location.Organization.Id,
                cancellationToken);

            if (!ignoreAuthorizationCheck)
            {
                if (customer is null)
                {
                    throw new CustomerNotFound();
                }

                if (!organizationAuthorizationService.CanModify(organization, customer))
                {
                    throw new Unauthorized();
                }

                if (!organizationOfferingService.CanCreateLocation(organization) ||
                    !organizationOfferingService.IsMoreInteractionAllowed(organization, customer))
                {
                    throw new NoMoreInteractionAllowed();
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(location.Id))
        {
            var existingLocation =
                await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
            if (existingLocation is not null)
            {
                if (!ignoreAuthorizationCheck && customer is null)
                {
                    throw new CustomerNotFound();
                }

                return await UpdateInternalAsync(
                    location,
                    existingLocation,
                    customer,
                    cancellationToken);
            }
        }
        else
        {
            location.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        var locationEntity = mapper.MapTo(location, organization);
        var physicalAddress = location.PhysicalAddress is null
            ? null
            : mapper.MapTo(location.PhysicalAddress, locationEntity);

        if (string.IsNullOrWhiteSpace(location.Organization?.Id))
        {
            var locationMembers = new List<LocationMember>();
            if (customerEntity is not null)
            {
                locationMembers.Add(new LocationMember
                {
                    Id = randomHelper.Generate(),
                    Role = LocationRoleConstants.Owner,
                    Customer = customerEntity,
                    Location = locationEntity
                });
            }

            locationEntity.LocationMembers = locationMembers;
            repositoryFactory.LocationMemberRepository.AddRange(locationMembers);
        }

        if (physicalAddress is not null)
        {
            physicalAddress.Id = randomHelper.Generate();
            _ = repositoryFactory.AddressRepository.Add(physicalAddress);
        }

        locationEntity = repositoryFactory.LocationRepository.Add(locationEntity);
        location = mapper.MapTo(locationEntity);

        await locationOutboxPublisher.PublishLocationAsync(
            [location],
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.AddressRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return location;
    }

    public async Task<Shared.Models.Location> UpdateAsync(
        Shared.Models.Location location,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(location.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        return await UpdateInternalAsync(location, existingLocation, customer, cancellationToken);
    }

    public async Task<Shared.Models.Location> DeleteAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!locationAuthorizationService.CanDelete(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        var deletedLocation = mapper.MapTo(repositoryFactory.LocationRepository.Remove(existingLocation));

        await locationOutboxPublisher.PublishLocationAsync(
            [deletedLocation],
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedLocation;
    }

    public async Task<Shared.Models.Location?> GetByIdAsync(
        string locationId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return null;
        }

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            return null;
        }

        return await EnrichLocationAsync(customer, location, cancellationToken);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Location>>, int)> GetPaginatedLocationsAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        ICollection<LocationOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
            // Ensure we do not return other customer location by forcing CustomerId as search criteria
            searchCriteria.CustomerId = customer.Id;
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.LocationRepository.GetPaginatedLocationsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        var mappedLocations = new List<Edge<Shared.Models.Location>>();
        foreach (var edge in edges)
        {
            var enrichedLocation = await EnrichLocationAsync(customer, edge.Node, cancellationToken);

            searchCriteria.CustomTagIds.ForEach(id =>
                enrichedLocation.Desks = enrichedLocation.Desks
                    .Where(desk => desk.CustomTags.Select(tag => tag.Id).Contains(id))
                    .ToList());

            searchCriteria.ZoneIds.ForEach(id =>
                enrichedLocation.Desks = enrichedLocation.Desks
                    .Where(desk => desk.Zones.Select(tag => tag.Id).Contains(id))
                    .ToList());

            mappedLocations.Add(new Edge<Shared.Models.Location>(edge.Cursor, enrichedLocation));
        }

        return (paginatedInfo, mappedLocations, totalCount);
    }

    public async Task<ICollection<Shared.Models.Location>> GetMyLocationsAsync(
        string? organizationId,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            var organization =
                await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (!organizationAuthorizationService.CanView(organization, customer))
            {
                throw new Unauthorized();
            }
        }

        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdAsync(
            customer.Id,
            organizationId,
            cancellationToken);
        return locations.Select(mapper.MapTo).ToList();
    }

    private async Task<Shared.Models.Location> UpdateInternalAsync(
        Shared.Models.Location location,
        Shared.Database.Entities.Location existingLocation,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        Address? physicalAddress = null;

        if (location.PhysicalAddress is null && existingLocation.PhysicalAddress is not null)
        {
            repositoryFactory.AddressRepository.Remove(existingLocation.PhysicalAddress);
        }
        else if (location.PhysicalAddress is not null && existingLocation.PhysicalAddress is null)
        {
            physicalAddress = mapper.MapTo(location.PhysicalAddress, existingLocation);
            physicalAddress.Id = randomHelper.Generate();
            repositoryFactory.AddressRepository.Add(physicalAddress);
        }
        else if (location.PhysicalAddress is not null && existingLocation.PhysicalAddress is not null)
        {
            physicalAddress = mapper.MergeToEntity(
                location.PhysicalAddress,
                existingLocation.PhysicalAddress,
                existingLocation);
            repositoryFactory.AddressRepository.Update(physicalAddress);
        }

        location = mapper.MapTo(
            repositoryFactory.LocationRepository.Update(
                mapper.MergeTo(location, existingLocation, physicalAddress)));

        await locationOutboxPublisher.PublishLocationAsync(
            [location],
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.AddressRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return location;
    }

    private async Task<Shared.Models.Location> EnrichLocationAsync(
        Customer? customer,
        Shared.Database.Entities.Location locationEdge,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !locationAuthorizationService.CanView(locationEdge, customer))
        {
            throw new Unauthorized();
        }

        var mappedLocation = mapper.MapTo(locationEdge);

        mappedLocation.CustomTags = mappedLocation.Desks
            .SelectMany(item => item.CustomTags.Select(customTag => new OrganizationTag
            {
                Id = customTag.Id, Name = customTag.Name, Type = OrganizationTagType.Custom, Color = customTag.Color
            }))
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .ToList();

        mappedLocation.Zones = mappedLocation.Desks
            .SelectMany(item => item.Zones.Select(zone => new OrganizationTag
            {
                Id = zone.Id, Name = zone.Name, Type = OrganizationTagType.Zone, Color = zone.Color
            }))
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .ToList();

        if (customer is not null)
        {
            mappedLocation.Permissions = new Permissions
            {
                CanView = locationAuthorizationService.CanView(locationEdge, customer),
                CanModify = locationAuthorizationService.CanModify(locationEdge, customer),
                CanDelete = locationAuthorizationService.CanDelete(locationEdge, customer),
                CanInvitePeople = locationAuthorizationService.CanInvitePeople(locationEdge, customer),
                CanCancelPeopleExistingInvitations =
                    locationAuthorizationService.CanCancelPeopleExistingInvitations(locationEdge, customer),
                CanViewAnalytics = locationAuthorizationService.CanViewAnalytics(locationEdge, customer)
            };
        }

        var now = timeProvider.GetUtcNow();
        mappedLocation.HasFutureBooking = await repositoryFactory.BookingRepository
            .Query(new Specification<Booking>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.Location.Id == locationEdge.Id && query.From >= now
            })
            .AnyAsync(cancellationToken);

        return mappedLocation;
    }
}
