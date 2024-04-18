using Api.Shared.Services.Grpc.UnityHub.Team.V1;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Interaction;
using TeamService = Api.Shared.Services.Grpc.UnityHub.Team.V1.TeamService;

namespace Slack.Api.Handlers.ActionHandlers.Team;

public class RemoveTeamButtonHandler(
    TeamConfiguration teamConfiguration,
    TeamService.TeamServiceClient teamServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ITeamService teamService,
    IMapper mapper,
    IPageNavigator pageNavigator) : IViewSubmissionHandler
{
    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;

        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                viewSubmission.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = RemoveTeamContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await teamService.GetPermissionsAsync(context.TeamId, workspaceMember, cancellationToken);
        if (!permissions.CanDelete)
        {
            throw new Unauthorized();
        }

        await teamServiceClient.RemoveAsync(
            new RemoveInput { Id = context.TeamId },
            teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

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
