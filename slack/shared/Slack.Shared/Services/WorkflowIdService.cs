using Enterprise.Shared.Temporal;

namespace Slack.Shared.Services;

public interface IWorkflowIdService
{
    string ReSyncSlackWorkspace(string workspaceId);
    string NewSlackWorkspaceJoined(string workspaceId);
}

public class WorkflowIdService(ITemporalHelperService temporalHelperService) : IWorkflowIdService
{
    public string ReSyncSlackWorkspace(string workspaceId) =>
        temporalHelperService.ToId($"{Workflows.Constants.ReSyncSlackWorkspacePrefix}-{workspaceId}");

    public string NewSlackWorkspaceJoined(string workspaceId) =>
        temporalHelperService.ToId($"{Workflows.Constants.NewSlackWorkspaceJoinedPrefix}-{workspaceId}");
}
