using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IPrivateBookingPreferenceService
{
    Task<(ICollection<Organization>, ICollection<Resource>)> PickResourceBasedOnCustomerPreferencesAsync(
        Customer customer,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> involvedOrganizationIds,
        ICollection<string> involvedOrganizationCustomDomains,
        CancellationToken cancellationToken);
}

public class PrivateBookingPreferenceService(IRepositoryFactory repositoryFactory) : IPrivateBookingPreferenceService
{
    public async Task<(ICollection<Organization>, ICollection<Resource>)> PickResourceBasedOnCustomerPreferencesAsync(
        Customer customer,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> involvedOrganizationIds,
        ICollection<string> involvedOrganizationCustomDomains,
        CancellationToken cancellationToken)
    {
        var (organization, locations) = await ResolveOrganizationAndLocationAsync(
            involvedOrganizationIds.FirstOrDefault(),
            involvedOrganizationCustomDomains.FirstOrDefault(),
            customer,
            cancellationToken);
        if (locations.Count == 0)
        {
            return (ToOrganizations(organization), []);
        }

        foreach (var location in locations)
        {
            var availableResources = await GetAvailableDeskResourcesAsync(from, until, location.Id, cancellationToken);
            var resources = SelectResourcesByCustomerPreferences(customer, availableResources);
            if (resources.Count != 0)
            {
                return (ToOrganizations(organization), resources);
            }
        }

        return (ToOrganizations(organization), []);
    }

    private async Task<(Organization?, List<Location>)> ResolveOrganizationAndLocationAsync(
        string? organizationId,
        string? organizationCustomDomain,
        Customer customer,
        CancellationToken cancellationToken)
    {
        var organizationEntity = await ResolveBookingOrganizationAsync(organizationId, organizationCustomDomain, cancellationToken);
        if (organizationEntity is null)
        {
            return await ResolveWithoutBookingOrganizationAsync(customer, cancellationToken);
        }

        return (organizationEntity, OrderLocationsByCustomerPreference(
            organizationEntity.Locations.Where(item => !item.DeletedAt.HasValue),
            customer,
            organizationEntity.Id));
    }

    private async Task<Organization?> ResolveBookingOrganizationAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken) =>
        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            organizationId,
            organizationCustomDomain,
            false,
            false,
            cancellationToken);

    private async Task<(Organization?, List<Location>)> ResolveWithoutBookingOrganizationAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        if (customer.DefaultOrganization is null)
        {
            var locationEntity = customer.PreferredLocations.FirstOrDefault();
            if (locationEntity is null)
            {
                return (null, []);
            }

            locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(locationEntity.Id, false, cancellationToken);
            return locationEntity is null ? (null, []) : (locationEntity.Organization, [locationEntity]);
        }

        var organizationEntity = await ResolveBookingOrganizationAsync(customer.DefaultOrganization.Id, null, cancellationToken);
        if (organizationEntity is null)
        {
            return (null, []);
        }

        return (organizationEntity, OrderLocationsByCustomerPreference(
            organizationEntity.Locations.Where(item => !item.DeletedAt.HasValue),
            customer,
            organizationEntity.Id));
    }

    private async Task<ICollection<Resource>> GetAvailableDeskResourcesAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        string locationId,
        CancellationToken cancellationToken) =>
        await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            locationId,
            from,
            until,
            [],
            [],
            [OrganizationTagTypeConstants.ResourceDesk],
            cancellationToken);

    private static ICollection<Resource> SelectResourcesByCustomerPreferences(
        Customer customer,
        ICollection<Resource> availableResources)
    {
        var selectedResource = FindByPreferredResource(customer, availableResources) ??
                               FindByPreferredTagType(customer, availableResources, OrganizationTagTypeConstants.Zone) ??
                               FindByPreferredTagType(customer, availableResources, OrganizationTagTypeConstants.Custom);

        if (selectedResource is not null)
        {
            return [selectedResource];
        }

        return availableResources.Count != 0 ? [availableResources.First()] : [];
    }

    private static Resource? FindByPreferredResource(Customer customer, IEnumerable<Resource> availableResources)
    {
        var preferredResourceIds = customer.PreferredResources.Select(item => item.Id).ToHashSet();
        return availableResources.FirstOrDefault(item => preferredResourceIds.Contains(item.Id));
    }

    private static Resource? FindByPreferredTagType(Customer customer, ICollection<Resource> availableResources, string tagType)
    {
        var preferredTagIds = customer.PreferredOrganizationTags
            .Where(tag => tag.Type == tagType)
            .Select(tag => tag.Id)
            .ToHashSet();

        return availableResources.FirstOrDefault(item => item.OrganizationTags.Any(tag => preferredTagIds.Contains(tag.Id)));
    }

    private static List<Location> OrderLocationsByCustomerPreference(
        IEnumerable<Location> locations,
        Customer customer,
        string organizationId)
    {
        var preferredLocationIds = customer.PreferredLocations
            .Where(item => item.Organization is not null && item.Organization.Id == organizationId)
            .Select(item => item.Id)
            .ToHashSet();

        return locations
            .OrderByDescending(item => preferredLocationIds.Contains(item.Id))
            .ThenBy(item => item.Name)
            .ToList();
    }

    private static ICollection<Organization> ToOrganizations(Organization? organizationEntity) =>
        organizationEntity is null ? [] : [organizationEntity];
}
