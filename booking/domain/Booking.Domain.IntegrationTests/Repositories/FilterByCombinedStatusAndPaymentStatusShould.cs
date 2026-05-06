using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Api.Shared.Services.Models;
using Booking.Domain.IntegrationTests.Fixtures;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Pagination;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class FilterByCombinedStatusAndPaymentStatusShould(
    IRepositoryFactory repositoryFactory,
    InfrastructureTestService.InfrastructureTestServiceClient infrastructureTestClient)
{
    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Return_Only_Intersection_When_Both_Filters_Applied(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var searchCriteria = new MarketplaceBookingSubscriptionSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, scenario.Organization.Id, null, [], [],
            [MarketplaceBookingSubscriptionStatus.Active],
            [PaymentStatus.Pending]);

        var (_, edges, totalCount) = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                new PaginationInputParam(null, null, null, null),
                searchCriteria,
                [],
                null,
                cancellationToken);

        totalCount.ShouldBe(1);
        edges.Single().Node.Status.ShouldBe(MarketplaceBookingSubscriptionStatusConstants.Active);
        edges.Single().Node.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.Pending);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Return_All_When_Both_Filters_Empty(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var searchCriteria = new MarketplaceBookingSubscriptionSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, scenario.Organization.Id, null, [], [], [], []);

        var (_, _, totalCount) = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                new PaginationInputParam(null, null, null, null),
                searchCriteria,
                [],
                null,
                cancellationToken);

        totalCount.ShouldBe(4);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Return_Payment_Filtered_Results_When_Only_Payment_Filter_Cleared(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var searchCriteria = new MarketplaceBookingSubscriptionSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, scenario.Organization.Id, null, [], [],
            [MarketplaceBookingSubscriptionStatus.Active],
            []);

        var (_, _, totalCount) = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                new PaginationInputParam(null, null, null, null),
                searchCriteria,
                [],
                null,
                cancellationToken);

        totalCount.ShouldBe(2);
    }
}
