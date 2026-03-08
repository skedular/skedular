using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Option = SlackNet.Blocks.Option;

namespace Slack.Api.Handlers.ActionHandlers.Resource;

public class AddResourceButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator,
    IOrganizationZoneService organizationZoneService,
    IOrganizationCustomTagService organizationCustomTagService,
    ILocationResourceService locationResourceService)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var resourceType = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationResourceTypeKey,
            Label = "Resource Type".ToPlainText(),
            Element =
                new ExternalSelectMenu { ActionId = OptionLoaderKeys.OrganizationResourceTypeKey, InitialOption = null, MinQueryLength = 0 },
            Optional = false
        };

        var name = new InputBlock
        {
            BlockId = ResourceActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = ResourceActionTypes.Name },
            Optional = false
        };

        var deactivated = new InputBlock
        {
            BlockId = ResourceActionTypes.Inactive,
            Label = "Activation Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = ResourceActionTypes.Inactive,
                    Options = new List<Option> { new() { Text = "Inactive".ToPlainText(), Value = ResourceActionTypes.Inactive } }
                },
            Optional = true
        };

        var requireBookingApproval = new InputBlock
        {
            BlockId = ResourceActionTypes.RequireBookingApproval,
            Label = "Booking Approval Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = ResourceActionTypes.RequireBookingApproval,
                    Options = new List<Option>
                    {
                        new() { Text = "Require Booking Approval".ToPlainText(), Value = ResourceActionTypes.RequireBookingApproval }
                    }
                },
            Optional = true
        };

        var capacity = new InputBlock
        {
            BlockId = ResourceActionTypes.Capacity,
            Label = "Capacity".ToPlainText(),
            Element = new PlainTextInput { ActionId = ResourceActionTypes.Capacity, InitialValue = "1" },
            Optional = false
        };

        var blocks = new List<Block>
        {
            resourceType,
            name,
            deactivated,
            requireBookingApproval,
            capacity
        };

        var customTagConnection =
            await organizationCustomTagService.GetAllCustomTagsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        if (customTagConnection.Edges.Any())
        {
            blocks.Add(new InputBlock
            {
                BlockId = CustomTagActionTypes.CustomTags,
                Label = "Tags".ToPlainText(),
                Element = new StaticMultiSelectMenu
                {
                    ActionId = CustomTagActionTypes.CustomTags,
                    Options = customTagConnection.Edges.Select(item => item.Node).Select(item => new Option
                    {
                        Text = item.Name.ToOptionText(),
                        Value = item.Id,
                        Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                    }).ToList()
                },
                Optional = true
            });
        }

        var zoneConnection = await organizationZoneService.GetAllZonesAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        if (zoneConnection.Edges.Any())
        {
            blocks.Add(new InputBlock
            {
                BlockId = ZoneActionTypes.Zones,
                Label = "Zones".ToPlainText(),
                Element = new StaticMultiSelectMenu
                {
                    ActionId = ZoneActionTypes.Zones,
                    Options = zoneConnection.Edges.Select(item => item.Node).Select(item => new Option
                    {
                        Text = item.Name.ToOptionText(),
                        Value = item.Id,
                        Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                    }).ToList()
                },
                Optional = true
            });
        }

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = ResourceCallbackTypes.AddResource,
                Title = "Add Resource",
                Close = "Cancel",
                Submit = "Add",
                Blocks = blocks,
                PrivateMetadata = action.Value
            },
            cancellationToken);
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.ButtonActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

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
        var context = AddResourceContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var deskId = randomHelper.Generate();
        var resource = new Shared.Models.Resource { Id = deskId, Location = new Shared.Models.Location { Id = context.LocationId } };

        if (values.TryGetValue(OptionLoaderKeys.OrganizationResourceTypeKey, out var resourceTypeBlock))
        {
            if (resourceTypeBlock.TryGetValue(OptionLoaderKeys.OrganizationResourceTypeKey, out var block))
            {
                if (block is ExternalSelectValue value)
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
            if (nameBlock.TryGetValue(ResourceActionTypes.Name, out var block))
            {
                if (block is PlainTextInputValue value)
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

        if (values.TryGetValue(ResourceActionTypes.Inactive, out var deactivatedBlock))
        {
            if (deactivatedBlock.TryGetValue(ResourceActionTypes.Inactive, out var block))
            {
                if (block is CheckboxGroupValue value)
                {
                    resource.Inactive = value.SelectedOptions.Any(item => item.Value == ResourceActionTypes.Inactive);
                }
                else
                {
                    throw new InvalidOperationException("Inactive must be CheckboxGroupValue");
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
            if (capacityBlock.TryGetValue(ResourceActionTypes.Capacity, out var block))
            {
                if (block is PlainTextInputValue value)
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
            if (customTagsBlock.TryGetValue(CustomTagActionTypes.CustomTags, out var block))
            {
                if (block is StaticMultiSelectValue value)
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
            if (zonesBlock.TryGetValue(ZoneActionTypes.Zones, out var block))
            {
                if (block is StaticMultiSelectValue value)
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

        await locationResourceService.AddAsync(workspaceMember.Id, resource, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
