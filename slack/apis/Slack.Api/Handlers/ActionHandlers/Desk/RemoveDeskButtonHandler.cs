using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Interaction;
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;

namespace Slack.Api.Handlers.ActionHandlers.Desk;

public class RemoveDeskButtonHandler(
    LocationConfiguration locationConfiguration,
    LocationService.LocationServiceClient locationServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ILocationService locationService,
    IMapper mapper,
    IPageNavigator pageNavigator) : IViewSubmissionHandler
{
    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            viewSubmission.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = RemoveDeskContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await locationService.GetPermissionsAsync(context.LocationId, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new Unauthorized();
        }

        await locationServiceClient.RemoveDeskAsync(
            new RemoveDeskInput { Id = context.DeskId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
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
