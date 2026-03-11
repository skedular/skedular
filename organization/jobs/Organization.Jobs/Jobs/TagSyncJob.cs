using Organization.Shared.Repositories;
using Organization.Shared.Services;

namespace Organization.Jobs.Jobs;

public class TagSyncJob(IServiceProvider serviceProvider, TimeProvider timeProvider, ILogger<TagSyncJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(TagSyncJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var organizationDefaultValuesProvider = scope.ServiceProvider.GetRequiredService<IOrganizationDefaultValuesProvider>();
                var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);

                foreach (var organization in organizations)
                {
                    var tags = organizationDefaultValuesProvider.GetDefaultTags(organization);
                    foreach (var tag in tags)
                    {
                        if (organization.Tags.All(item => item.Type != tag.Type))
                        {
                            repositoryFactory.TagRepository.Add(tag);
                        }
                    }

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
