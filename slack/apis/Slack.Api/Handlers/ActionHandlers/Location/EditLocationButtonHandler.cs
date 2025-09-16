using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Microsoft.EntityFrameworkCore;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;
using LocationType = Api.Shared.Services.Models.LocationType;

namespace Slack.Api.Handlers.ActionHandlers.Location;

public class EditLocationButtonHandler(
    LocationConfiguration locationConfiguration,
    LocationService.LocationServiceClient locationServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ILocationPermissionsService locationPermissionsService,
    IWorkspaceChannelService workspaceChannelService,
    IMapper mapper,
    IPageNavigator pageNavigator,
    ILocationService locationService) : IViewSubmissionHandler
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
        var context = EditLocationContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions =
            await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new UnauthorizedAccessException();
        }

        var existingLocation = await locationService.GetAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        var values = viewSubmission.View.State.Values;
        var updateInput = new UpdateInput
        {
            Id = context.LocationId,
            OrganizationId = workspace.Organization.Id,
            Type = existingLocation.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Private,
                LocationType.Marketplace => global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

        if (values.TryGetValue(LocationActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(LocationActionTypes.Name, out var name))
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

        if (values.TryGetValue(LocationActionTypes.About, out var aboutBlock))
        {
            if (aboutBlock.TryGetValue(LocationActionTypes.About, out var about))
            {
                if (about is PlainTextInputValue value)
                {
                    updateInput.About = value.Value.ToSafeString();
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
            if (timezoneBlock.TryGetValue(OptionLoaderKeys.TimezoneKey, out var timezone))
            {
                if (timezone is ExternalSelectValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.SelectedOption.Value);
                    updateInput.Timezone = value.SelectedOption.Value;
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
                    var locationEntity = await repositoryFactory.LocationRepository.Query(
                            new Specification<Shared.Database.Entities.Location> { Criteria = query => query.Id == context.LocationId }
                                .AddInclude(query => query.DailyUpdateChannel!))
                        .FirstOrDefaultAsync(cancellationToken);
                    if (locationEntity is not null)
                    {
                        locationEntity.DailyUpdateChannel = string.IsNullOrWhiteSpace(value.SelectedChannel)
                            ? null
                            : await workspaceChannelService.EnsureChannelResourcesAllExistAsync(
                                workspaceEntity,
                                value.SelectedChannel,
                                cancellationToken);
                        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                    }
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

        await locationServiceClient.UpdateAsync(
            updateInput,
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
