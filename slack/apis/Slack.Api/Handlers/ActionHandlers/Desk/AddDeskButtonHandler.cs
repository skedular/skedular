using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
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
using LocationService = Api.Shared.Services.Grpc.UnityHub.Location.V1.LocationService;
using Option = SlackNet.Blocks.Option;

namespace Slack.Api.Handlers.ActionHandlers.Desk;

public class AddDeskButtonHandler(
    LocationConfiguration locationConfiguration,
    LocationService.LocationServiceClient locationServiceClient,
    ICustomerService customerService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator) : IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        var cancellationToken = CancellationToken.None;

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
        var zoneConnection = await GetZonesAsync(context.LocationId, workspaceMember, cancellationToken);
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
        await slackApiClient.Views.Open(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = DeskCallbackTypes.AddDesk,
                Title = "Add Desk",
                Close = "Cancel",
                Submit = "Add",
                Blocks = blocks,
                PrivateMetadata = action.Value
            });
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

        if (values.TryGetValue(ZoneActionTypes.Zones, out var zonesBlock))
        {
            if (zonesBlock.TryGetValue(ZoneActionTypes.Zones, out var zones))
            {
                if (zones is StaticMultiSelectValue value)
                {
                    addDeskInput.TagIds.AddRange(value.SelectedOptions.Select(item => item.Value).ToList());
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

    private async Task<TagConnection> GetZonesAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        var getPaginatedTagsInput = new GetPaginatedTagsInput
        {
            After = string.Empty,
            First = -1,
            Before = string.Empty,
            Last = -1,
            Where = new TagWhereInput { LocationId = locationId, Type = LocationTagType.Zone }
        };

        getPaginatedTagsInput.OrderBy.AddRange([
            new TagOrderInput { Direction = OrderDirection.Ascending, Field = TagOrderField.TagName }
        ]);

        return await locationServiceClient.GetPaginatedTagsAsync(
            getPaginatedTagsInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }
}
