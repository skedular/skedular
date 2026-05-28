using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Icons = Slack.Shared.Constants.Icons;
using Option = SlackNet.Blocks.Option;
using Button = SlackNet.Blocks.Button;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface ITeamsPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class TeamsPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ICommonComponents commonComponents,
    IBookingsPage bookingsPage,
    ITeamComponents teamComponents,
    ITeamPermissionsService teamPermissionsService,
    IBookingPermissionsService bookingPermissionsService,
    ITeamService teamService,
    IEntityMapper entityMapper,
    IBookingsPageContextService bookingsPageContextService) :
    ITeamsPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<ButtonAction>
{
    private const int TeamsPageSize = 5;
    private const string TeamsCallback = "Teams";
    private const string FirstPageTeams = "Teams_FirstPageTeams";
    private const string PreviousPageTeams = "Teams_PreviousPageTeams";
    private const string NextPageTeams = "Teams_NextPageTeams";
    private const string LastPageTeams = "Teams_LastPageTeams";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case FirstPageTeams:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageTeams:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageTeams:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageTeams:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;
        }
    }

    public async Task HandleAsync(StaticSelectAction action, BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);

        if (action.SelectedOption.Value.StartsWith(BookingActionTypes.Bookings))
        {
            var teamId = action.SelectedOption.Value[BookingActionTypes.Bookings.Length..];
            var bookingPermissions = await bookingPermissionsService.GetTeamPermissionsAsync(workspaceMember.Id, teamId, cancellationToken);
            if (!bookingPermissions.CanViewBookings)
            {
                throw new UnauthorizedAccessException();
            }

            var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.BookingsPage = context.PageContext.BookingsPage = bookingsPageContextService.GetDefaultBookingsPageContext();
            context.PageContext.PushCurrentPageToVisitedPages();

            await bookingsPage.RenderWithContextAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                request.View.Hash,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(TeamActionTypes.EditTeam))
        {
            var teamId = action.SelectedOption.Value[TeamActionTypes.EditTeam.Length..];
            var permissions = await teamPermissionsService.GetPermissionsAsync(workspaceMember.Id, teamId, cancellationToken);
            if (!permissions.CanModify)
            {
                throw new UnauthorizedAccessException();
            }

            var context = EditTeamContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.PushCurrentPageToVisitedPages();
            context.TeamId = teamId;

            await OpenEditTeamDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(TeamActionTypes.RemoveTeam))
        {
            var teamId = action.SelectedOption.Value[TeamActionTypes.RemoveTeam.Length..];
            var permissions = await teamPermissionsService.GetPermissionsAsync(workspaceMember.Id, teamId, cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new UnauthorizedAccessException();
            }

            var context = RemoveTeamContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.PushCurrentPageToVisitedPages();
            context.TeamId = teamId;

            await OpenRemoveTeamDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
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

    public async Task Handle(StaticSelectAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.StaticSelectActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);
        if (commonPageContext.PageContext.TeamsPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.TeamsPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.TeamsPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.TeamsPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.TeamsPage.Pagination.CurrentLast,
                commonPageContext,
                hash,
                cancellationToken);
        }
    }

    private async Task RenderFirstPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            TeamsPageSize,
            null,
            null,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderPreviousPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.TeamsPage.Pagination.Before,
            TeamsPageSize,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderNextPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.TeamsPage.Pagination.After,
            TeamsPageSize,
            null,
            null,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderLastPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            TeamsPageSize,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderInternalAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Teams;

        var connection = await teamService.GetPaginatedTeamsAsync(
            workspaceMember.Id,
            workspace.Organization.Id,
            null,
            after,
            first,
            before,
            last,
            cancellationToken);

        var teams = connection.Edges.Select(item => item.Node).ToList();
        var teamIds = teams.Select(item => item.Id).ToList();
        var teamsWithChannel = await repositoryFactory.TeamRepository.GetActiveByIdsAsync(teamIds, cancellationToken);
        teams = teams.Select(item =>
        {
            var matchedTeam = teamsWithChannel.FirstOrDefault(replicatedTeam => replicatedTeam.Id == item.Id);
            if (matchedTeam is not null)
            {
                item.DailyUpdateChannel = entityMapper.MapTo(matchedTeam.DailyUpdateChannel);
            }

            return item;
        }).ToList();

        var asyncBlocks = await Task.WhenAll(
            GetToolbarAsync(workspace, workspaceMember, commonPageContext.PageContext, cancellationToken),
            teamComponents.GetTeamCardsAsync(workspaceMember, teams, commonPageContext.PageContext, cancellationToken));

        IReadOnlyList<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetTeamsSearchCriteriaAndPaginationBlocks(connection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = TeamsCallback,
                Blocks = blocks.SelectMany(item => item.Count == 0 ? item : item.Append(new DividerBlock())).SkipLast(1).ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<StaticSelectAction, TeamsPage>(TeamActionTypes.ActionsMenu)
            .RegisterBlockActionHandler<ButtonAction, TeamsPage>(FirstPageTeams)
            .RegisterBlockActionHandler<ButtonAction, TeamsPage>(LastPageTeams)
            .RegisterBlockActionHandler<ButtonAction, TeamsPage>(NextPageTeams)
            .RegisterBlockActionHandler<ButtonAction, TeamsPage>(PreviousPageTeams);

    private static IReadOnlyList<Block> GetTitle() => [new SectionBlock { Text = "*Teams*".ToMarkdown() }];

    private async Task<IReadOnlyList<Block>> GetToolbarAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, workspaceMember.Timezone);
        var addTeamButton = await teamComponents.GetAddTeamButtonAsync(workspace, workspaceMember, pageContext, cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>().Concat(homeAndBackButtons).Concat(addTeamButton).Concat(feedbackButton).ToList()
            }
        ];
    }

    private static List<Block> GetTeamsSearchCriteriaAndPaginationBlocks(Connection<TeamEdge> teamConnection, PageContext pageContext)
    {
        if (!teamConnection.Edges.Any())
        {
            return [new SectionBlock { Text = "No team found".ToMarkdown() }];
        }

        var totalTeamsCount =
            new SectionBlock { Text = $"Total teams: {teamConnection.TotalCount}".ToMarkdown() };
        if (teamConnection.TotalCount <= TeamsPageSize)
        {
            return [totalTeamsCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.TeamsPage);

        var paginationButtons = new List<IActionElement>();
        if (teamConnection.PageInfo.HasPreviousPage)
        {
            pageContext.TeamsPage.Pagination.First = TeamsPageSize;
            pageContext.TeamsPage.Pagination.After = null;
            pageContext.TeamsPage.Pagination.Before = null;
            pageContext.TeamsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageTeams, Text = Icons.FirstPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.TeamsPage.Pagination.First = null;
            pageContext.TeamsPage.Pagination.After = null;
            pageContext.TeamsPage.Pagination.Before = teamConnection.PageInfo.StartCursor;
            pageContext.TeamsPage.Pagination.Last = TeamsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageTeams, Text = Icons.PreviousPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (teamConnection.PageInfo.HasNextPage)
        {
            pageContext.TeamsPage.Pagination.First = TeamsPageSize;
            pageContext.TeamsPage.Pagination.After = teamConnection.PageInfo.EndCursor;
            pageContext.TeamsPage.Pagination.Before = null;
            pageContext.TeamsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageTeams, Text = Icons.NextPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.TeamsPage.Pagination.First = null;
            pageContext.TeamsPage.Pagination.After = null;
            pageContext.TeamsPage.Pagination.Before = null;
            pageContext.TeamsPage.Pagination.Last = TeamsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageTeams, Text = Icons.LastPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return [totalTeamsCount, paginationActionBlock];
    }

    private async Task OpenEditTeamDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        EditTeamContext context,
        CancellationToken cancellationToken)
    {
        var team = await teamService.GetAsync(workspaceMember.Id, context.TeamId, cancellationToken);
        var name = new InputBlock
        {
            BlockId = TeamActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = TeamActionTypes.Name, InitialValue = team.Name.ToSafeString() },
            Optional = false
        };

        var about = new InputBlock
        {
            BlockId = TeamActionTypes.About,
            Label = "About".ToPlainText(),
            Element = new PlainTextInput { ActionId = TeamActionTypes.About, InitialValue = team.About.ToSafeString(), Multiline = true },
            Optional = true
        };

        var timezone = new InputBlock
        {
            BlockId = OptionLoaderKeys.TimezoneKey,
            Label = "Timezone".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.TimezoneKey,
                InitialOption = string.IsNullOrWhiteSpace(team.Timezone)
                    ? null
                    : new Option { Text = team.Timezone.ToOptionText(), Value = team.Timezone },
                MinQueryLength = 3
            },
            Optional = true
        };

        var primaryLocation = new InputBlock
        {
            BlockId = TeamActionTypes.PrimaryLocation,
            Label = "Primary Location".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationLocationKey,
                InitialOption = team.PrimaryLocation is null
                    ? null
                    : new Option { Text = team.PrimaryLocation.Name.ToOptionText(), Value = team.PrimaryLocation.Id },
                MinQueryLength = 3
            },
            Optional = true
        };

        var teamEntity = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);

        var updateChannel = new InputBlock
        {
            BlockId = TeamActionTypes.SlackUpdateChannel,
            Label = "Slack update channel".ToPlainText(),
            Element = new ChannelSelectMenu
            {
                ActionId = TeamActionTypes.SlackUpdateChannel, InitialChannel = teamEntity?.DailyUpdateChannel?.Id
            },
            Optional = true
        };

        var organizationMembers = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationMemberAndCustomerPairKey,
            Label = "Members".ToPlainText(),
            Element = new ExternalMultiSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationMemberAndCustomerPairKey,
                InitialOptions = team.TeamMembers.Where(item => item.OrganizationMember is not null).Select(item => new Option
                {
                    Text = item.Customer.DisplayableName.ToOptionText(),
                    Value = $"{item.OrganizationMember!.Id}{Global.OptionLoaderValueSeparator}{item.Customer.Id}"
                }).ToList(),
                MinQueryLength = 0
            },
            Optional = false
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = TeamCallbackTypes.EditTeam,
                Title = "Edit Team",
                Close = "Cancel",
                Submit = "Save",
                Blocks =
                [
                    name, about, timezone, primaryLocation, updateChannel, organizationMembers
                ],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task OpenRemoveTeamDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveTeamContext context,
        CancellationToken cancellationToken)
    {
        var team = await teamService.GetAsync(workspaceMember.Id, context.TeamId, cancellationToken);
        var confirmationMessage = new SectionBlock { Text = $"Are you sure you want to remove the team {team.Name.ToSafeString()}?" };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = TeamCallbackTypes.RemoveTeam,
                Title = "Remove Team",
                Close = "No",
                Submit = "Yes",
                Blocks = [confirmationMessage],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }
}
