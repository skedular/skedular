using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;
using OrganizationService = Api.Shared.Services.Grpc.UnityHub.Organization.V1.OrganizationService;

namespace Slack.Api.Handlers.ActionHandlers.DeskType;

public class EditDeskTypeButtonHandler(
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
        var context = EditDeskTypeContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions =
            await organizationService.GetPermissionsAsync(workspace, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new Unauthorized();
        }

        var values = viewSubmission.View.State.Values;
        var updateDeskTypeInput = new UpdateDeskTypeInput { Id = context.DeskTypeId };

        if (values.TryGetValue(DeskTypeActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(DeskTypeActionTypes.Name, out var name))
            {
                if (name is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    updateDeskTypeInput.Name = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("name must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("name block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("name block is missing");
        }

        if (values.TryGetValue(DeskTypeActionTypes.Description, out var descriptionBlock))
        {
            if (descriptionBlock.TryGetValue(DeskTypeActionTypes.Description, out var description))
            {
                if (description is PlainTextInputValue value)
                {
                    updateDeskTypeInput.Description = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("description must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("description block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("description block is missing");
        }

        await organizationServiceClient.UpdateDeskTypeAsync(
            updateDeskTypeInput,
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
