using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Team.V1;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Microsoft.EntityFrameworkCore;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Icons = Slack.Shared.Constants.Icons;
using Option = SlackNet.Blocks.Option;
using Button = SlackNet.Blocks.Button;
using CustomerService = Api.Shared.Services.Grpc.UnityHub.Customer.V1.CustomerService;
using GetInput = Api.Shared.Services.Grpc.UnityHub.Team.V1.GetInput;
using Team = Slack.Shared.Database.Entities.Team;
using TeamService = Api.Shared.Services.Grpc.UnityHub.Team.V1.TeamService;
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
    TeamConfiguration teamConfiguration,
    CustomerConfiguration customerConfiguration,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ICommonComponents commonComponents,
    CustomerService.CustomerServiceClient customerServiceClient,
    TeamService.TeamServiceClient teamServiceClient,
    IBookingsPage bookingsPage,
    ITeamComponents teamComponents,
    ITeamService teamService,
    IBookingService bookingService,
    IMapper mapper,
    IBookingsPageContextService bookingsPageContextService)
    : IBlockActionHandler<StaticSelectAction>, IBlockActionHandler<ButtonAction>, ITeamsPage
{
    private const int TeamsPageSize = 5;
    private const string TeamsCallback = "Teams";
    private const string FirstPageTeams = "Teams_FirstPageTeams";
    private const string PreviousPageTeams = "Teams_PreviousPageTeams";
    private const string NextPageTeams = "Teams_NextPageTeams";
    private const string LastPageTeams = "Teams_LastPageTeams";

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

            case TeamActionTypes.SetAsDefaultTeam:
                await SetAsDefaultTeamAsync(workspace,
                    workspaceMember,
                    SetAsDefaultTeamContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case TeamActionTypes.ClearDefaultTeam:
                await ClearDefaultTeamAsync(workspace,
                    workspaceMember,
                    ClearDefaultTeamContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;
        }
    }

    public async Task Handle(StaticSelectAction action, BlockActionRequest request)
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

        if (action.SelectedOption.Value.StartsWith(BookingActionTypes.Bookings))
        {
            var teamId = action.SelectedOption.Value[BookingActionTypes.Bookings.Length..];
            var bookingPermissions =
                await bookingService.GetTeamPermissionsAsync(teamId, workspaceMember, cancellationToken);
            if (!bookingPermissions.CanViewBookings)
            {
                throw new Unauthorized();
            }

            var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.BookingsPage =
                context.PageContext.BookingsPage = bookingsPageContextService.GetDefaultBookingsPageContext();
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
            var permissions = await teamService.GetPermissionsAsync(teamId, workspaceMember, cancellationToken);
            if (!permissions.CanModify)
            {
                throw new Unauthorized();
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
            var permissions = await teamService.GetPermissionsAsync(teamId, workspaceMember, cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new Unauthorized();
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

    public async Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);
        if (commonPageContext.PageContext.TeamsPage.TeamsPagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.TeamsPage.TeamsPagination.CurrentAfter,
                commonPageContext.PageContext.TeamsPage.TeamsPagination.CurrentFirst,
                commonPageContext.PageContext.TeamsPage.TeamsPagination.CurrentBefore,
                commonPageContext.PageContext.TeamsPage.TeamsPagination.CurrentLast,
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
            commonPageContext.PageContext.TeamsPage.TeamsPagination.Before,
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
            commonPageContext.PageContext.TeamsPage.TeamsPagination.After,
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

        var teamConnection = await GetPaginatedTeamsAsync(
            workspace,
            workspaceMember,
            after,
            first,
            before,
            last,
            commonPageContext,
            cancellationToken);
        var teams = teamConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var teamIds = teams.Select(item => item.Id).ToList();
        var teamsWithChannel = await repositoryFactory.TeamRepository
            .Query(new Specification<Team>
                {
                    Criteria = query => !query.DeletedAt.HasValue && teamIds.Contains(query.Id)
                }
                .AddInclude(query => query.DailyUpdateChannel))
            .ToListAsync(cancellationToken);
        teams = teams.Select(item =>
        {
            var matchedTeam =
                teamsWithChannel.FirstOrDefault(replicatedTeam => replicatedTeam.Id == item.Id);
            if (matchedTeam is not null)
            {
                item.DailyUpdateChannel = mapper.MapTo(matchedTeam.DailyUpdateChannel);
            }

            return item;
        }).ToList();

        var asyncBlocks = await Task.WhenAll(
            GetToolbarAsync(workspace, workspaceMember, commonPageContext.PageContext, cancellationToken),
            teamComponents.GetTeamCardsAsync(
                workspaceMember,
                teams,
                commonPageContext.PageContext,
                cancellationToken));

        ICollection<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetTeamsSearchCriteriaAndPaginationBlocks(teamConnection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.PublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = TeamsCallback,
                Blocks = blocks
                    .SelectMany(item => item.Count == 0 ? item : item.Concat([new DividerBlock()]))
                    .SkipLast(1)
                    .ToList(),
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
            .RegisterBlockActionHandler<ButtonAction, TeamsPage>(PreviousPageTeams)
            .RegisterBlockActionHandler<ButtonAction, TeamsPage>(TeamActionTypes.SetAsDefaultTeam)
            .RegisterBlockActionHandler<ButtonAction, TeamsPage>(TeamActionTypes.ClearDefaultTeam);

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Teams*".ToMarkdown() }
    ];

    private async Task<ICollection<Block>> GetToolbarAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext);
        var addTeamButton =
            await teamComponents.GetAddTeamButtonAsync(workspace, workspaceMember, pageContext, cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(addTeamButton)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }

    private async Task<TeamConnection> GetPaginatedTeamsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.TeamsPage);
        var getPaginatedTeamsInput = new GetPaginatedTeamsInput
        {
            After = after.ToSafeString(),
            First = first.ToNullInt(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new TeamWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedTeamsInput.OrderBy.AddRange([
            new TeamOrderInput { Direction = OrderDirection.Ascending, Field = TeamOrderField.Name }
        ]);

        return await teamServiceClient.GetPaginatedTeamsAsync(
            getPaginatedTeamsInput,
            teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private static List<Block> GetTeamsSearchCriteriaAndPaginationBlocks(
        TeamConnection teamConnection,
        PageContext pageContext)
    {
        if (teamConnection.Edges.Count == 0)
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
            pageContext.TeamsPage.TeamsPagination.First = TeamsPageSize;
            pageContext.TeamsPage.TeamsPagination.After = null;
            pageContext.TeamsPage.TeamsPagination.Before = null;
            pageContext.TeamsPage.TeamsPagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageTeams,
                Text = Icons.FirstPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.TeamsPage.TeamsPagination.First = null;
            pageContext.TeamsPage.TeamsPagination.After = null;
            pageContext.TeamsPage.TeamsPagination.Before = teamConnection.PageInfo.StartCursor;
            pageContext.TeamsPage.TeamsPagination.Last = TeamsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageTeams,
                Text = Icons.PreviousPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (teamConnection.PageInfo.HasNextPage)
        {
            pageContext.TeamsPage.TeamsPagination.First = TeamsPageSize;
            pageContext.TeamsPage.TeamsPagination.After = teamConnection.PageInfo.EndCursor;
            pageContext.TeamsPage.TeamsPagination.Before = null;
            pageContext.TeamsPage.TeamsPagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageTeams,
                Text = Icons.NextPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.TeamsPage.TeamsPagination.First = null;
            pageContext.TeamsPage.TeamsPagination.After = null;
            pageContext.TeamsPage.TeamsPagination.Before = null;
            pageContext.TeamsPage.TeamsPagination.Last = TeamsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageTeams,
                Text = Icons.LastPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
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
        var team = await teamServiceClient.GetAsync(
            new GetInput { Id = context.TeamId },
            teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var name = new InputBlock
        {
            BlockId = TeamActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = TeamActionTypes.Name, InitialValue = team.Name.ToSafeString()
            },
            Optional = false
        };

        var about = new InputBlock
        {
            BlockId = TeamActionTypes.About,
            Label = "About".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = TeamActionTypes.About, InitialValue = team.About.ToSafeString(), Multiline = true
            },
            Optional = true
        };

        var timezone = new InputBlock
        {
            BlockId = OptionLoaderKeys.TimezoneKey,
            Label = "Timezone".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.TimezoneKey,
                InitialOption =
                    string.IsNullOrWhiteSpace(team.Timezone)
                        ? null
                        : new Option { Text = team.Timezone.ToOptionText(), Value = team.Timezone },
                MinQueryLength = 3
            },
            Optional = false
        };

        var teamEntity = await repositoryFactory.TeamRepository
            .Query(new Specification<Team> { Criteria = query => query.Id == team.Id }
                .AddInclude(query => query.DailyUpdateChannel))
            .FirstOrDefaultAsync(cancellationToken);

        var updateChannel = new InputBlock
        {
            BlockId = TeamActionTypes.SlackUpdateChannel,
            Label = "Slack update channel".ToPlainText(),
            Element = new ChannelSelectMenu
            {
                ActionId = TeamActionTypes.SlackUpdateChannel,
                InitialChannel = teamEntity?.DailyUpdateChannel?.Id
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
                InitialOptions =
                    team.Members.Select(item =>
                    {
                        var customer = mapper.MapTo(item.Customer);
                        return new Option
                        {
                            Text = customer.GetCustomerName().ToOptionText(),
                            Value = $"{item.OrganizationMember.Id}{Global.OptionLoaderValueSeparator}{customer.Id}"
                        };
                    }).ToList(),
                MinQueryLength = 0
            },
            Optional = false
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = TeamCallbackTypes.EditTeam,
                Title = "Edit Team",
                Close = "Cancel",
                Submit = "Save",
                Blocks =
                [
                    name, about, timezone, updateChannel, organizationMembers
                ],
                PrivateMetadata = context.Serialize()
            });
    }

    private async Task OpenRemoveTeamDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveTeamContext context,
        CancellationToken cancellationToken)
    {
        var team = await teamServiceClient.GetAsync(
            new GetInput { Id = context.TeamId },
            teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var confirmationMessage = new SectionBlock
        {
            Text = $"Are you sure you want to remove this team {team.Name.ToSafeString()}?"
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = TeamCallbackTypes.RemoveTeam,
                Title = "Remove Team",
                Close = "No",
                Submit = "Yes",
                Blocks =
                    [confirmationMessage],
                PrivateMetadata = context.Serialize()
            });
    }

    private async Task SetAsDefaultTeamAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        SetAsDefaultTeamContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.AddDefaultTeamAsync(
            new AddDefaultTeamInput { TeamId = context.TeamId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }

    private async Task ClearDefaultTeamAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ClearDefaultTeamContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.RemoveDefaultTeamAsync(
            new RemoveDefaultTeamInput { TeamId = context.TeamId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }
}
