using Api.Shared.Services.Models;
using Enterprise.Shared;
using Organization.Shared.Repositories;

namespace Organization.Jobs.Jobs;

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
                var organizations = await repositoryFactory.OrganizationRepository.GetAllIncludingDeletedAsync(cancellationToken);

                foreach (var organization in organizations)
                {
                    organization.ListingMetadata = new ListingMetadata(organization.About.ToSafeString(), string.Empty, string.Empty);
                    repositoryFactory.OrganizationRepository.Update(organization);
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
