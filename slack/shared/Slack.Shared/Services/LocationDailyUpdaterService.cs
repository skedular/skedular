using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
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
using Admin_GetInput = Api.Shared.Services.Grpc.Skedular.Location.V1.Admin_GetInput;
using Icons = Slack.Shared.Constants.Icons;
using LocationConfiguration = Slack.Shared.Configurations.LocationConfiguration;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Booking.V1.OrderDirection;

namespace Slack.Shared.Services;

public interface ILocationDailyUpdaterService
{
    Task SendDailyUpdateAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationDailyUpdaterService(
    LocationConfiguration locationConfiguration,
    BookingConfiguration bookingConfiguration,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    LocationService.LocationServiceClient locationServiceClient,
    BookingService.BookingServiceClient bookingServiceClient,
    IBookingComponents bookingComponents,
    IWorkspaceMemberService workspaceMemberService,
    TimeProvider timeProvider) : ILocationDailyUpdaterService
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

        var location = await locationServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = locationId },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(location.OrganizationId))
        {
            return;
        }

        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByOrganizationIdAsync(location.OrganizationId, cancellationToken);
        if (workspaceEntity is null)
        {
            return;
        }

        var workspace = mapper.MapTo(workspaceEntity);
        var convertedNow = TimeZoneInfo.ConvertTime(now, locationEntity.Timezone.ToTimezoneInfo());
        var from = new DateTimeOffset(convertedNow.Year, convertedNow.Month, convertedNow.Day, 0, 0, 0, TimeSpan.Zero)
            .StartOfDay();
        var until = from.EndOfDay();
        var getPaginatedBookingsInput = new Admin_GetPaginatedBookingsInput
        {
            After = string.Empty,
            First = LocationBookingsPageSize,
            Before = string.Empty,
            Last = -1,
            Where = new BookingWhereInput { FromGTE = from.ToTimestamp(), FromLTE = until.ToTimestamp() }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
        getPaginatedBookingsInput.Where.LocationIds.Add(locationId);
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
                var customerEntity = await repositoryFactory.CustomerRepository.GetByIdAsync(booking.Customer.Id, cancellationToken);
                if (customerEntity is null)
                {
                    continue;
                }

                var customer = mapper.MapTo(customerEntity)!;
                blocks.Add(new SectionBlock
                {
                    Text = workspaceMemberService.GetMentionedCustomerNameInSlackFormat(
                        workspace,
                        customer.Identities.Select(item => item.Id).ToList(),
                        customer).ToMarkdownWithIcon(Icons.People)
                });
                blocks.AddRange(bookingComponents.GetDesksLines(booking));
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
                            new PageContext(),
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
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
