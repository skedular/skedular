using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Api.Shared.Services.Models;
using Booking.Domain.IntegrationTests.Fixtures;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Pagination;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class FilterBySubscriptionStatusShould(
    IRepositoryFactory repositoryFactory,
    InfrastructureTestService.InfrastructureTestServiceClient infrastructureTestClient)
{
    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Persist_And_Reload_Weekly_Selected_Days(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        scenario.ActivePending.Subscription.WeeklySelectedDays = [DayOfWeekConstants.Tuesday, DayOfWeekConstants.Thursday];
        repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(scenario.ActivePending.Subscription);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var reloaded = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdUntrackedAsync(
            scenario.ActivePending.Subscription.Id,
            cancellationToken);

        reloaded.ShouldNotBeNull().WeeklySelectedDays.ShouldBe([DayOfWeekConstants.Tuesday, DayOfWeekConstants.Thursday]);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Return_Only_Active_Subscriptions_When_Active_Status_Filter_Applied(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var searchCriteria = new MarketplaceBookingSubscriptionSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, scenario.Organization.Id, null, [], [], [MarketplaceBookingSubscriptionStatus.Active], []);

        var (_, edges, totalCount) = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                new PaginationInputParam(null, null, null, null),
                searchCriteria,
                [],
                null,
                cancellationToken);

        totalCount.ShouldBe(2);
        edges.ShouldAllBe(e => e.Node.Status == MarketplaceBookingSubscriptionStatusConstants.Active);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Return_All_Subscriptions_When_Empty_Statuses_Filter_Applied(
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
    public async Task Return_Matching_Subscriptions_When_Multiple_Statuses_Filter_Applied(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var searchCriteria = new MarketplaceBookingSubscriptionSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, scenario.Organization.Id, null, [], [],
            [MarketplaceBookingSubscriptionStatus.Active, MarketplaceBookingSubscriptionStatus.Cancelled],
            []);

        var (_, _, totalCount) = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                new PaginationInputParam(null, null, null, null),
                searchCriteria,
                [],
                null,
                cancellationToken);

        totalCount.ShouldBe(4);
    }
}
