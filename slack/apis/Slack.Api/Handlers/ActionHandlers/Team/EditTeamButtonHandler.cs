using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;
using TeamService = Api.Shared.Services.Grpc.Skedular.Team.V1.TeamService;

namespace Slack.Api.Handlers.ActionHandlers.Team;

public class EditTeamButtonHandler(
    TeamConfiguration teamConfiguration,
    TeamService.TeamServiceClient teamServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ITeamService teamService,
    IWorkspaceChannelService workspaceChannelService,
    IMapper mapper,
    IRandomHelper randomHelper,
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
        var context = EditTeamContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions =
            await teamService.GetPermissionsAsync(context.TeamId, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new Unauthorized();
        }

        var team = await teamService.GetTeamAsync(context.TeamId, workspaceMember, cancellationToken);
        var values = viewSubmission.View.State.Values;
        var updateInput =
            new UpdateInput { Id = context.TeamId, OrganizationId = workspace.Organization.Id };

        if (values.TryGetValue(TeamActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(TeamActionTypes.Name, out var name))
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

        if (values.TryGetValue(TeamActionTypes.About, out var aboutBlock))
        {
            if (aboutBlock.TryGetValue(TeamActionTypes.About, out var about))
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
                    updateInput.Timezone = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? string.Empty
                        : value.SelectedOption.Value;
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

        if (values.TryGetValue(TeamActionTypes.PrimaryLocation, out var primaryLocationBlock))
        {
            if (primaryLocationBlock.TryGetValue(OptionLoaderKeys.OrganizationLocationKey, out var primaryLocation))
            {
                if (primaryLocation is ExternalSelectValue value)
                {
                    updateInput.PrimaryLocationId = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? string.Empty
                        : value.SelectedOption.Value;
                }
                else
                {
                    throw new InvalidOperationException("primary location must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("primaryLocation block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("primaryLocation block is missing");
        }

        if (values.TryGetValue(OptionLoaderKeys.OrganizationMemberAndCustomerPairKey, out var organizationMembersBlock))
        {
            if (organizationMembersBlock.TryGetValue(OptionLoaderKeys.OrganizationMemberAndCustomerPairKey,
                    out var organizationMembers))
            {
                if (organizationMembers is ExternalMultiSelectValue value)
                {
                    if (value.SelectedOptions.Count == 0)
                    {
                        throw new ArgumentException("No members selected.");
                    }

                    updateInput.Members.AddRange(value.SelectedOptions
                        .Select(item =>
                        {
                            var memberCustomerIdPair = item.Value.Split(Global.OptionLoaderValueSeparator);
                            var organizationMemberId = memberCustomerIdPair.First();
                            var customerId = memberCustomerIdPair.Last();

                            ArgumentException.ThrowIfNullOrWhiteSpace(organizationMemberId);
                            ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

                            var existingMember = team.TeamMembers.FirstOrDefault(teamMember =>
                                teamMember.OrganizationMember is not null &&
                                teamMember.OrganizationMember.Customer.Id == customerId);

                            return new Member
                            {
                                Id = existingMember is null ? randomHelper.Generate() : existingMember.Id,
                                MembershipType = existingMember is null
                                    ? MembershipType.Member
                                    : existingMember.MembershipType switch
                                    {
                                        TeamMembershipType.Owner => MembershipType.Owner,
                                        TeamMembershipType.Administrator => MembershipType.Administrator,
                                        TeamMembershipType.Member => MembershipType.Member,
                                        _ => throw new ArgumentOutOfRangeException()
                                    },
                                Customer = new Customer { Id = customerId },
                                OrganizationMember = new OrganizationMember
                                {
                                    Id = organizationMemberId, Customer = new Customer { Id = customerId }
                                }
                            };
                        }));
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

        if (values.TryGetValue(TeamActionTypes.SlackUpdateChannel, out var slackUpdateChannelBlock))
        {
            if (slackUpdateChannelBlock.TryGetValue(TeamActionTypes.SlackUpdateChannel, out var slackUpdateChannel))
            {
                if (slackUpdateChannel is ChannelSelectValue value)
                {
                    var teamEntity = await repositoryFactory.TeamRepository
                        .Query(new Specification<Shared.Database.Entities.Team>
                            {
                                Criteria = query => query.Id == context.TeamId
                            }
                            .AddInclude(query => query.DailyUpdateChannel))
                        .FirstOrDefaultAsync(cancellationToken);
                    if (teamEntity is not null)
                    {
                        teamEntity.DailyUpdateChannel = string.IsNullOrWhiteSpace(value.SelectedChannel)
                            ? null
                            : await workspaceChannelService.EnsureChannelResourcesAllExistAsync(
                                workspaceEntity,
                                value.SelectedChannel,
                                cancellationToken);
                        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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

        await teamServiceClient.UpdateAsync(
            updateInput,
            teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
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
