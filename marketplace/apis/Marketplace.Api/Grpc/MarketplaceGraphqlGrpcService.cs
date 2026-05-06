using Api.Shared.Grpc.Skedular.Marketplace.Graphql.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using HotChocolate.Subscriptions;

namespace Marketplace.Api.Grpc;

public class MarketplaceGraphqlGrpcService(
    MarketplaceConfiguration marketplaceConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITopicEventSender topicEventSender) : MarketplaceGraphqlService.MarketplaceGraphqlServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(marketplaceConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);

        return new RaiseGraphqlChangeResponse();
    }
}
