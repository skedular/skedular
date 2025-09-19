using Api.Shared.Services;
using Enterprise.Shared;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Zone;

public class EditZoneButtonHandler(
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
        var context = EditZoneContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new UnauthorizedAccessException();
        }

        var values = viewSubmission.View.State.Values;
        string name;
        string description;

        if (values.TryGetValue(ZoneActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(ZoneActionTypes.Name, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    name = value.Value.ToSafeString();
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

        if (values.TryGetValue(ZoneActionTypes.Description, out var descriptionBlock))
        {
            if (descriptionBlock.TryGetValue(ZoneActionTypes.Description, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    description = value.Value.ToSafeString();
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

        await organizationZoneService.UpdateAsync(workspaceMember.Id, context.ZoneId, name, description, cancellationToken);

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
