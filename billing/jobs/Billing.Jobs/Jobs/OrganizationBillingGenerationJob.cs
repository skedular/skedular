using Billing.Shared.Database.Entities;
using Billing.Shared.Publishers;
using Billing.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;

namespace Billing.Jobs.Jobs;

public class OrganizationBillingGenerationJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<OrganizationBillingGenerationJob> logger,
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
                var billingInternalPublisher =
                    scope.ServiceProvider.GetRequiredService<IBillingInternalPublisher>();

                var now = timeProvider.GetUtcNow();
                var organizationOfferingIds = await repositoryFactory.OrganizationOfferingRepository.Query(
                    new Specification<OrganizationOffering>
                    {
                        Criteria = query => query.End <= now && !query.InvoiceDate.HasValue
                    }).Select(organization => organization.Id).ToListAsync(cancellationToken);
                if (organizationOfferingIds.Count != 0)
                {
                    await billingInternalPublisher.PublishOrganizationOfferingRequireBillingAsync(
                        organizationOfferingIds,
                        cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(OrganizationBillingGenerationJob));
            }
        } while (true);
    }
}
