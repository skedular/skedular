using Api.Shared.Services.Models;
using Enterprise.Shared;
using Location.Shared.Repositories;

namespace Location.Jobs.Jobs;

public class MigrateListingMetadata(IServiceProvider serviceProvider, ILogger<MigrateListingMetadata> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(MigrateListingMetadata).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var locations = await repositoryFactory.LocationRepository.GetAllIncludingDeletedAsync(cancellationToken);

                foreach (var location in locations)
                {
                    location.ListingMetadata = new ListingMetadata(location.About.ToSafeString(), string.Empty, string.Empty);
                    repositoryFactory.LocationRepository.Update(location);
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", _jobName);
            }
        } while (true);
    }
}
