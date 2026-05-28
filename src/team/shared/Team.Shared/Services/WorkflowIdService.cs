using Enterprise.Shared.Temporal;

namespace Team.Shared.Services;

public interface IWorkflowIdService
{
    string InviteToJoin(string joinInvitationId);
}

public class WorkflowIdService(ITemporalHelperService temporalHelperService) : IWorkflowIdService
{
    public string InviteToJoin(string joinInvitationId) =>
        temporalHelperService.ToId(joinInvitationId);
}
