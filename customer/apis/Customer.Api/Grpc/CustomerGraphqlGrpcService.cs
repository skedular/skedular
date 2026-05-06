using Api.Shared.Grpc.Skedular.Customer.Graphql.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using HotChocolate.Subscriptions;

namespace Customer.Api.Grpc;

public class CustomerGraphqlGrpcService(
    CustomerConfiguration customerConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITopicEventSender topicEventSender) : CustomerGraphqlService.CustomerGraphqlServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);

        return new RaiseGraphqlChangeResponse();
    }
}
