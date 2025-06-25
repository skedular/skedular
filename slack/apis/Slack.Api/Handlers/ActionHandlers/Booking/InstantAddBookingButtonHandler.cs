using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Google.Protobuf.WellKnownTypes;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;
using BookingService = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class InstantAddBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingComponents bookingComponents,
    IRandomHelper randomHelper,
    IMapper mapper,
    IPageNavigator pageNavigator) : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>
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
        var context = InstantAddBookingContext.Deserialize(action.Value);

        if (context.InitiationSource != InitiationSource.App)
        {
            if (string.IsNullOrEmpty(context.CustomerId))
            {
                context.CustomerId = customerId;
            }

            var getPaginatedBookingsInput = new GetPaginatedBookingsInput
            {
                After = string.Empty,
                First = 1,
                Before = string.Empty,
                Last = -1,
                Where = new BookingWhereInput { FromGte = context.From.ToTimestamp(), FromLte = context.Until.ToTimestamp(), IncludeMineOnly = true }
            };
            getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
            if (!string.IsNullOrWhiteSpace(context.LocationId))
            {
                getPaginatedBookingsInput.Where.LocationIds.Add(context.LocationId);
            }

            if (!string.IsNullOrWhiteSpace(context.TeamId))
            {
                getPaginatedBookingsInput.Where.TeamIds.Add(context.TeamId);
            }

            getPaginatedBookingsInput.OrderBy.AddRange([
                new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From }
            ]);
            var bookingConnection = await bookingServiceClient.GetPaginatedBookingsAsync(
                getPaginatedBookingsInput,
                bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken);

            var slackApiClient = workspace.GetApiClient();
            if (bookingConnection.TotalCount == 0)
            {
                var addInput = new AddInput
                {
                    Id = randomHelper.Generate(),
                    From = context.From.ToTimestamp(),
                    Until = context.Until.ToTimestamp(),
                    Type = BookingType.WorkingFromOffice
                };

                addInput.CustomerIds.Add(customerId);
                addInput.OrganizationIds.Add(workspace.Organization.Id);
                if (!string.IsNullOrWhiteSpace(context.TeamId.ToSafeString()))
                {
                    addInput.TeamIds.Add(context.TeamId);
                }

                var booking = mapper.MapTo(
                    await bookingServiceClient.AddAsync(
                        addInput,
                        bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                        cancellationToken: cancellationToken));

                var blocks = new List<Block> { new SectionBlock { Text = "Your booking on is now confirmed.".ToMarkdown() } };
                var bookingCardBlocks = bookingComponents.GetBookingCard(workspace, booking, [], customerId, false, context.PageContext);
                blocks.AddRange(bookingCardBlocks);
                var message = new Message { Channel = request.Channel.Id, Blocks = blocks };
                await slackApiClient.Chat.PostEphemeral(workspaceMember.Id, message, cancellationToken);
            }
            else
            {
                var blocks = new List<Block> { new SectionBlock { Text = "Found a matching booking".ToMarkdown() } };
                var booking = bookingConnection.Edges.Select(item => mapper.MapTo(item.Node)).First();
                var bookingCardBlocks = bookingComponents.GetBookingCard(workspace, booking, [], customerId, false, context.PageContext);
                blocks.AddRange(bookingCardBlocks);

                var message = new Message { Channel = request.Channel.Id, Blocks = blocks };
                await slackApiClient.Chat.PostEphemeral(workspaceMember.Id, message, cancellationToken);
            }
        }
        else
        {
            var addInput = new AddInput
            {
                Id = randomHelper.Generate(),
                From = context.From.ToTimestamp(),
                Until = context.Until.ToTimestamp(),
                Type = BookingType.WorkingFromOffice
            };

            addInput.CustomerIds.Add(customerId);
            addInput.OrganizationIds.Add(workspace.Organization.Id);
            if (!string.IsNullOrWhiteSpace(context.TeamId.ToSafeString()))
            {
                addInput.TeamIds.Add(context.TeamId);
            }

            await bookingServiceClient.AddAsync(
                addInput,
                bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken);

            await pageNavigator.BackAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                request.View.Hash,
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
}
