using Enterprise.Shared.Exceptions;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Pages;

public interface IPageNavigator
{
    Task BackAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext context,
        string? hash,
        CancellationToken cancellationToken);
}

public class PageNavigator(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    IHomePage homePage,
    IBookingsPage bookingsPage,
    ILocationsPage locationsPage,
    ITeamsPage teamsPage,
    IZonesPage zonesPage,
    IDesksPage desksPage,
    ISettingsPage settingsPage,
    IBillingPage billingPage,
    IWorkspaceMemberService workspaceMemberService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper) :
    IPageNavigator,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<ButtonAction>
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
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

        await BackAsync(
            workspace,
            workspaceMember,
            CommonPageContext.Deserialize(action.Value),
            request.View.Hash,
            cancellationToken);
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfiguration.EnableAsyncMode)
        {
            asyncPageRenderingService.ButtonActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task BackAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        var page = context.PageContext.PopLastVisitedPage();
        if (page is null)
        {
            await homePage.RenderWithContextAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                hash,
                cancellationToken);

            return;
        }

        switch (page)
        {
            case PageType.Home:
                await homePage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            case PageType.Bookings:
                await bookingsPage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            case PageType.Locations:
                await locationsPage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            case PageType.Teams:
                await teamsPage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            case PageType.Zones:
                await zonesPage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            case PageType.Desks:
                await desksPage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            case PageType.Settings:
                await settingsPage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            case PageType.Billing:
                await billingPage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;

            default:
                await homePage.RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    hash,
                    cancellationToken);
                break;
        }
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<ButtonAction, PageNavigator>(CommonActionTypes.Back);
}
