using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

/// <summary>
///     Resolves the full resource set for event marketplace products.
/// </summary>
public interface IMarketplaceEventResourceService
{
    /// <summary>
    ///     Picks every matching resource for an event product.
    ///     Event products reserve the full matching tagged resource set and do not use customer preferences.
    /// </summary>
    /// <param name="from">The start time of the booking window.</param>
    /// <param name="until">The end time of the booking window.</param>
    /// <param name="productVersion">The event product version being booked.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A collection containing the full matching available resource set.</returns>
    /// <exception cref="NoResourceAvailable">Thrown when the full event resource set cannot be booked.</exception>
    Task<ICollection<Resource>> PickEventResourcesAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        ProductVersion productVersion,
        CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the event marketplace resource service.
/// </summary>
public class MarketplaceEventResourceService(IRepositoryFactory repositoryFactory) : IMarketplaceEventResourceService
{
    public async Task<ICollection<Resource>> PickEventResourcesAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        ProductVersion productVersion,
        CancellationToken cancellationToken)
    {
        if (productVersion.Type != ProductTypeConstants.Event)
        {
            throw new MarketplaceEventResourceSelectionRequiresEventProduct();
        }

        var productTagIds = productVersion.OrganizationTags
            .Where(item => item.Type == OrganizationTagTypeConstants.Product)
            .Select(item => item.Id)
            .ToList();
        if (productTagIds.Count == 0)
        {
            throw new NoResourceAvailable();
        }

        var matchingResources = (await repositoryFactory.LocationRepository.GetAllWithActiveOrganizationAsync(
                false,
                false,
                productTagIds,
                cancellationToken))
            .SelectMany(location => location.Resources)
            .OrderBy(resource => resource.Id)
            .ToList();
        if (matchingResources.Count == 0)
        {
            throw new NoResourceAvailable();
        }

        var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            null,
            from,
            until,
            matchingResources.Select(item => item.Id).ToList(),
            productTagIds,
            [],
            cancellationToken);

        return availableResources.Count < matchingResources.Count
            ? throw new NoResourceAvailable()
            : availableResources.OrderBy(item => item.Id).ToList();
    }
}
