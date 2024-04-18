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
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using LocationService = Api.Shared.Services.Grpc.UnityHub.Location.V1.LocationService;

namespace Slack.Api.Handlers.ActionHandlers.Zone;

public class AddZoneButtonHandler(
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

        var name = new InputBlock
        {
            BlockId = ZoneActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = ZoneActionTypes.Name },
            Optional = false
        };

        var description = new InputBlock
        {
            BlockId = ZoneActionTypes.Description,
            Label = "Description".ToPlainText(),
            Element = new PlainTextInput { ActionId = ZoneActionTypes.Description, Multiline = true },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = ZoneCallbackTypes.AddZone,
                Title = "Add Zone",
                Close = "Cancel",
                Submit = "Add",
                Blocks = [name, description],
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
        var context = AddZoneContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var zoneId = randomHelper.Generate();
        var addTagInput = new AddTagInput { Id = zoneId, LocationId = context.LocationId, Type = LocationTagType.Zone };

        if (values.TryGetValue(ZoneActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(ZoneActionTypes.Name, out var name))
            {
                if (name is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    addTagInput.Name = value.Value.ToSafeString();
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
            if (descriptionBlock.TryGetValue(ZoneActionTypes.Description, out var description))
            {
                if (description is PlainTextInputValue value)
                {
                    addTagInput.Description = value.Value.ToSafeString();
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

        await locationServiceClient.AddTagAsync(
            addTagInput,
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
}
