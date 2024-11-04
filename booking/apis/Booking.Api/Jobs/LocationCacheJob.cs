using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Jobs;

public class LocationCacheJob(IServiceProvider serviceProvider, ILogger<LocationCacheJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var cachedLocationService = scope.ServiceProvider.GetRequiredService<ICachedLocationService>();
                var locations = await repositoryFactory.LocationRepository
                    .Query(new Specification<Location> { Criteria = query => !query.DeletedAt.HasValue })
                    .ToListAsync(cancellationToken);

                foreach (var location in locations)
                {
                    logger.LogTrace("Caching location by id {id}", location.Id);
                    _ = await cachedLocationService.GetByIdAsync(location.Id, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(LocationCacheJob));
            }
        } while (true);
    }
}
