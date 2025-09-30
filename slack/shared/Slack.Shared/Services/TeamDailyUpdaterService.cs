using Enterprise.Shared;
using Enterprise.Shared.Time;
using Slack.Shared.Components;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.WebApi;
using Icons = Slack.Shared.Constants.Icons;

namespace Slack.Shared.Services;

public interface ITeamDailyUpdaterService
{
    Task SendDailyUpdateAsync(string locationId, CancellationToken cancellationToken);
}

public class TeamDailyUpdaterService(
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IBookingComponents bookingComponents,
    IWorkspaceMemberService workspaceMemberService,
    TimeProvider timeProvider,
    ITeamService teamService,
    IBookingService bookingService) : ITeamDailyUpdaterService
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

        var team = await teamService.AdminGetAsync(teamId, cancellationToken);
        if (string.IsNullOrWhiteSpace(team.Organization?.Id))
        {
            return;
        }

        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByOrganizationIdAsync(team.Organization!.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            return;
        }

        var workspace = mapper.MapTo(workspaceEntity);
        var convertedNow = TimeZoneInfo.ConvertTime(now, teamEntity.Timezone.ToTimezoneInfo());
        var from = new DateTimeOffset(convertedNow.Year, convertedNow.Month, convertedNow.Day, 0, 0, 0, TimeSpan.Zero)
            .StartOfDay();
        var until = from.EndOfDay();
        var bookingConnection = await bookingService.Admin_GetPaginatedBookingsAsync(
            new BookingSearchCriteria(
                null,
                from,
                null,
                until,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                [],
                false,
                null,
                [workspace.Organization.Id],
                [],
                [teamId],
                []),
            string.Empty,
            TeamBookingsPageSize,
            string.Empty,
            ((int?)null).ToNullInt(),
            cancellationToken);
        var bookings = bookingConnection.Edges.Select(item => item.Node).ToList();
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
                var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsUntrackedAsync(customerIds, cancellationToken);
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
