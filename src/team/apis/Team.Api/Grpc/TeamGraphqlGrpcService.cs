using Api.Shared.Grpc.Skedular.Team.Graphql.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using HotChocolate.Subscriptions;

namespace Team.Api.Grpc;

public class TeamGraphqlGrpcService(
    TeamConfiguration teamConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITopicEventSender topicEventSender,
    ILogger<TeamGraphqlGrpcService> logger) : TeamGraphqlService.TeamGraphqlServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(teamConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);
        logger.LogInformation("gRPC RaiseGraphqlChange sent for topic {TopicName} and id {EntityId}", request.TopicName, request.Id);

        return new RaiseGraphqlChangeResponse();
    }
}
