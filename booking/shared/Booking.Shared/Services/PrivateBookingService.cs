using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IPrivateBookingService
{
    Task<(ICollection<Organization>, ICollection<Resource>)> PickResourceBasedOnCustomerPreferencesAsync(
        Models.Booking booking,
        string customerId,
        ICollection<Organization> organizations,
        ICollection<Resource> resources,
        CancellationToken cancellationToken);
}

public class PrivatePrivateBookingService(IRepositoryFactory repositoryFactory) : IPrivateBookingService
{
    public async Task<(ICollection<Organization>, ICollection<Resource>)> PickResourceBasedOnCustomerPreferencesAsync(
        Models.Booking booking,
        string customerId,
        ICollection<Organization> organizations,
        ICollection<Resource> resources,
        CancellationToken cancellationToken)
    {
        if (booking.Resources.Count != 0)
        {
            return (organizations, resources);
        }

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, true, cancellationToken) ?? throw new CustomerNotFound();
        var organization = booking.InvolvedOrganizations.FirstOrDefault();
        var organizationEntity = organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                organization.Id,
                organization.UniqueAlphanumericName,
                false,
                false,
                cancellationToken);

        Location? locationEntity;
        if (organizationEntity is null)
        {
            if (customer.DefaultOrganization is null)
            {
                locationEntity = customer.PreferredLocations.FirstOrDefault();
                if (locationEntity is not null)
                {
                    locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(locationEntity.Id, false, cancellationToken);
                    if (locationEntity is not null)
                    {
                        organizationEntity = locationEntity.Organization;
                    }
                }
            }
            else
            {
                organizationEntity = customer.DefaultOrganization;
                locationEntity = customer.PreferredLocations.FirstOrDefault(item =>
                    item.Organization is not null && item.Organization.Id == customer.DefaultOrganization.Id);
                if (locationEntity is not null)
                {
                    locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(locationEntity.Id, false, cancellationToken);
                }
            }
        }
        else
        {
            locationEntity = customer.PreferredLocations.FirstOrDefault(item =>
                item.Organization is not null && item.Organization.Id == organizationEntity.Id) ?? organizationEntity.Locations.FirstOrDefault();
        }

        if (locationEntity is null)
        {
            return (organizationEntity is null ? [] : [organizationEntity], resources);
        }

        var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            locationEntity.Id,
            booking.From,
            booking.Until,
            [],
            [],
            [OrganizationTagTypeConstants.ResourceDesk],
            cancellationToken);

        var resource = availableResources.FirstOrDefault(item =>
            customer.PreferredResources.Select(preferredResource => preferredResource.Id).Contains(item.Id));
        if (resource is null)
        {
            var preferredZones = customer.PreferredOrganizationTags
                .Where(tag => tag.Type == OrganizationTagTypeConstants.Zone)
                .Select(tag => tag.Id)
                .ToList();
            resource = availableResources.FirstOrDefault(item => item.OrganizationTags.Any(tag => preferredZones.Contains(tag.Id)));
            if (resource is null)
            {
                var preferredTags = customer.PreferredOrganizationTags
                    .Where(tag => tag.Type == OrganizationTagTypeConstants.Custom)
                    .Select(tag => tag.Id)
                    .ToList();
                resource = availableResources.FirstOrDefault(item => item.OrganizationTags.Any(tag => preferredTags.Contains(tag.Id)));
                resources = resource is null ? availableResources.Count != 0 ? [availableResources.First()] : [] : [resource];
            }
            else
            {
                resources = [resource];
            }
        }
        else
        {
            resources = [resource];
        }

        return (organizationEntity is null ? [] : [organizationEntity], resources);
    }
}
