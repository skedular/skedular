using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Location.Shared.Workflows;
using Constants = Api.Shared.Services.Constants;
using Customer = Location.Shared.Database.Entities.Customer;
using Organization = Location.Shared.Database.Entities.Organization;

namespace Location.Api.Services;

public interface ILocationService
{
    Task<Shared.Models.Location> AddAsync(Shared.Models.Location location, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Shared.Models.Location> UpdateAsync(Shared.Models.Location location, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Shared.Models.Location> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Location?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);

    Task<IReadOnlyList<Shared.Models.Location>> GetMyLocationsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Shared.Models.Location>>, int)> GetPaginatedLocationsAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        IReadOnlyList<LocationOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class LocationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IMapper mapper,
    TimeProvider timeProvider,
    IContext context,
    ICachedLocationService cachedLocationService) : ILocationService
{
    public async Task<Shared.Models.Location> AddAsync(
        Shared.Models.Location location,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(location.Organization);

        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);

        Organization organization;
        if (!string.IsNullOrWhiteSpace(location.Organization.Id))
        {
            organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization.Id, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(location.Organization.CustomDomain))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                location.Organization.Id,
                location.Organization.CustomDomain,
                false,
                false,
                cancellationToken) ?? throw new OrganizationNotFound();
        }
        else
        {
            throw new InvalidOperationException("Either id or customDomain must be provided.");
        }

        if (!ignoreAuthorizationCheck)
        {
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            if (!await organizationAuthorizationService.CanModifyAsync(organization.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            if (!await organizationOfferingService.CanCreateLocationAsync(organization.Id, cancellationToken) ||
                !await organizationOfferingService.IsMoreInteractionAllowedAsync(organization.Id, customer.Id, cancellationToken))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        if (string.IsNullOrWhiteSpace(location.Id))
        {
            location.Id = randomHelper.Generate();
        }
        else
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

        if (location.ExtraMetadata is not null)
        {
            location.ExtraMetadata = location.ExtraMetadata with
            {
                RelatedImageLinks = location.ExtraMetadata.RelatedImageLinks?.Where(item => !string.IsNullOrWhiteSpace(item)).ToList(),
                RelatedVideoLinks = location.ExtraMetadata.RelatedVideoLinks?.Where(item => !string.IsNullOrWhiteSpace(item)).ToList(),
                OtherLinks = location.ExtraMetadata.OtherLinks?.Where(item => !string.IsNullOrWhiteSpace(item)).ToList()
            };
        }

        var organizationTags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            location.OrganizationTags.Select(item => item.Id).ToList(),
            location.Organization.Id,
            location.Organization.CustomDomain,
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var locationEntity = mapper.MapTo(location, organization, organizationTags);

        if (organization.CustomDomain == Constants.SkedularPublicLocationsCustomDomainName &&
            string.IsNullOrWhiteSpace(location.UniqueClaimCode))
        {
            locationEntity.UniqueClaimCode = randomHelper.GenerateAlphanumericNumeric(10).ToUpperInvariant();
        }

        locationEntity.OpeningHours = location.OpeningHours ?? OpeningHours.Default;
        locationEntity = repositoryFactory.LocationRepository.Add(locationEntity);

        if (location.PhysicalAddress is not null)
        {
            location.PhysicalAddress.Id = randomHelper.Generate();
            var locationPhysicalAddressEntity = mapper.MapTo(location.PhysicalAddress, locationEntity);
            repositoryFactory.LocationPhysicalAddressRepository.Add(locationPhysicalAddressEntity);
        }

        location = mapper.MapTo(locationEntity);

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartWorkflowLocationDailyAnalytics(
            new GenerateLocationDailyAnalyticsInput(location.Id, timeProvider.GetUtcNow().AddDays(1), null),
            repositoryFactory.UnitOfWork);

        temporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(location.Organization.Id),
            repositoryFactory.UnitOfWork);

        if (organization.CustomDomain != Constants.SkedularPublicLocationsCustomDomainName)
        {
            temporalOutboxService.StartWorkflowNewLocationJoined(new NewLocationJoinedInput(location.Id), repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(location.Id, cancellationToken);

        return location;
    }

    public async Task<Shared.Models.Location> UpdateAsync(
        Shared.Models.Location location,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(location.Id);

        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken) ??
                               throw new LocationNotFound();

        if (!ignoreAuthorizationCheck)
        {
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customer.Id, cancellationToken))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        return await UpdateInternalAsync(location, existingLocation, customer, cancellationToken);
    }

    public async Task<Shared.Models.Location> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(id, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanDeleteAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedLocation = mapper.MapTo(repositoryFactory.LocationRepository.Remove(existingLocation));

        locationOutboxPublisher.PublishLocations([deletedLocation], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(existingLocation.Organization.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.RemoveByIdAsync(existingLocation.Id, cancellationToken);

        return deletedLocation;
    }

    public async Task<Shared.Models.Location?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var location = await cachedLocationService.GetByIdAsync(id, cancellationToken);
        if (location is null)
        {
            return null;
        }

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            var verifiableToken = context.GetVerifiableToken();
            if (!string.IsNullOrWhiteSpace(verifiableToken) || location.Type.ToLocationType() != LocationType.Marketplace)
            {
                customer = await cachedCustomerService.GetAsync(cancellationToken);
            }
        }

        return await EnrichLocationAsync(customer, location, cancellationToken);
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Shared.Models.Location>>, int)> GetPaginatedLocationsAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        IReadOnlyList<LocationOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            customer = await cachedCustomerService.GetAsync(cancellationToken);
            if (searchCriteria.Types.Count != 1 || searchCriteria.Types.First() != LocationType.Marketplace)
            {
                // Ensure we do not return another customer location by forcing CustomerId as search criteria
                searchCriteria = searchCriteria with { CustomerId = customer.Id };
            }
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.LocationRepository.GetPaginatedLocationsUntrackedAsync(
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

    public async Task<IReadOnlyList<Shared.Models.Location>> GetMyLocationsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);

        Organization? organization = null;
        if (!string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationCustomDomain,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
            if (!await organizationAuthorizationService.CanViewAsync(organization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdUntrackedAsync(customerId, organization?.Id, cancellationToken);

        return locations.Select(mapper.MapTo).ToList();
    }

    private async Task<Shared.Models.Location> UpdateInternalAsync(
        Shared.Models.Location location,
        Shared.Database.Entities.Location existingLocation,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        if (customer is not null &&
            !await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (location.ExtraMetadata is not null)
        {
            location.ExtraMetadata = location.ExtraMetadata with
            {
                RelatedImageLinks = location.ExtraMetadata.RelatedImageLinks?.Where(item => !string.IsNullOrWhiteSpace(item)).ToList(),
                RelatedVideoLinks = location.ExtraMetadata.RelatedVideoLinks?.Where(item => !string.IsNullOrWhiteSpace(item)).ToList(),
                OtherLinks = location.ExtraMetadata.OtherLinks?.Where(item => !string.IsNullOrWhiteSpace(item)).ToList()
            };
        }

        var organizationTags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            location.OrganizationTags.Select(item => item.Id).ToList(),
            existingLocation.Organization.Id,
            existingLocation.Organization.CustomDomain,
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var originalOpeningHours = existingLocation.OpeningHours;
        var uniqueClaimCode = existingLocation.UniqueClaimCode;
        var contactedViaEmail = existingLocation.ContactedViaEmail;
        var contactedViaSms = existingLocation.ContactedViaSms;
        var contactedViaCall = existingLocation.ContactedViaCall;
        var contactedViaWhatsapp = existingLocation.ContactedViaWhatsapp;

        existingLocation = mapper.MergeTo(location, existingLocation, organizationTags);

        if (string.IsNullOrWhiteSpace(location.UniqueClaimCode))
        {
            existingLocation.UniqueClaimCode = uniqueClaimCode;
        }

        existingLocation.ContactedViaEmail = contactedViaEmail;
        existingLocation.ContactedViaSms = contactedViaSms;
        existingLocation.ContactedViaCall = contactedViaCall;
        existingLocation.ContactedViaWhatsapp = contactedViaWhatsapp;

        // Restoring original opening hours
        existingLocation.OpeningHours = originalOpeningHours;

        if (location.PhysicalAddress is not null)
        {
            if (existingLocation.PhysicalAddress is null)
            {
                location.PhysicalAddress.Id = randomHelper.Generate();
                var locationPhysicalAddressEntity = mapper.MapTo(location.PhysicalAddress, existingLocation);
                repositoryFactory.LocationPhysicalAddressRepository.Add(locationPhysicalAddressEntity);
            }
            else
            {
                location.PhysicalAddress.Id = existingLocation.PhysicalAddress.Id;
                var locationPhysicalAddressEntity = mapper.MergeTo(location.PhysicalAddress, existingLocation.PhysicalAddress, existingLocation);
                repositoryFactory.LocationPhysicalAddressRepository.Update(locationPhysicalAddressEntity);
            }
        }

        location = mapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(existingLocation.Organization.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(location.Id, cancellationToken);

        return location;
    }

    private async Task<Shared.Models.Location> EnrichLocationAsync(
        Customer? customer,
        Shared.Database.Entities.Location location,
        CancellationToken cancellationToken)
    {
        var isMarketplace = location.Type.ToLocationType() == LocationType.Marketplace;
        if (!isMarketplace && customer is not null &&
            !await organizationAuthorizationService.CanViewAsync(location.OrganizationId, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var mappedLocation = mapper.MapTo(location);

        if (customer is null || location.Organization.CustomDomain == Constants.SkedularPublicLocationsCustomDomainName)
        {
            return mappedLocation;
        }

        mappedLocation.Permissions = new Permissions
        {
            CanView =
                isMarketplace || await organizationAuthorizationService.CanViewAsync(location.OrganizationId, customer.Id, cancellationToken),
            CanModify = await organizationAuthorizationService.CanModifyAsync(location.OrganizationId, customer.Id, cancellationToken),
            CanDelete = await organizationAuthorizationService.CanDeleteAsync(location.OrganizationId, customer.Id, cancellationToken),
            CanViewAnalytics =
                await organizationAuthorizationService.CanViewAnalyticsAsync(location.OrganizationId, customer.Id, cancellationToken)
        };

        if (!mappedLocation.Permissions.CanModify)
        {
            mappedLocation.UniqueClaimCode = null;
        }

        if (mappedLocation.ExtraMetadata?.OtherLinks is not null && !customer.Identities.Any(item =>
                !string.IsNullOrWhiteSpace(item.Email) &&
                (item.Email.Contains("morteza.alizadeh@gmail.com", StringComparison.InvariantCultureIgnoreCase) ||
                 item.Email.Contains("leila.alavi78@gmail.com", StringComparison.InvariantCultureIgnoreCase))))
        {
            mappedLocation.ExtraMetadata = mappedLocation.ExtraMetadata with
            {
                OtherLinks = mappedLocation.ExtraMetadata.OtherLinks.Where(item =>
                    item.Contains("sharedspace.co.nz", StringComparison.InvariantCultureIgnoreCase)).ToList()
            };
        }

        return mappedLocation;
    }
}
