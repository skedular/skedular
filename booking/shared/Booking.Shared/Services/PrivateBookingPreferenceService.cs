using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;

namespace Booking.Shared.Services;

public interface IPrivateBookingPreferenceService
{
    Task<(ICollection<Organization>, ICollection<Resource>)> PickResourceBasedOnCustomerPreferencesAsync(
        string customerId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> involvedOrganizationUniqueAlphanumericNames,
        ICollection<Organization> organizations,
        ICollection<Resource> resources,
        CancellationToken cancellationToken);
}

public class PrivateBookingPreferenceService(IRepositoryFactory repositoryFactory) : IPrivateBookingPreferenceService
{
    public async Task<(ICollection<Organization>, ICollection<Resource>)> PickResourceBasedOnCustomerPreferencesAsync(
        string customerId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> involvedOrganizationUniqueAlphanumericNames,
        ICollection<Organization> organizations,
        ICollection<Resource> resources,
        CancellationToken cancellationToken)
    {
        if (resources.Count != 0)
        {
            return (organizations, resources);
        }

        var customer = await GetCustomerAsync(customerId, cancellationToken);
        var (organization, location) =
            await ResolveOrganizationAndLocationAsync(involvedOrganizationUniqueAlphanumericNames, customer, cancellationToken);
        if (location is null)
        {
            return (ToOrganizations(organization), resources);
        }

        var availableResources = await GetAvailableDeskResourcesAsync(from, until, location.Id, cancellationToken);
        resources = SelectResourcesByCustomerPreferences(customer, availableResources);

        return (ToOrganizations(organization), resources);
    }

    private async Task<CustomerEntity> GetCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, true, cancellationToken) ?? throw new CustomerNotFound();

    private async Task<(Organization?, Location?)> ResolveOrganizationAndLocationAsync(
        ICollection<string> involvedOrganizationUniqueAlphanumericNames,
        CustomerEntity customer,
        CancellationToken cancellationToken)
    {
        var organizationEntity = await ResolveBookingOrganizationAsync(involvedOrganizationUniqueAlphanumericNames, cancellationToken);
        if (organizationEntity is null)
        {
            return await ResolveWithoutBookingOrganizationAsync(customer, cancellationToken);
        }

        var locationEntity = customer.PreferredLocations.FirstOrDefault(item =>
                                 item.Organization is not null && item.Organization.Id == organizationEntity.Id) ??
                             organizationEntity.Locations.FirstOrDefault();

        return (organizationEntity, locationEntity);
    }

    private async Task<Organization?> ResolveBookingOrganizationAsync(
        ICollection<string> involvedOrganizationUniqueAlphanumericNames,
        CancellationToken cancellationToken)
    {
        if (involvedOrganizationUniqueAlphanumericNames.Count == 0)
        {
            return null;
        }

        return await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
            null,
            involvedOrganizationUniqueAlphanumericNames.First(),
            false,
            false,
            cancellationToken);
    }

    private async Task<(Organization?, Location? )> ResolveWithoutBookingOrganizationAsync(
        CustomerEntity customer,
        CancellationToken cancellationToken)
    {
        Location? locationEntity;
        if (customer.DefaultOrganization is null)
        {
            locationEntity = customer.PreferredLocations.FirstOrDefault();
            if (locationEntity is null)
            {
                return (null, null);
            }

            locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(locationEntity.Id, false, cancellationToken);
            return (locationEntity?.Organization, locationEntity);
        }

        var location = customer.PreferredLocations.FirstOrDefault(item =>
            item.Organization is not null && item.Organization.Id == customer.DefaultOrganization.Id);
        locationEntity = location is null
            ? null
            : await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, false, cancellationToken);

        return (customer.DefaultOrganization, locationEntity);
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
        CustomerEntity customer,
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

    private static Resource? FindByPreferredResource(CustomerEntity customer, IEnumerable<Resource> availableResources)
    {
        var preferredResourceIds = customer.PreferredResources.Select(item => item.Id).ToHashSet();
        return availableResources.FirstOrDefault(item => preferredResourceIds.Contains(item.Id));
    }

    private static Resource? FindByPreferredTagType(CustomerEntity customer, ICollection<Resource> availableResources, string tagType)
    {
        var preferredTagIds = customer.PreferredOrganizationTags
            .Where(tag => tag.Type == tagType)
            .Select(tag => tag.Id)
            .ToHashSet();

        return availableResources.FirstOrDefault(item => item.OrganizationTags.Any(tag => preferredTagIds.Contains(tag.Id)));
    }

    private static ICollection<Organization> ToOrganizations(Organization? organizationEntity) =>
        organizationEntity is null ? [] : [organizationEntity];
}
