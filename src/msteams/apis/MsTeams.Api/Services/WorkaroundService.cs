using Enterprise.Shared.Database;
using MsTeams.Shared.Repositories;
using MsTeams.Shared.Services;
using MsTeams.Shared.Workflows;

namespace MsTeams.Api.Services;

public interface IWorkaroundService
{
    Task ReSyncMsTeamsAsync(string tenantId, CancellationToken cancellationToken);
    Task ReSyncAllMsTeamsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, ITemporalService temporalService) : IWorkaroundService
{
    public async Task ReSyncMsTeamsAsync(string tenantId, CancellationToken cancellationToken)
    {
        var tenant = await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null || tenant.IsReplicatedDeleted())
        {
            return;
        }

        await temporalService.StartWorkflowReSyncMsTeamsAsync(new ReSyncMsTeamsInput(tenant.Id, null), cancellationToken);
    }

    public async Task ReSyncAllMsTeamsAsync(CancellationToken cancellationToken)
    {
        var tenants = await repositoryFactory.AzureTenantRepository.GetAllAsync(cancellationToken);

        foreach (var tenant in tenants)
        {
            await temporalService.StartWorkflowReSyncMsTeamsAsync(new ReSyncMsTeamsInput(tenant.Id, null), cancellationToken);
        }
    }
}
