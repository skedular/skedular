using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Interaction;
using OrganizationService = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService;

namespace Slack.Api.Handlers.ActionHandlers.Zone;

public class RemoveZoneButtonHandler(
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IOrganizationService organizationService,
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
        var context = RemoveZoneContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await organizationService.GetPermissionsAsync(workspace, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new Unauthorized();
        }

        await organizationServiceClient.RemoveZoneAsync(
            new RemoveZoneInput { Id = context.ZoneId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
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
