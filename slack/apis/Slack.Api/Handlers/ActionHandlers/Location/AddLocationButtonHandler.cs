using Api.Shared.Services;
using Api.Shared.Services.Models;
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

namespace Slack.Api.Handlers.ActionHandlers.Location;

public class AddLocationButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IWorkspaceChannelService workspaceChannelService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator,
    ILocationService locationService)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        _ = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var name = new InputBlock
        {
            BlockId = LocationActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = LocationActionTypes.Name },
            Optional = false
        };

        var about = new InputBlock
        {
            BlockId = LocationActionTypes.About,
            Label = "About".ToPlainText(),
            Element = new PlainTextInput { ActionId = LocationActionTypes.About, Multiline = true },
            Optional = true
        };

        var timezone = new InputBlock
        {
            BlockId = OptionLoaderKeys.TimezoneKey,
            Label = "Timezone".ToPlainText(),
            Element = new ExternalSelectMenu { ActionId = OptionLoaderKeys.TimezoneKey, MinQueryLength = 3 },
            Optional = false
        };

        var updateChannel = new InputBlock
        {
            BlockId = LocationActionTypes.SlackUpdateChannel,
            Label = "Slack update channel".ToPlainText(),
            Element = new ChannelSelectMenu { ActionId = LocationActionTypes.SlackUpdateChannel },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = LocationCallbackTypes.AddLocation,
                Title = "Add Location",
                Close = "Cancel",
                Submit = "Add",
                Blocks = [name, about, timezone, updateChannel],
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
        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(workspaceEntity, viewSubmission.User.Id, cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = CommonPageContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var locationId = randomHelper.Generate();
        var location = new Shared.Models.Location
        {
            Id = randomHelper.Generate(), Organization = new Organization { Id = workspace.Organization.Id }, Type = LocationType.Private
        };

        if (values.TryGetValue(LocationActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(LocationActionTypes.Name, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    location.Name = value.Value.ToSafeString();
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

        if (values.TryGetValue(LocationActionTypes.About, out var aboutBlock))
        {
            if (aboutBlock.TryGetValue(LocationActionTypes.About, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    location.ListingMetadata = location.ListingMetadata with { About = value.Value.ToSafeString() };
                }
                else
                {
                    throw new InvalidOperationException("about must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("about block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("about block is missing");
        }

        if (values.TryGetValue(OptionLoaderKeys.TimezoneKey, out var timezoneBlock))
        {
            if (timezoneBlock.TryGetValue(OptionLoaderKeys.TimezoneKey, out var block))
            {
                if (block is ExternalSelectValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.SelectedOption.Value);
                    location.Timezone = value.SelectedOption.Value;
                }
                else
                {
                    throw new InvalidOperationException("timezone must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("timezone block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("timezone block is missing");
        }

        if (values.TryGetValue(LocationActionTypes.SlackUpdateChannel, out var slackUpdateChannelBlock))
        {
            if (slackUpdateChannelBlock.TryGetValue(LocationActionTypes.SlackUpdateChannel, out var slackUpdateChannel))
            {
                if (slackUpdateChannel is ChannelSelectValue value)
                {
                    var locationEntity = await repositoryFactory.LocationRepository.UpsertNakedAsync(locationId, cancellationToken);
                    locationEntity.DailyUpdateChannel = string.IsNullOrWhiteSpace(value.SelectedChannel)
                        ? null
                        : await workspaceChannelService.EnsureChannelResourcesAllExistAsync(
                            workspaceEntity,
                            value.SelectedChannel,
                            cancellationToken);
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException("slack update channel must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("slack update channel block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("slack update channel block is missing");
        }

        await locationService.AddAsync(workspaceMember.Id, location, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
