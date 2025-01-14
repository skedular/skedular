using Api.Shared.Services.Grpc.Skedular.Location.V1;
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
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;

namespace Slack.Api.Handlers.ActionHandlers.Desk;

public class EditDeskButtonHandler(
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
        var context = EditDeskContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions =
            await locationService.GetPermissionsAsync(context.LocationId, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new Unauthorized();
        }

        var values = viewSubmission.View.State.Values;
        var updateDeskInput = new UpdateDeskInput { Id = context.DeskId };

        if (values.TryGetValue(DeskActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(DeskActionTypes.Name, out var name))
            {
                if (name is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    updateDeskInput.Name = value.Value.ToSafeString();
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

        if (values.TryGetValue(DeskActionTypes.Deactivated, out var deactivatedBlock))
        {
            if (deactivatedBlock.TryGetValue(DeskActionTypes.Deactivated, out var deactivated))
            {
                if (deactivated is CheckboxGroupValue value)
                {
                    updateDeskInput.Deactivated =
                        value.SelectedOptions.Any(item => item.Value == DeskActionTypes.Deactivated);
                }
                else
                {
                    throw new InvalidOperationException("deactivated must be CheckboxGroupValue");
                }
            }
            else
            {
                throw new InvalidOperationException("deactivated block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("deactivated block is missing");
        }

        if (values.TryGetValue(DeskActionTypes.RequireBookingApproval, out var requireBookingApprovalBlock))
        {
            if (requireBookingApprovalBlock.TryGetValue(
                    DeskActionTypes.RequireBookingApproval,
                    out var requireBookingApproval))
            {
                if (requireBookingApproval is CheckboxGroupValue value)
                {
                    updateDeskInput.RequireBookingApproval =
                        value.SelectedOptions.Any(item => item.Value == DeskActionTypes.RequireBookingApproval);
                }
                else
                {
                    throw new InvalidOperationException("requireBookingApproval must be CheckboxGroupValue");
                }
            }
            else
            {
                throw new InvalidOperationException("requireBookingApproval block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("requireBookingApproval block is missing");
        }

        if (values.TryGetValue(CustomTagActionTypes.CustomTags, out var customTagsBlock))
        {
            if (customTagsBlock.TryGetValue(CustomTagActionTypes.CustomTags, out var customTags))
            {
                if (customTags is StaticMultiSelectValue value)
                {
                    updateDeskInput.CustomTagIds.AddRange(value.SelectedOptions.Select(item => item.Value).ToList());
                }
                else
                {
                    throw new InvalidOperationException("customTags must be StaticMultiSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("customTags block is missing");
            }
        }

        if (values.TryGetValue(ZoneActionTypes.Zones, out var zonesBlock))
        {
            if (zonesBlock.TryGetValue(ZoneActionTypes.Zones, out var zones))
            {
                if (zones is StaticMultiSelectValue value)
                {
                    updateDeskInput.ZoneIds.AddRange(value.SelectedOptions.Select(item => item.Value).ToList());
                }
                else
                {
                    throw new InvalidOperationException("zones must be StaticMultiSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("zones block is missing");
            }
        }

        await locationServiceClient.UpdateDeskAsync(
            updateDeskInput,
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
