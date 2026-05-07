using Api.Shared.Services;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Team;

public class RemoveTeamButtonHandler(
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ITeamPermissionsService teamPermissionsService,
    IEntityMapper entityMapper,
    IPageNavigator pageNavigator,
    ITeamService teamService) : IViewSubmissionHandler
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
        var context = RemoveTeamContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await teamPermissionsService.GetPermissionsAsync(workspaceMember.Id, context.TeamId, cancellationToken);
        if (!permissions.CanDelete)
        {
            throw new UnauthorizedAccessException();
        }

        await teamService.RemoveAsync(workspaceMember.Id, context.TeamId, cancellationToken);

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
