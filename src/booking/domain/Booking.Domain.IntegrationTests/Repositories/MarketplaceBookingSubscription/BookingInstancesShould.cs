using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Booking.Domain.IntegrationTests.Fixtures;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Pagination;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;

namespace Booking.Domain.IntegrationTests.Repositories.MarketplaceBookingSubscription;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class BookingInstancesShould(
    IRepositoryFactory repositoryFactory,
    InfrastructureTestService.InfrastructureTestServiceClient infrastructureTestClient)
{
    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Filter_And_Page_Only_The_Subscription_Recurring_Instances(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var firstStart = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(1), TimeSpan.Zero);
        var first = AddInstance(scenario.ActivePending.Subscription, firstStart);
        var second = AddInstance(scenario.ActivePending.Subscription, firstStart.AddDays(1));
        var otherSubscription = AddInstance(scenario.ActiveConfirmed.Subscription, firstStart.AddDays(2));
        repositoryFactory.RecurringBookingRepository.Add(first);
        repositoryFactory.RecurringBookingRepository.Add(second);
        repositoryFactory.RecurringBookingRepository.Add(otherSubscription);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var pageOne = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetPaginatedBookingInstancesUntrackedAsync(
                scenario.ActivePending.Subscription.Id,
                new PaginationInputParam(null, 1, null, null),
                firstStart,
                firstStart.AddDays(2),
                cancellationToken);

        pageOne.Item3.ShouldBe(2);
        pageOne.Item2.ShouldHaveSingleItem().Node.Id.ShouldBe(first.Id);
        pageOne.Item1.HasNextPage.ShouldBeTrue();

        var pageTwo = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetPaginatedBookingInstancesUntrackedAsync(
                scenario.ActivePending.Subscription.Id,
                new PaginationInputParam(pageOne.Item1.EndCursor, 1, null, null),
                firstStart,
                firstStart.AddDays(2),
                cancellationToken);

        pageTwo.Item2.ShouldHaveSingleItem().Node.Id.ShouldBe(second.Id);
    }

    private static RecurringBooking AddInstance(MarketplaceBookingSubscriptionEntity subscription, DateTimeOffset from) => new()
    {
        Id = Guid.CreateVersion7().ToString(),
        From = from,
        Until = from.AddHours(1),
        StartDate = from,
        Category = "WORKING_FROM_COWORKING_SPACE",
        Channel = "MARKETPLACE",
        Frequency = "DAILY",
        Interval = 1,
        EndType = "DATE",
        EndDate = from.AddDays(1),
        MarketplaceBookingSubscription = subscription,
        InvolvedOrganizations = subscription.InvolvedOrganizations,
    };
}
