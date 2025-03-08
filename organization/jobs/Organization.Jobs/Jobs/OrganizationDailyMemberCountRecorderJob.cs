using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Jobs.Jobs;

public class OrganizationDailyMemberCountRecorderJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<OrganizationDailyMemberCountRecorderJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(OrganizationDailyMemberCountRecorderJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var organizationInternalPublisher = scope.ServiceProvider.GetRequiredService<IOrganizationInternalPublisher>();
                var yesterday = timeProvider.GetUtcNow().EndOfYesterday();
                var organizationIds = await repositoryFactory.OrganizationRepository.Query(
                    new Specification<Shared.Database.Entities.Organization>
                    {
                        Criteria = query =>
                            query.OrganizationOfferings.Any(organizationOffering =>
                                !organizationOffering.DeletedAt.HasValue &&
                                (!query.DailyMemberCountLastRecordedAt.HasValue ||
                                 query.DailyMemberCountLastRecordedAt < yesterday))
                    }).Select(query => query.Id).ToListAsync(cancellationToken);
                if (organizationIds.Count != 0)
                {
                    await organizationInternalPublisher.PublishRecordOrganizationDailyMemberCountAsync(organizationIds, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
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
