using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
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
using LocationType = Api.Shared.Services.Models.LocationType;

namespace Slack.Api.Handlers.ActionHandlers.Location;

public class EditLocationButtonHandler(
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
        var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new UnauthorizedAccessException();
        }

        var existingLocation = await locationService.GetAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        var values = viewSubmission.View.State.Values;
        var location = new Shared.Models.Location
        {
            Id = context.LocationId,
            Organization = new Organization { Id = workspace.Organization.Id },
            Type = existingLocation.Type ?? LocationType.Private
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

        await locationService.UpdateAsync(workspaceMember.Id, location, cancellationToken);


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
