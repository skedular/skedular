using Api.Shared.Grpc.Skedular.Booking.Graphql.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Grpc.Core;

namespace Booking.Api.Grpc;

public class BookingGraphqlGrpcService(
    BookingConfiguration bookingConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IGraphQlTopicEventSender graphQlTopicEventSender) : BookingGraphqlService.BookingGraphqlServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(request.TopicName, request.Id, context.CancellationToken);

        return new RaiseGraphqlChangeResponse();
    }
}
