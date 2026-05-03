using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

/// <summary>
///     Service for managing resource availability and validation.
/// </summary>
public interface IResourceService
{
    /// <summary>
    ///     Retrieves resource entities and validates their availability for the specified time range.
    /// </summary>
    /// <param name="from">The start date and time of the booking period.</param>
    /// <param name="until">The end date and time of the booking period.</param>
    /// <param name="resourceIds">The IDs of the resources to check.</param>
    /// <param name="tagIds">The IDs of the tags to filter resources by.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of available resources.</returns>
    /// <exception cref="ResourceNotAvailable">Thrown when not all requested resources are available.</exception>
    Task<IReadOnlyList<Resource>> GetResourceEntitiesAndValidateAvailabilityAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the resource service.
/// </summary>
public class ResourceService(IRepositoryFactory repositoryFactory) : IResourceService
{
    /// <summary>
    ///     Retrieves resource entities and validates their availability for the specified time range.
    /// </summary>
    /// <param name="from">The start date and time of the booking period.</param>
    /// <param name="until">The end date and time of the booking period.</param>
    /// <param name="resourceIds">The IDs of the resources to check.</param>
    /// <param name="tagIds">The IDs of the tags to filter resources by.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of available resources.</returns>
    /// <exception cref="ResourceNotAvailable">Thrown when not all requested resources are available.</exception>
    public async Task<IReadOnlyList<Resource>> GetResourceEntitiesAndValidateAvailabilityAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        CancellationToken cancellationToken)
    {
        if (resourceIds.Count == 0)
        {
            return [];
        }

        var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            null,
            from,
            until,
            resourceIds,
            tagIds,
            [],
            cancellationToken);

        return availableResources.Count != resourceIds.Count || !availableResources.All(item => resourceIds.Contains(item.Id))
            ? throw new ResourceNotAvailable()
            : availableResources;
    }
}
