using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
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
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;
using OrganizationService = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService;
using Option = SlackNet.Blocks.Option;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection;

namespace Slack.Api.Handlers.ActionHandlers.Desk;

public class AddDeskButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    LocationConfiguration locationConfiguration,
    LocationService.LocationServiceClient locationServiceClient,
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    ICustomerService customerService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                request.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(customer);

        var context = AddDeskContext.Deserialize(action.Value);

        var name = new InputBlock
        {
            BlockId = DeskActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = DeskActionTypes.Name },
            Optional = false
        };

        var deactivated = new InputBlock
        {
            BlockId = DeskActionTypes.Deactivated,
            Label = "Activation Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = DeskActionTypes.Deactivated,
                    Options = new List<Option>
                    {
                        new() { Text = "Deactivated".ToPlainText(), Value = DeskActionTypes.Deactivated }
                    }
                },
            Optional = true
        };

        var requireBookingApproval = new InputBlock
        {
            BlockId = DeskActionTypes.RequireBookingApproval,
            Label = "Booking Approval Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = DeskActionTypes.RequireBookingApproval,
                    Options = new List<Option>
                    {
                        new()
                        {
                            Text = "Require Booking Approval".ToPlainText(),
                            Value = DeskActionTypes.RequireBookingApproval
                        }
                    }
                },
            Optional = true
        };

        var blocks = new List<Block> { name, deactivated, requireBookingApproval };

        var customTagConnection = await GetCustomTagsAsync(workspace, workspaceMember, cancellationToken);
        if (customTagConnection.Edges.Count != 0)
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
                        Description =
                            string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                    }).ToList()
                },
                Optional = true
            });
        }

        var zoneConnection = await GetZonesAsync(workspace, workspaceMember, cancellationToken);
        if (zoneConnection.Edges.Count != 0)
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
                        Description =
                            string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
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
                CallbackId = DeskCallbackTypes.AddDesk,
                Title = "Add Desk",
                Close = "Cancel",
                Submit = "Add",
                Blocks = blocks,
                PrivateMetadata = action.Value
            },
            cancellationToken);
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfiguration.EnableAsyncMode)
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
        var context = AddDeskContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var deskId = randomHelper.Generate();
        var addDeskInput = new AddDeskInput { Id = deskId, LocationId = context.LocationId };

        if (values.TryGetValue(DeskActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(DeskActionTypes.Name, out var name))
            {
                if (name is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    addDeskInput.Name = value.Value.ToSafeString();
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

        if (values.TryGetValue(DeskActionTypes.Deactivated, out var deactivatedBlock))
        {
            if (deactivatedBlock.TryGetValue(DeskActionTypes.Deactivated, out var deactivated))
            {
                if (deactivated is CheckboxGroupValue value)
                {
                    addDeskInput.Deactivated =
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
            if (requireBookingApprovalBlock.TryGetValue(DeskActionTypes.RequireBookingApproval,
                    out var requireBookingApproval))
            {
                if (requireBookingApproval is CheckboxGroupValue value)
                {
                    addDeskInput.RequireBookingApproval =
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
                    addDeskInput.CustomTagIds.AddRange(value.SelectedOptions.Select(item => item.Value).ToList());
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
        else
        {
            throw new InvalidOperationException("customTags block is missing");
        }

        if (values.TryGetValue(ZoneActionTypes.Zones, out var zonesBlock))
        {
            if (zonesBlock.TryGetValue(ZoneActionTypes.Zones, out var zones))
            {
                if (zones is StaticMultiSelectValue value)
                {
                    addDeskInput.ZoneIds.AddRange(value.SelectedOptions.Select(item => item.Value).ToList());
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
        else
        {
            throw new InvalidOperationException("zones block is missing");
        }

        await locationServiceClient.AddDeskAsync(
            addDeskInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;

    private async Task<CustomTagConnection> GetCustomTagsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        var getPaginatedCustomTagsInput = new GetPaginatedCustomTagsInput
        {
            After = string.Empty,
            First = -1,
            Before = string.Empty,
            Last = -1,
            Where = new CustomTagWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedCustomTagsInput.OrderBy.AddRange([
            new CustomTagOrderInput { Direction = OrderDirection.Ascending, Field = CustomTagOrderField.CustomTagName }
        ]);

        return await organizationServiceClient.GetPaginatedCustomTagsAsync(
            getPaginatedCustomTagsInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private async Task<ZoneConnection> GetZonesAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        var getPaginatedZonesInput = new GetPaginatedZonesInput
        {
            After = string.Empty,
            First = -1,
            Before = string.Empty,
            Last = -1,
            Where = new ZoneWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedZonesInput.OrderBy.AddRange([
            new ZoneOrderInput { Direction = OrderDirection.Ascending, Field = ZoneOrderField.ZoneName }
        ]);

        return await organizationServiceClient.GetPaginatedZonesAsync(
            getPaginatedZonesInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }
}
