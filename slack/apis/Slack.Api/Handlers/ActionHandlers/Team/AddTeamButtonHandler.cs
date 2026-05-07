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

namespace Slack.Api.Handlers.ActionHandlers.Team;

public class AddTeamButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IWorkspaceChannelService workspaceChannelService,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator,
    ITeamService teamService)
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

        var workspace = entityMapper.MapTo(workspaceEntity);
        var name = new InputBlock
        {
            BlockId = TeamActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = TeamActionTypes.Name },
            Optional = false
        };

        var about = new InputBlock
        {
            BlockId = TeamActionTypes.About,
            Label = "About".ToPlainText(),
            Element = new PlainTextInput { ActionId = TeamActionTypes.About, Multiline = true },
            Optional = true
        };

        var timezone = new InputBlock
        {
            BlockId = OptionLoaderKeys.TimezoneKey,
            Label = "Timezone".ToPlainText(),
            Element = new ExternalSelectMenu { ActionId = OptionLoaderKeys.TimezoneKey, MinQueryLength = 3 },
            Optional = true
        };

        var primaryLocation = new InputBlock
        {
            BlockId = TeamActionTypes.PrimaryLocation,
            Label = "Primary Location".ToPlainText(),
            Element = new ExternalSelectMenu { ActionId = OptionLoaderKeys.OrganizationLocationKey, InitialOption = null, MinQueryLength = 0 },
            Optional = true
        };

        var updateChannel = new InputBlock
        {
            BlockId = TeamActionTypes.SlackUpdateChannel,
            Label = "Slack update channel".ToPlainText(),
            Element = new ChannelSelectMenu { ActionId = TeamActionTypes.SlackUpdateChannel },
            Optional = true
        };

        var organizationMembers = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationMemberAndCustomerPairKey,
            Label = "Members".ToPlainText(),
            Element = new ExternalMultiSelectMenu { ActionId = OptionLoaderKeys.OrganizationMemberAndCustomerPairKey, MinQueryLength = 0 },
            Optional = false
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = TeamCallbackTypes.AddTeam,
                Title = "Add Team",
                Close = "Cancel",
                Submit = "Add",
                Blocks = [name, about, timezone, primaryLocation, updateChannel, organizationMembers],
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

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        var context = CommonPageContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var team = new Shared.Models.Team { Id = randomHelper.Generate(), Organization = new Organization { Id = workspace.Organization.Id } };

        if (values.TryGetValue(TeamActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(TeamActionTypes.Name, out var name))
            {
                if (name is PlainTextInputValue value)
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
            if (aboutBlock.TryGetValue(TeamActionTypes.About, out var about))
            {
                if (about is PlainTextInputValue value)
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
            if (timezoneBlock.TryGetValue(OptionLoaderKeys.TimezoneKey, out var timezone))
            {
                if (timezone is ExternalSelectValue value)
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
            if (primaryLocationBlock.TryGetValue(OptionLoaderKeys.OrganizationLocationKey, out var primaryLocation))
            {
                if (primaryLocation is ExternalSelectValue value)
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
            if (organizationMembersBlock.TryGetValue(OptionLoaderKeys.OrganizationMemberAndCustomerPairKey, out var organizationMembers))
            {
                if (organizationMembers is ExternalMultiSelectValue value)
                {
                    if (value.SelectedOptions.Count == 0)
                    {
                        throw new ArgumentException("No members selected.");
                    }

                    team.TeamMembers = value.SelectedOptions.Select(item =>
                    {
                        var memberCustomerIdPair = item.Value.Split(Global.OptionLoaderValueSeparator);
                        var organizationMemberId = memberCustomerIdPair.First();
                        var customerId = memberCustomerIdPair.Last();

                        ArgumentException.ThrowIfNullOrWhiteSpace(organizationMemberId);
                        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

                        return new TeamMember
                        {
                            Id = randomHelper.Generate(),
                            Role = TeamMemberRole.Member,
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
            if (slackUpdateChannelBlock.TryGetValue(TeamActionTypes.SlackUpdateChannel, out var slackUpdateChannel))
            {
                if (slackUpdateChannel is ChannelSelectValue value)
                {
                    var teamEntity = await repositoryFactory.TeamRepository.UpsertNakedAsync(team.Id, cancellationToken);
                    teamEntity.DailyUpdateChannel = string.IsNullOrWhiteSpace(value.SelectedChannel)
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

        await teamService.AddAsync(workspaceMember.Id, team, cancellationToken);
        await pageNavigator.BackAsync(workspace, workspaceMember, new CommonPageContext(context.PageContext), viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
