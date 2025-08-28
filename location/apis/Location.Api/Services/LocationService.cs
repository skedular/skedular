using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Microsoft.EntityFrameworkCore;
using Booking = Location.Shared.Database.Entities.Booking;
using Customer = Location.Shared.Models.Customer;
using Organization = Location.Shared.Database.Entities.Organization;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Api.Services;

public interface ILocationService
{
    Task<Shared.Models.Location> AddAsync(Shared.Models.Location location, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Shared.Models.Location> UpdateAsync(Shared.Models.Location location, CancellationToken cancellationToken);
    Task<Shared.Models.Location> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Location?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<ICollection<Shared.Models.Location>> GetMyLocationsAsync(string? organizationUniqueAlphanumericName, CancellationToken cancellationToken);

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
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    ITemporalOutboxPublisher temporalOutboxPublisher,
    IMapper mapper,
    TimeProvider timeProvider) : ILocationService
{
    public async Task<Shared.Models.Location> AddAsync(
        Shared.Models.Location location,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(location.Organization);

        var (customer, _) = await customerService.GetNullableAsync(cancellationToken);

        Organization organization;
        if (!string.IsNullOrWhiteSpace(location.Organization.Id))
        {
            organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization.Id, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(location.Organization.UniqueAlphanumericName))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                location.Organization.Id,
                location.Organization.UniqueAlphanumericName,
                false,
                false,
                cancellationToken) ?? throw new OrganizationNotFound();
        }
        else
        {
            throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
        }

        if (!ignoreAuthorizationCheck)
        {
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            if (!organizationAuthorizationService.CanModify(organization, customer))
            {
                throw new UnauthorizedAccessException();
            }

            if (!organizationOfferingService.CanCreateLocation(organization) ||
                !organizationOfferingService.IsMoreInteractionAllowed(organization, customer))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        if (!string.IsNullOrWhiteSpace(location.Id))
        {
            var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
            if (existingLocation is not null)
            {
                if (!ignoreAuthorizationCheck && customer is null)
                {
                    throw new CustomerNotFound();
                }

                return await UpdateInternalAsync(location, existingLocation, customer, cancellationToken);
            }
        }
        else
        {
            location.Id = randomHelper.Generate();
        }

        var locationRef = location;
        var organizationTags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    locationRef.Tags.Select(item => item.Id).Contains(query.Id) &&
                                    query.Organization.Id == locationRef.Organization.Id &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var locationEntity = mapper.MapTo(location, organization, organizationTags);

        locationEntity.OpeningHours = OpeningHours.Default;
        locationEntity = repositoryFactory.LocationRepository.Add(locationEntity);
        location = mapper.MapTo(locationEntity);

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        temporalOutboxPublisher.StartWorkflowLocationDailyAnalytics(
            new GenerateLocationDailyAnalyticsInput(organization.Id, timeProvider.GetUtcNow().AddDays(1)),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return location;
    }

    public async Task<Shared.Models.Location> UpdateAsync(Shared.Models.Location location, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(location.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken) ??
                               throw new LocationNotFound();
        if (!organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        return await UpdateInternalAsync(location, existingLocation, customer, cancellationToken);
    }

    public async Task<Shared.Models.Location> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(id, cancellationToken) ?? throw new LocationNotFound();
        if (!organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!organizationAuthorizationService.CanDelete(existingLocation.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedLocation = mapper.MapTo(repositoryFactory.LocationRepository.Remove(existingLocation));

        locationOutboxPublisher.PublishLocations([deletedLocation], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedLocation;
    }

    public async Task<Shared.Models.Location?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            customer = await cachedCustomerService.GetAsync(cancellationToken);
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(id, cancellationToken);
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
            customer = await cachedCustomerService.GetAsync(cancellationToken);
            // Ensure we do not return another customer location by forcing CustomerId as search criteria
            searchCriteria = searchCriteria with { CustomerId = customer.Id };
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

            searchCriteria.TagIds.ForEach(id =>
                enrichedLocation.Resources = enrichedLocation.Resources.Where(desk => desk.Tags.Select(tag => tag.Id).Contains(id)).ToList());

            mappedLocations.Add(new Edge<Shared.Models.Location>(enrichedLocation, edge.Cursor));
        }

        return (paginatedInfo, mappedLocations, totalCount);
    }

    public async Task<ICollection<Shared.Models.Location>> GetMyLocationsAsync(string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);

        Organization? organization = null;
        if (!string.IsNullOrWhiteSpace(organizationUniqueAlphanumericName))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               null,
                               organizationUniqueAlphanumericName,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
            if (!organizationAuthorizationService.CanView(organization, customer))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdAsync(customer.Id, organization?.Id, cancellationToken);

        return locations.Select(mapper.MapTo).ToList();
    }

    private async Task<Shared.Models.Location> UpdateInternalAsync(
        Shared.Models.Location location,
        Shared.Database.Entities.Location existingLocation,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !organizationAuthorizationService.CanModify(existingLocation.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var locationRef = location;
        var locationEntityRef = existingLocation;
        var organizationTags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    locationRef.Tags.Select(item => item.Id).Contains(query.Id) &&
                                    query.Organization.Id == locationEntityRef.Organization.Id &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var originalOpeningHours = existingLocation.OpeningHours;

        existingLocation = mapper.MergeTo(location, existingLocation, organizationTags);

        // Restoring original opening hours
        existingLocation.OpeningHours = originalOpeningHours;

        location = mapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return location;
    }

    private async Task<Shared.Models.Location> EnrichLocationAsync(
        Customer? customer,
        Shared.Database.Entities.Location locationEdge,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !organizationAuthorizationService.CanView(locationEdge.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var mappedLocation = mapper.MapTo(locationEdge);

        mappedLocation.CustomTags = mappedLocation.Resources
            .SelectMany(item => item.Tags.Where(tag => tag.Type == OrganizationTagType.Custom).Select(customTag => new Shared.Models.OrganizationTag
            {
                Id = customTag.Id, Name = customTag.Name, Type = OrganizationTagType.Custom, Color = customTag.Color
            }))
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .ToList();

        mappedLocation.Zones = mappedLocation.Resources
            .SelectMany(item => item.Tags.Where(tag => tag.Type == OrganizationTagType.Zone).Select(zone => new Shared.Models.OrganizationTag
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
                CanView = organizationAuthorizationService.CanView(locationEdge.Organization, customer),
                CanModify = organizationAuthorizationService.CanModify(locationEdge.Organization, customer),
                CanDelete = organizationAuthorizationService.CanDelete(locationEdge.Organization, customer),
                CanViewAnalytics = organizationAuthorizationService.CanViewAnalytics(locationEdge.Organization, customer)
            };
        }

        var now = timeProvider.GetUtcNow();
        mappedLocation.HasFutureBooking = await repositoryFactory.BookingRepository
            .Query(new Specification<Booking>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.InvolvedLocations.Select(item => item.Id).Contains(locationEdge.Id) && query.From >= now
            })
            .AnyAsync(cancellationToken);

        return mappedLocation;
    }
}
