using Api.Shared.Services;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Resource;

public class RemoveResourceButtonHandler(
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ILocationPermissionsService locationPermissionsService,
    IEntityMapper entityMapper,
    IPageNavigator pageNavigator,
    ILocationResourceService locationResourceService) : IViewSubmissionHandler
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

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        var context = RemoveResourceContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new UnauthorizedAccessException();
        }

        await locationResourceService.RemoveAsync(workspaceMember.Id, context.ResourceId, cancellationToken);

        await pageNavigator.BackAsync(workspace, workspaceMember, new CommonPageContext(context.PageContext), viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
