using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Processors.Jobs;

public class OrganizationOfferingRenewalJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<OrganizationOfferingRenewalJob> logger,
    ITimeHelper timeHelper) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await timeHelper.RandomSleepWhileStartingUpAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory =
                    scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var organizationInternalPublisher =
                    scope.ServiceProvider.GetRequiredService<IOrganizationInternalPublisher>();

                var now = timeProvider.GetUtcNow();
                var organizationIds = await repositoryFactory.OrganizationRepository.Query(
                    new Specification<Shared.Database.Entities.Organization>
                    {
                        Criteria = query =>
                            query.OrganizationOfferings.Any(organizationOffering =>
                                !organizationOffering.DeletedAt.HasValue && organizationOffering.End <= now &&
                                organizationOffering.AutoRenew)
                    }).Select(query => query.Id).ToListAsync(cancellationToken);
                if (organizationIds.Count != 0)
                {
                    await organizationInternalPublisher.PublishOrganizationsRequireOfferingAutoRenewAsync(
                        organizationIds,
                        cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(OrganizationOfferingRenewalJob));
            }
        } while (true);
    }
}
