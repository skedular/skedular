using Enterprise.Shared.Temporal;
using MsTeams.Shared.Workflows;

namespace MsTeams.Shared.Services;

public interface IWorkflowIdService
{
    string ReSyncMsTeams(string tenantId);
}

public class WorkflowIdService(ITemporalHelperService temporalHelperService) : IWorkflowIdService
{
    public string ReSyncMsTeams(string tenantId) =>
        temporalHelperService.ToId($"{Constants.ReSyncMsTeamsPrefix}-{tenantId}");
}
