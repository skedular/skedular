using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Random;
using Slack.Api.Components;
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
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;
using BookingType = Api.Shared.Services.Models.BookingType;
using Customer = Slack.Shared.Models.Customer;
using Organization = Slack.Shared.Models.Organization;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class InstantAddBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingComponents bookingComponents,
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
        var context = InstantAddBookingContext.Deserialize(action.Value);

        if (context.InitiationSource != InitiationSource.App)
        {
            if (string.IsNullOrEmpty(context.CustomerId))
            {
                context.CustomerId = customerId;
            }

            var bookingConnection = await bookingService.GetPaginatedBookingsAsync(
                workspaceMember.Id,
                new BookingSearchCriteria(
                    null,
                    context.From,
                    null,
                    context.Until,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    [],
                    true,
                    null,
                    [workspace.Organization.Id],
                    string.IsNullOrWhiteSpace(context.LocationId) ? [] : [context.LocationId],
                    string.IsNullOrWhiteSpace(context.TeamId) ? [] : [context.TeamId],
                    []),
                string.Empty,
                1,
                string.Empty,
                ((int?)null).ToNullInt(),
                cancellationToken);

            var slackApiClient = workspace.GetApiClient();
            if (bookingConnection.TotalCount == 0)
            {
                var booking = await bookingService.AddAsync(
                    workspaceMember.Id,
                    new Shared.Models.Booking
                    {
                        Id = randomHelper.Generate(),
                        From = context.From,
                        Until = context.Until,
                        Type = BookingType.WorkingFromOffice,
                        InvolvedCustomers = [new Customer { Id = customerId }],
                        InvolvedOrganizations = [new Organization { Id = workspace.Organization.Id }],
                        InvolvedTeams =
                            string.IsNullOrWhiteSpace(context.TeamId.ToSafeString())
                                ? []
                                : [new Shared.Models.Team { Id = context.TeamId.ToSafeString() }]
                    },
                    cancellationToken);

                var blocks = new List<Block> { new SectionBlock { Text = "Your booking on is now confirmed.".ToMarkdown() } };
                var bookingCardBlocks = bookingComponents.GetBookingCard(workspace, booking, [], customerId, false, context.PageContext);
                blocks.AddRange(bookingCardBlocks);
                var message = new Message { Channel = request.Channel.Id, Blocks = blocks };
                await slackApiClient.Chat.PostEphemeral(workspaceMember.Id, message, cancellationToken);
            }
            else
            {
                var blocks = new List<Block> { new SectionBlock { Text = "Found a matching booking".ToMarkdown() } };
                var booking = bookingConnection.Edges.Select(item => item.Node).First();
                var bookingCardBlocks = bookingComponents.GetBookingCard(workspace, booking, [], customerId, false, context.PageContext);
                blocks.AddRange(bookingCardBlocks);

                var message = new Message { Channel = request.Channel.Id, Blocks = blocks };
                await slackApiClient.Chat.PostEphemeral(workspaceMember.Id, message, cancellationToken);
            }
        }
        else
        {
            _ = await bookingService.AddAsync(
                workspaceMember.Id,
                new Shared.Models.Booking
                {
                    Id = randomHelper.Generate(),
                    From = context.From,
                    Until = context.Until,
                    Type = BookingType.WorkingFromOffice,
                    InvolvedCustomers = [new Customer { Id = customerId }],
                    InvolvedOrganizations = [new Organization { Id = workspace.Organization.Id }],
                    InvolvedTeams =
                        string.IsNullOrWhiteSpace(context.TeamId.ToSafeString())
                            ? []
                            : [new Shared.Models.Team { Id = context.TeamId.ToSafeString() }]
                },
                cancellationToken);

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
