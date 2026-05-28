using Api.Shared.Grpc.Skedular.Core.Graphql.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using HotChocolate.Subscriptions;

namespace Core.Api.Grpc;

public class CoreGraphqlGrpcService(
    CoreConfiguration coreConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITopicEventSender topicEventSender) : CoreGraphqlService.CoreGraphqlServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(coreConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);

        return new RaiseGraphqlChangeResponse();
    }
}
