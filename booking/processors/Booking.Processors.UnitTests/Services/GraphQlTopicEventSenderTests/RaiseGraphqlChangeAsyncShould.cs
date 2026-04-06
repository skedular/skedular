using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Booking.Processors.Services;
using Enterprise.Shared.Grpc;
using Grpc.Core;

namespace Booking.Processors.UnitTests.Services.GraphQlTopicEventSenderTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RaiseGraphqlChangeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Forward_The_Request_To_The_Booking_Grpc_Service(
        [Frozen] BookingConfiguration bookingConfiguration,
        CallInvoker callInvoker,
        string topicName,
        string id,
        string apiKey,
        CancellationToken cancellationToken)
    {
        bookingConfiguration.ApiKey = apiKey;
        var sut = new GraphQlTopicEventSender(bookingConfiguration, new BookingService.BookingServiceClient(callInvoker));

        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<RaiseGraphqlChangeInput, RaiseGraphqlChangeResponse>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == apiKey)),
                A<RaiseGraphqlChangeInput>.That.Matches(input => input.TopicName == topicName && input.Id == id)))
            .Returns(CreateResponse());

        await sut.RaiseGraphqlChangeAsync(topicName, id, cancellationToken);

        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<RaiseGraphqlChangeInput, RaiseGraphqlChangeResponse>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == apiKey)),
                A<RaiseGraphqlChangeInput>.That.Matches(input => input.TopicName == topicName && input.Id == id)))
            .MustHaveHappenedOnceExactly();
    }

    private static AsyncUnaryCall<RaiseGraphqlChangeResponse> CreateResponse() =>
        new(
            Task.FromResult(new RaiseGraphqlChangeResponse()),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
}
