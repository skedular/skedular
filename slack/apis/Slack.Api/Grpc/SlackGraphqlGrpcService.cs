using Api.Shared.Grpc.Skedular.Slack.Graphql.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using HotChocolate.Subscriptions;

namespace Slack.Api.Grpc;

public class SlackGraphqlGrpcService(
    SlackConfiguration slackConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITopicEventSender topicEventSender) : SlackGraphqlService.SlackGraphqlServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(slackConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);

        return new RaiseGraphqlChangeResponse();
    }
}
