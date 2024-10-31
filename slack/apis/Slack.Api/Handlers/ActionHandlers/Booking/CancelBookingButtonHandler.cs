using Api.Shared.Services.Grpc.UnityHub.Booking.V1;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;
using BookingService = Api.Shared.Services.Grpc.UnityHub.Booking.V1.BookingService;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class CancelBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IBookingService bookingService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IPageNavigator pageNavigator) : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, customerId) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                request.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = CancelBookingContext.Deserialize(action.Value);
        var booking = mapper.MapTo(await bookingServiceClient.GetAsync(
            new GetInput { Id = context.BookingId },
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken));

        if (booking.Customer.Id != customerId)
        {
            var permissions =
                await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember,
                    cancellationToken);
            if (!permissions.CanDeleteBookingOnBehalf)
            {
                throw new Unauthorized();
            }
        }

        await bookingServiceClient.DeleteAsync(
            new DeleteInput { Id = context.BookingId },
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
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
}
