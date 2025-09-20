using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
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

namespace Slack.Api.Handlers.ActionHandlers.Team;

public class EditTeamButtonHandler(
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ITeamService teamService,
    ITeamPermissionsService teamPermissionsService,
    IWorkspaceChannelService workspaceChannelService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator) : IViewSubmissionHandler
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
        var context = EditTeamContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var permissions = await teamPermissionsService.GetPermissionsAsync(workspaceMember.Id, context.TeamId, cancellationToken);
        if (!permissions.CanModify)
        {
            throw new UnauthorizedAccessException();
        }

        var existingTeam = await teamService.GetAsync(workspaceMember.Id, context.TeamId, cancellationToken);
        var values = viewSubmission.View.State.Values;
        var team = new Shared.Models.Team { Id = context.TeamId, Organization = new Organization { Id = workspace.Organization.Id } };

        if (values.TryGetValue(TeamActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(TeamActionTypes.Name, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    team.Name = value.Value.ToSafeString();
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
            if (aboutBlock.TryGetValue(TeamActionTypes.About, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    team.About = value.Value.ToSafeString();
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
                    team.Timezone = string.IsNullOrWhiteSpace(value.SelectedOption?.Value) ? string.Empty : value.SelectedOption.Value;
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
            if (primaryLocationBlock.TryGetValue(OptionLoaderKeys.OrganizationLocationKey, out var block))
            {
                if (block is ExternalSelectValue value)
                {
                    team.PrimaryLocation = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? null
                        : new Shared.Models.Location { Id = value.SelectedOption.Value };
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
            if (organizationMembersBlock.TryGetValue(OptionLoaderKeys.OrganizationMemberAndCustomerPairKey, out var block))
            {
                if (block is ExternalMultiSelectValue value)
                {
                    if (value.SelectedOptions.Count == 0)
                    {
                        throw new ArgumentException("No members selected.");
                    }

                    team.TeamMembers = value.SelectedOptions
                        .Select(item =>
                        {
                            var memberCustomerIdPair = item.Value.Split(Global.OptionLoaderValueSeparator);
                            var organizationMemberId = memberCustomerIdPair.First();
                            var customerId = memberCustomerIdPair.Last();

                            ArgumentException.ThrowIfNullOrWhiteSpace(organizationMemberId);
                            ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

                            var existingMember = existingTeam.TeamMembers.FirstOrDefault(teamMember =>
                                teamMember.OrganizationMember is not null &&
                                teamMember.OrganizationMember.Customer.Id == customerId);

                            return new TeamMember
                            {
                                Id = existingMember is null ? randomHelper.Generate() : existingMember.Id,
                                Role = existingMember is null
                                    ? TeamMemberRole.Member
                                    : existingMember.Role switch
                                    {
                                        TeamMemberRole.Owner => TeamMemberRole.Owner,
                                        TeamMemberRole.Administrator => TeamMemberRole.Administrator,
                                        TeamMemberRole.Member => TeamMemberRole.Member,
                                        _ => throw new ArgumentOutOfRangeException()
                                    },
                                Status = TeamMemberStatus.Active,
                                Customer = new Customer { Id = customerId },
                                OrganizationMember = new OrganizationMember { Id = organizationMemberId, Customer = new Customer { Id = customerId } }
                            };
                        }).ToList();
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
            if (slackUpdateChannelBlock.TryGetValue(TeamActionTypes.SlackUpdateChannel, out var block))
            {
                if (block is ChannelSelectValue value)
                {
                    var teamEntity = await repositoryFactory.TeamRepository.Query(
                            new Specification<Shared.Database.Entities.Team> { Criteria = query => query.Id == context.TeamId }
                                .AddInclude(query => query.DailyUpdateChannel!))
                        .FirstOrDefaultAsync(cancellationToken);
                    if (teamEntity is not null)
                    {
                        teamEntity.DailyUpdateChannel = string.IsNullOrWhiteSpace(value.SelectedChannel)
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

        await teamService.UpdateAsync(workspaceMember.Id, team, cancellationToken);

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
