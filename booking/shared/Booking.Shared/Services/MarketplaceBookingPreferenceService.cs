using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

/// <summary>
///     Service for selecting marketplace booking resources based on customer preferences.
///     Handles the logic for picking the most appropriate resources for a booking request.
/// </summary>
public interface IMarketplaceBookingPreferenceService
{
    /// <summary>
    ///     Picks resources for a marketplace booking based on customer preferences.
    ///     Prioritizes resources in order: customer preferred resources, preferred locations,
    ///     preferred zone tags, preferred custom tags, then any available resources.
    /// </summary>
    /// <param name="customer">The customer making the booking, used for preferences.</param>
    /// <param name="from">The start time of the booking window.</param>
    /// <param name="until">The end time of the booking window.</param>
    /// <param name="productVersion">The product version being booked.</param>
    /// <param name="numberOfResourcesToBook">The number of resources required.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A collection of selected resources.</returns>
    /// <exception cref="NoResourceAvailable">Thrown when insufficient resources are available.</exception>
    Task<ICollection<Resource>> PickResourceBasedOnCustomerPreferencesAsync(
        Customer? customer,
        DateTimeOffset from,
        DateTimeOffset until,
        ProductVersion productVersion,
        int numberOfResourcesToBook,
        CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the marketplace booking preference service.
/// </summary>
public class MarketplaceBookingPreferenceService(IRepositoryFactory repositoryFactory) : IMarketplaceBookingPreferenceService
{
    /// <summary>
    ///     Picks resources for a marketplace booking based on customer preferences.
    ///     Prioritizes resources in order: customer preferred resources, preferred locations,
    ///     preferred zone tags, preferred custom tags, then any available resources.
    /// </summary>
    /// <param name="customer">The customer making the booking, used for preferences.</param>
    /// <param name="from">The start time of the booking window.</param>
    /// <param name="until">The end time of the booking window.</param>
    /// <param name="productVersion">The product version being booked.</param>
    /// <param name="numberOfResourcesToBook">The number of resources required.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A collection of selected resources.</returns>
    /// <exception cref="NoResourceAvailable">Thrown when insufficient resources are available.</exception>
    public async Task<ICollection<Resource>> PickResourceBasedOnCustomerPreferencesAsync(
        Customer? customer,
        DateTimeOffset from,
        DateTimeOffset until,
        ProductVersion productVersion,
        int numberOfResourcesToBook,
        CancellationToken cancellationToken)
    {
        var availableResources = await GetAvailableResourcesAsync(from, until, productVersion.OrganizationTags, cancellationToken);
        if (availableResources.Count < numberOfResourcesToBook)
        {
            throw new NoResourceAvailable();
        }

        if (customer is null)
        {
            return availableResources.Take(numberOfResourcesToBook).ToList();
        }

        var customerPreferredResourceIds = customer.PreferredResources.Select(item => item.Id).ToList();
        var customerPreferredLocationIds = customer.PreferredLocations.Select(item => item.Id).ToList();
        var customerPreferredZoneTagIds = customer.PreferredOrganizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type) && item.Type.ToOrganizationTagType() == OrganizationTagType.Zone)
            .Select(item => item.Id).ToList();
        var customerPreferredCustomTagIds = customer.PreferredOrganizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type) && item.Type.ToOrganizationTagType() == OrganizationTagType.Custom)
            .Select(item => item.Id).ToList();

        var resources = new List<Resource>();
        foreach (var resource in availableResources.Where(item => customerPreferredResourceIds.Any(resourceId => resourceId == item.Id)))
        {
            resources.Add(resource);
            if (resources.Count == numberOfResourcesToBook)
            {
                return resources;
            }
        }

        foreach (var resource in availableResources.Where(item =>
                     item.Location is not null && customerPreferredLocationIds.Any(locationId => locationId == item.Location.Id)))
        {
            resources.Add(resource);
            if (resources.Count == numberOfResourcesToBook)
            {
                return resources;
            }
        }

        foreach (var resource in availableResources.Where(item =>
                     customerPreferredZoneTagIds.Any(tagId => item.OrganizationTags.Any(tag => tagId == tag.Id))))
        {
            resources.Add(resource);
            if (resources.Count == numberOfResourcesToBook)
            {
                return resources;
            }
        }

        foreach (var resource in availableResources.Where(item =>
                     customerPreferredCustomTagIds.Any(tagId => item.OrganizationTags.Any(tag => tagId == tag.Id))))
        {
            resources.Add(resource);
            if (resources.Count == numberOfResourcesToBook)
            {
                return resources;
            }
        }

        var selectedResourceIds = resources.Select(item => item.Id).ToList();
        var unselectedResources = availableResources.Where(item => !selectedResourceIds.Contains(item.Id)).ToList();

        return resources.Concat(unselectedResources.Take(numberOfResourcesToBook - resources.Count)).ToList();
    }

    /// <summary>
    ///     Gets available resources for the specified time window and organization tags.
    ///     Filters resources by product tags and availability.
    /// </summary>
    /// <param name="from">The start time of the booking window.</param>
    /// <param name="until">The end time of the booking window.</param>
    /// <param name="organizationTags">The organization tags to filter resources by.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A collection of available resources.</returns>
    private async Task<ICollection<Resource>> GetAvailableResourcesAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<OrganizationTag> organizationTags,
        CancellationToken cancellationToken) =>
        await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            null,
            from,
            until,
            [],
            organizationTags.Where(item => item.Type == OrganizationTagTypeConstants.Product).Select(item => item.Id).ToList(),
            [],
            cancellationToken);
}
