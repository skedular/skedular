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

public interface ILocationDailyUpdaterService
{
    Task SendDailyUpdateAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationDailyUpdaterService(
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IBookingComponents bookingComponents,
    IWorkspaceMemberService workspaceMemberService,
    TimeProvider timeProvider,
    ILocationService locationService,
    IBookingService bookingService) : ILocationDailyUpdaterService
{
    public async Task SendDailyUpdateAsync(string locationId, CancellationToken cancellationToken)
    {
        const int LocationBookingsPageSize = 5;

        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);
        var locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        if (locationEntity?.DailyUpdateChannel is null ||
            (locationEntity.SlackChannelDailyUpdateLastSentAt is not null &&
             (now - locationEntity.SlackChannelDailyUpdateLastSentAt.Value).TotalHours <= 23))
        {
            return;
        }

        var location = await locationService.AdminGetAsync(locationId, cancellationToken);
        if (string.IsNullOrWhiteSpace(location.Organization?.Id))
        {
            return;
        }

        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByOrganizationIdAsync(location.Organization!.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            return;
        }

        var workspace = mapper.MapTo(workspaceEntity);
        var convertedNow = TimeZoneInfo.ConvertTime(now, locationEntity.Timezone.ToTimezoneInfo());
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
                [locationId],
                [],
                []),
            string.Empty,
            LocationBookingsPageSize,
            string.Empty,
            ((int?)null).ToNullInt(),
            cancellationToken);
        var bookings = bookingConnection.Edges.Select(item => item.Node).ToList();
        var blocks = new List<Block>
        {
            new SectionBlock { Text = "*Who's in today?*".ToMarkdown() },
            new SectionBlock { Text = location.Name.ToSafeString().ToMarkdownWithIcon(Icons.Location) }
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

            if (bookingConnection.TotalCount > LocationBookingsPageSize)
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
                            InitiationSource.LocationDailyUpdateChannel,
                            null,
                            locationId,
                            null)
                        .Serialize()
                }
            ]
        });

        var message = new Message { Channel = locationEntity.DailyUpdateChannel.Id, Blocks = blocks, Text = "Who's in today?" };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Chat.PostMessage(message, cancellationToken);

        locationEntity.SlackChannelDailyUpdateLastSentAt = now;
        repositoryFactory.LocationRepository.Update(locationEntity);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
