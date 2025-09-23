using Api.Shared.Services;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;
using BookingType = Api.Shared.Services.Models.BookingType;
using Customer = Slack.Shared.Models.Customer;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class JoinBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IRandomHelper randomHelper,
    IMapper mapper,
    IPageNavigator pageNavigator,
    IBookingService bookingService) : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, customerId) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = JoinBookingContext.Deserialize(action.Value);
        var existingBooking = await bookingService.GetAsync(workspaceMember.Id, context.BookingId, cancellationToken);
        var booking = new Shared.Models.Booking
        {
            Id = randomHelper.Generate(),
            From = existingBooking.From,
            Until = existingBooking.Until,
            Type = BookingType.WorkingFromOffice,
            InvolvedCustomers = [new Customer { Id = customerId }],
            InvolvedOrganizations = [new Organization { Id = workspace.Organization.Id }],
            InvolvedTeams = existingBooking.InvolvedTeams.Select(item => new Shared.Models.Team { Id = item.Id }).ToList()
        };

        await bookingService.AddAsync(workspaceMember.Id, booking, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            request.View.Hash,
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
}
