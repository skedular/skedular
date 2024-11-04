using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Jobs;

public class OrganizationCacheJob(
    IServiceProvider serviceProvider,
    ILogger<OrganizationCacheJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var cachedOrganizationService = scope.ServiceProvider.GetRequiredService<ICachedOrganizationService>();
                var organizations = await repositoryFactory.OrganizationRepository
                    .Query(new Specification<Organization> { Criteria = query => !query.DeletedAt.HasValue })
                    .ToListAsync(cancellationToken);

                foreach (var organization in organizations)
                {
                    logger.LogTrace("Caching organization by id {id}", organization.Id);
                    _ = await cachedOrganizationService.GetByIdAsync(organization.Id, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(OrganizationCacheJob));
            }
        } while (true);
    }
}
