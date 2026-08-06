using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Graphql.V1;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;

namespace Booking.Jobs.Services;

public class GraphQlTopicEventSender(
    BookingConfiguration bookingConfiguration,
    BookingGraphqlService.BookingGraphqlServiceClient bookingServiceClient)
    : IGraphQlTopicEventSender
{
    public async Task RaiseGraphqlChangeAsync(string topicName, string id, CancellationToken cancellationToken) =>
        await bookingServiceClient.RaiseGraphqlChangeAsync(
            new RaiseGraphqlChangeInput
            {
                TopicName = topicName,
                Id = id,
            },
            bookingConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
}
