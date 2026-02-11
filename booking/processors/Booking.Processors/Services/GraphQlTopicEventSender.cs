using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;

namespace Booking.Processors.Services;

public class GraphQlTopicEventSender(BookingConfiguration bookingConfiguration, BookingService.BookingServiceClient bookingServiceClient)
    : IGraphQlTopicEventSender
{
    public async Task RaiseGraphqlChangeAsync(string topicName, string id, CancellationToken cancellationToken) =>
        await bookingServiceClient.RaiseGraphqlChangeAsync(
            new RaiseGraphqlChangeInput { TopicName = topicName, Id = id },
            bookingConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
}
