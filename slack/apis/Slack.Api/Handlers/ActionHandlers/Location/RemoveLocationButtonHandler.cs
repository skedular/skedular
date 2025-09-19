using Api.Shared.Services;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Location;

public class RemoveLocationButtonHandler(
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ILocationPermissionsService locationPermissionsService,
    IMapper mapper,
    IPageNavigator pageNavigator,
    ILocationService locationService) : IViewSubmissionHandler
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
        var context = RemoveLocationContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        if (!permissions.CanDelete)
        {
            throw new UnauthorizedAccessException();
        }

        await locationService.RemoveAsync(workspaceMember.Id, context.LocationId, cancellationToken);

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
