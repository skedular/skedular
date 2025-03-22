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

namespace Slack.Api.Handlers.ActionHandlers.Resource;

public class EditResourceButtonHandler(
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
        var context = EditResourceContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await locationService.GetPermissionsAsync(context.LocationId, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new Unauthorized();
        }

        var values = viewSubmission.View.State.Values;
        var updateInput = new UpdateResourceInput { Id = context.ResourceId };

        if (values.TryGetValue(OptionLoaderKeys.OrganizationResourceTypeKey, out var locationBlock))
        {
            if (locationBlock.TryGetValue(OptionLoaderKeys.OrganizationResourceTypeKey, out var location))
            {
                if (location is ExternalSelectValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.SelectedOption?.Value);
                    updateInput.TagIds.Add(value.SelectedOption?.Value);
                }
                else
                {
                    throw new InvalidOperationException("Resource Type must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("Resource Type block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("Resource Type block is missing");
        }

        if (values.TryGetValue(ResourceActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(ResourceActionTypes.Name, out var name))
            {
                if (name is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    updateInput.Name = value.Value.ToSafeString();
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

        if (values.TryGetValue(ResourceActionTypes.Inactive, out var deactivatedBlock))
        {
            if (deactivatedBlock.TryGetValue(ResourceActionTypes.Inactive, out var deactivated))
            {
                if (deactivated is CheckboxGroupValue value)
                {
                    updateInput.Inactive = value.SelectedOptions.Any(item => item.Value == ResourceActionTypes.Inactive);
                }
                else
                {
                    throw new InvalidOperationException("inactive must be CheckboxGroupValue");
                }
            }
            else
            {
                throw new InvalidOperationException("inactive block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("inactive block is missing");
        }

        if (values.TryGetValue(ResourceActionTypes.RequireBookingApproval, out var requireBookingApprovalBlock))
        {
            if (requireBookingApprovalBlock.TryGetValue(ResourceActionTypes.RequireBookingApproval, out var requireBookingApproval))
            {
                if (requireBookingApproval is CheckboxGroupValue value)
                {
                    updateInput.RequireBookingApproval = value.SelectedOptions.Any(item => item.Value == ResourceActionTypes.RequireBookingApproval);
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
                    updateInput.TagIds.AddRange(value.SelectedOptions.Select(item => item.Value).ToList());
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
                    updateInput.TagIds.AddRange(value.SelectedOptions.Select(item => item.Value).ToList());
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

        await locationServiceClient.UpdateResourceAsync(
            updateInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await pageNavigator.BackAsync(workspace, workspaceMember, new CommonPageContext(context.PageContext), viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
