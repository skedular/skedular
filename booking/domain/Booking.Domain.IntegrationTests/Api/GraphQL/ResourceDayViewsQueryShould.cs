using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class ResourceDayViewsQueryShould(
    IResourceDayViewsQuery resourceDayViewsQuery,
    InfrastructureTestService.InfrastructureTestServiceClient infrastructureTestClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_List_When_No_Resources_Exist(CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);

        var filter = new ResourceAvailabilityFilterInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow), OrganizationCustomDomain = "test-org", LocationIds = [], Statuses = []
        };

        var result = await resourceDayViewsQuery.ExecuteAsync(filter, [], cancellationToken);

        result.ShouldNotBeNull();
        result.Errors.ShouldBeEmpty();
        result.Data.ShouldNotBeNull();
        result.Data.ResourceDayViews.ShouldNotBeNull();
        result.Data.ResourceDayViews.Items.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Subscription_Key_In_Result(CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);

        var filter = new ResourceAvailabilityFilterInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow), OrganizationCustomDomain = "test-org", LocationIds = [], Statuses = []
        };

        var result = await resourceDayViewsQuery.ExecuteAsync(filter, [], cancellationToken);

        result.ShouldNotBeNull();
        result.Errors.ShouldBeEmpty();
        result.Data.ShouldNotBeNull();
        result.Data.ResourceDayViews.SubscriptionKey.ShouldNotBeNullOrWhiteSpace();
    }
}
