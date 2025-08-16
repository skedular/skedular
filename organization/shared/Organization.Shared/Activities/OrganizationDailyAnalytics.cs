using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public class OrganizationDailyAnalytics(
    IRepositoryFactory repositoryFactory,
    ITemporalService temporalService,
    TimeProvider timeProvider,
    IRandomHelper randomHelper)
{
    [Activity]
    public async Task<bool> RecordOrganizationMembersCountAsync(string organizationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null || organization.IsDeleted())
        {
            return false;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        _ = repositoryFactory.DailyMemberCountRecordingRepository.Add(new DailyMemberCountRecording
        {
            Id = randomHelper.Generate(),
            Count = organization.OrganizationMembers.Count(item => item.IsNotDeleted()),
            Date = startOfToday,
            Organization = organization
        });

        _ = repositoryFactory.OrganizationRepository.Update(organization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    [Activity]
    public async Task ExecuteNextGenerateOrganizationDailyAnalyticsWorkflowAsync(string organizationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        await temporalService.StartWorkflowGenerateOrganizationDailyAnalyticsAsync(
            new GenerateOrganizationDailyAnalyticsInput(organizationId, timeProvider.GetUtcNow().AddDays(1)),
            cancellationToken);
    }
}
