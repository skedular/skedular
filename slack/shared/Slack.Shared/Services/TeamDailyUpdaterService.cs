using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Slack.Shared.Components;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Mappers;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.WebApi;
using Icons = Slack.Shared.Constants.Icons;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Booking.V1.OrderDirection;
using TeamConfiguration = Slack.Shared.Configurations.TeamConfiguration;

namespace Slack.Shared.Services;

public interface ITeamDailyUpdaterService
{
    Task SendDailyUpdateAsync(string locationId, CancellationToken cancellationToken);
}

public class TeamDailyUpdaterService(
    TeamConfiguration teamConfiguration,
    BookingConfiguration bookingConfiguration,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    TeamService.TeamServiceClient teamServiceClient,
    BookingService.BookingServiceClient bookingServiceClient,
    IBookingComponents bookingComponents,
    IWorkspaceMemberService workspaceMemberService,
    TimeProvider timeProvider) : ITeamDailyUpdaterService
{
    public async Task SendDailyUpdateAsync(string teamId, CancellationToken cancellationToken)
    {
        const int TeamBookingsPageSize = 5;

        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);
        var teamEntity = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        if (teamEntity?.DailyUpdateChannel is null ||
            (teamEntity.SlackChannelDailyUpdateLastSentAt is not null &&
             (now - teamEntity.SlackChannelDailyUpdateLastSentAt.Value).TotalHours <= 23))
        {
            return;
        }

        var team = await teamServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = teamId },
            teamConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(team.OrganizationId))
        {
            return;
        }

        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByOrganizationIdAsync(team.OrganizationId, cancellationToken);
        if (workspaceEntity is null)
        {
            return;
        }

        var workspace = mapper.MapTo(workspaceEntity);
        var convertedNow = TimeZoneInfo.ConvertTime(now, teamEntity.Timezone.ToTimezoneInfo());
        var from = new DateTimeOffset(convertedNow.Year, convertedNow.Month, convertedNow.Day, 0, 0, 0, TimeSpan.Zero)
            .StartOfDay();
        var until = from.EndOfDay();
        var getPaginatedBookingsInput = new Admin_GetPaginatedBookingsInput
        {
            After = string.Empty,
            First = TeamBookingsPageSize,
            Before = string.Empty,
            Last = -1,
            Where = new BookingWhereInput { FromGte = from.ToTimestamp(), FromLte = until.ToTimestamp() }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
        getPaginatedBookingsInput.Where.TeamIds.Add(teamId);
        getPaginatedBookingsInput.OrderBy.AddRange([
            new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From }
        ]);
        var bookingConnection = await bookingServiceClient.Admin_GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var bookings = bookingConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var blocks = new List<Block>
        {
            new SectionBlock { Text = "*Who's in today?*".ToMarkdown() },
            new SectionBlock { Text = team.Name.ToSafeString().ToMarkdownWithIcon(Icons.Team) }
        };

        if (bookings.Count == 0)
        {
            blocks.Add(new SectionBlock { Text = "*No one has joined yet, be the first*".ToMarkdown() });
        }
        else
        {
            foreach (var booking in bookings)
            {
                var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
                var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, cancellationToken);
                if (customerEntities.Count == 0)
                {
                    continue;
                }

                foreach (var customerEntity in customerEntities)
                {
                    var customer = mapper.MapTo(customerEntity)!;
                    blocks.Add(new SectionBlock
                    {
                        Text = workspaceMemberService.GetMentionedCustomerNameInSlackFormat(
                            workspace,
                            customer.Identities.Select(item => item.Id).ToList(),
                            customer).ToMarkdownWithIcon(Icons.People)
                    });
                    blocks.AddRange(bookingComponents.GetResourcesLines(booking));
                }
            }

            if (bookingConnection.TotalCount > TeamBookingsPageSize)
            {
                blocks.Add(new SectionBlock { Text = "*To see other bookings, check the Skedular application in Slack*".ToMarkdown() });
            }

            blocks.Add(new DividerBlock());
        }

        blocks.Add(new ActionsBlock
        {
            Elements =
            [
                new Button
                {
                    ActionId = BookingActionTypes.InstantAddBooking,
                    Text = "Join".ToPlainTextWithIcon(Icons.Join),
                    Value = new InstantAddBookingContext(
                            PageContext.New(),
                            from,
                            until,
                            InitiationSource.TeamDailyUpdateChannel,
                            null,
                            null,
                            teamId)
                        .Serialize()
                }
            ]
        });

        var message = new Message { Channel = teamEntity.DailyUpdateChannel.Id, Blocks = blocks, Text = "Who's in today?" };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Chat.PostMessage(message, cancellationToken);

        teamEntity.SlackChannelDailyUpdateLastSentAt = now;
        repositoryFactory.TeamRepository.Update(teamEntity);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
