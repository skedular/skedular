using Api.Shared.Services;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Zone;

public class RemoveZoneButtonHandler(
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IOrganizationPermissionsService organizationPermissionsService,
    IMapper mapper,
    IPageNavigator pageNavigator,
    IOrganizationZoneService organizationZoneService) : IViewSubmissionHandler
{
    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            viewSubmission.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = RemoveZoneContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new UnauthorizedAccessException();
        }

        await organizationZoneService.RemoveAsync(workspaceMember.Id, context.ZoneId, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash,
            cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
