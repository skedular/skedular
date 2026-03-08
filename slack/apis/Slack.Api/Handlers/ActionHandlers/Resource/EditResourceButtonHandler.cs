using Api.Shared.Services;
using Enterprise.Shared;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Resource;

public class EditResourceButtonHandler(
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ILocationPermissionsService locationPermissionsService,
    IMapper mapper,
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

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = EditResourceContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new UnauthorizedAccessException();
        }

        var values = viewSubmission.View.State.Values;
        var resource = await locationResourceService.GetAsync(workspaceMember.Id, context.ResourceId, cancellationToken);

        if (values.TryGetValue(OptionLoaderKeys.OrganizationResourceTypeKey, out var locationBlock))
        {
            if (locationBlock.TryGetValue(OptionLoaderKeys.OrganizationResourceTypeKey, out var location))
            {
                if (location is ExternalSelectValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.SelectedOption?.Value);
                    resource.ResourceType = new ResourceType { Id = value.SelectedOption!.Value };
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
                    resource.Name = value.Value.ToSafeString();
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
                    resource.Inactive = value.SelectedOptions.Any(item => item.Value == ResourceActionTypes.Inactive);
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
                    resource.RequireBookingApproval = value.SelectedOptions.Any(item => item.Value == ResourceActionTypes.RequireBookingApproval);
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

        if (values.TryGetValue(ResourceActionTypes.Capacity, out var capacityBlock))
        {
            if (capacityBlock.TryGetValue(ResourceActionTypes.Capacity, out var capacity))
            {
                if (capacity is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);

                    if (int.TryParse(value.Value, out var capacityValue))
                    {
                        if (capacityValue > 0)
                        {
                            resource.Capacity = capacityValue;
                        }
                        else
                        {
                            throw new InvalidOperationException("capacity must be greater than 0");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("capacity value must be integer");
                    }
                }
                else
                {
                    throw new InvalidOperationException("capacity must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("capacity block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("capacity block is missing");
        }

        if (values.TryGetValue(CustomTagActionTypes.CustomTags, out var customTagsBlock))
        {
            if (customTagsBlock.TryGetValue(CustomTagActionTypes.CustomTags, out var customTags))
            {
                if (customTags is StaticMultiSelectValue value)
                {
                    resource.CustomTags = value.SelectedOptions.Select(item => new OrganizationCustomTag { Id = item.Value }).ToList();
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
                    resource.Zones = value.SelectedOptions.Select(item => new OrganizationZone { Id = item.Value }).ToList();
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

        await locationResourceService.UpdateAsync(workspaceMember.Id, resource, cancellationToken);

        await pageNavigator.BackAsync(workspace, workspaceMember, new CommonPageContext(context.PageContext), viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
