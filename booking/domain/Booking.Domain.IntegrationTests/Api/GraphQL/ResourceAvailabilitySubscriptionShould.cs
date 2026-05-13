using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class ResourceAvailabilitySubscriptionShould(
    IOnResourceAvailabilityChangedSubscription onResourceAvailabilityChangedSubscription,
    IResourceDayViewsQuery resourceDayViewsQuery,
    InfrastructureTestService.InfrastructureTestServiceClient infrastructureTestClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Receive_Initial_Snapshot_On_Subscribe(CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);

        var filter = new ResourceAvailabilityFilterInput { Date = DateOnly.FromDateTime(DateTime.UtcNow), OrganizationCustomDomain = "test-org" };

        var queryResult = await resourceDayViewsQuery.ExecuteAsync(filter, [], cancellationToken);
        queryResult.Data.ShouldNotBeNull();
        var subscriptionKey = queryResult.Data.ResourceDayViews.SubscriptionKey;
        subscriptionKey.ShouldNotBeNullOrWhiteSpace();

        IOnResourceAvailabilityChangedResult? received = null;
        var tcs = new TaskCompletionSource<IOnResourceAvailabilityChangedResult?>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var subscription = onResourceAvailabilityChangedSubscription
            .Watch(subscriptionKey, filter)
            .Subscribe(
                result =>
                {
                    if (!tcs.Task.IsCompleted)
                    {
                        tcs.SetResult(result.Data);
                    }
                },
                error => tcs.TrySetException(error));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        cts.Token.Register(() => tcs.TrySetCanceled());

        received = await tcs.Task;

        received.ShouldNotBeNull();
        received.ResourceAvailability.ShouldNotBeNull();
    }
}
