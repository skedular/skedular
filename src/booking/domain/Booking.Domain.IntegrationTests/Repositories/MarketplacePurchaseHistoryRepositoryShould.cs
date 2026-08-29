using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Api.Shared.Services.Models;
using Booking.Domain.IntegrationTests.Fixtures;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Pagination;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplacePurchaseHistoryRepositoryShould(
    IRepositoryFactory repositoryFactory,
    InfrastructureTestService.InfrastructureTestServiceClient infrastructureTestClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_No_Retained_Purchases_When_Organization_Has_None(CancellationToken cancellationToken)
    {
        var result = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetPaginatedRowsAsync(
            new PaginationInputParam(null, 50, null, null),
            new MarketplacePurchaseHistorySearchCriteria("organization-with-no-history", null, null),
            null,
            cancellationToken);

        result.Item2.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Return_All_Retained_Subscription_Roots_Without_Starting_Workflows(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var result = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetPaginatedRowsAsync(
            new PaginationInputParam(null, 50, null, null),
            new MarketplacePurchaseHistorySearchCriteria(scenario.Organization.CustomDomain, null, null,
                [MarketplacePurchaseSourceType.Subscription]),
            null,
            cancellationToken);

        result.Item3.ShouldBe(4);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Exclude_Subscription_Roots_From_Standalone_Bookings_And_Retain_Deleted_Subscriptions(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        scenario.CancelledConfirmed.Subscription.DeletedAt = TimeProvider.System.GetUtcNow();
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetPaginatedRowsAsync(
            new PaginationInputParam(null, 50, null, null),
            new MarketplacePurchaseHistorySearchCriteria(scenario.Organization.CustomDomain, null, null),
            null,
            cancellationToken);

        result.Item2.ShouldContain(item => item.Node.Id == scenario.CancelledConfirmed.Subscription.Id);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Scope_History_To_The_Requested_Organization_Domain(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var result = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetPaginatedRowsAsync(
            new PaginationInputParam(null, 50, null, null),
            new MarketplacePurchaseHistorySearchCriteria("different-organization.example", null, null),
            null,
            cancellationToken);

        result.Item2.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Return_Filtered_History_With_Stable_Keyset_Pages(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var criteria = new MarketplacePurchaseHistorySearchCriteria(
            scenario.Organization.CustomDomain,
            null,
            null,
            [MarketplacePurchaseSourceType.Subscription],
            [MarketplacePurchaseLifecycleState.Active]);
        var first = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetPaginatedRowsAsync(
            new PaginationInputParam(null, 2, null, null), criteria, null, cancellationToken);

        first.Item2.Count.ShouldBeLessThanOrEqualTo(2);
        first.Item3.ShouldBeGreaterThanOrEqualTo(first.Item2.Count);
        if (first.Item1.HasNextPage)
        {
            var second = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetPaginatedRowsAsync(
                new PaginationInputParam(first.Item1.EndCursor, 2, null, null), criteria, null, cancellationToken);
            second.Item2.Select(edge => edge.Node.Id).Intersect(first.Item2.Select(edge => edge.Node.Id)).ShouldBeEmpty();
        }

        first.Item2.ShouldAllBe(edge => edge.Node.SourceType == MarketplacePurchaseSourceType.Subscription);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Refresh_Subscription_Financial_Fields_From_Its_Current_Billed_Cycle(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var subscription = scenario.ActiveConfirmed.Subscription;
        var template = scenario.ActiveConfirmed.MarketplaceBooking;
        template.PaymentStatus = PaymentStatusConstants.NotSet;
        template.TotalAmount = null;
        template.Currency = null;

        var now = TimeProvider.System.GetUtcNow();
        var recurringBooking = repositoryFactory.RecurringBookingRepository.Add(new RecurringBooking
        {
            Id = Guid.CreateVersion7().ToString(),
            From = now.AddHours(-1),
            Until = now.AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Frequency = BookingFrequency.Daily.ToBookingFrequency(),
            Interval = 1,
            EndType = RecurringBookingEndType.UntilDate.ToRecurringBookingEndType(),
            StartDate = now.AddDays(-1),
            EndDate = now.AddDays(1),
            MarketplaceBookingSubscription = subscription,
        });
        var billedMarketplaceBooking = repositoryFactory.MarketplaceBookingRepository.Add(new MarketplaceBooking
        {
            Id = Guid.CreateVersion7().ToString(),
            PaymentStatus = PaymentStatusConstants.Confirmed,
            IsPaymentRequired = true,
            Quantity = 1,
            ProductPricing = template.ProductPricing,
            PaymentMethod = template.PaymentMethod,
            PaymentExpiry = now.AddDays(1),
            TotalAmountExcludeTax = 25m,
            TotalAmount = 25m,
            Currency = Currency.Nzd.ToCurrency(),
            InvoiceEmailList = [],
            BillingMode = template.BillingMode,
            // The seeder replaces the scenario graph's product-version reference with
            // the tracked database instance. Reuse that instance here; attaching the
            // original fixture object would create two tracked entities with the same key.
            ProductVersion = template.ProductVersion,
            RecurringBooking = recurringBooking,
        });
        recurringBooking.MarketplaceBooking = billedMarketplaceBooking;
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            billedMarketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetPaginatedRowsAsync(
            new PaginationInputParam(null, 50, null, null),
            new MarketplacePurchaseHistorySearchCriteria(scenario.Organization.CustomDomain, null, null,
                [MarketplacePurchaseSourceType.Subscription]),
            null,
            cancellationToken);

        var history = result.Item2.Single(item => item.Node.Id == subscription.Id).Node;
        history.PaymentStatus.ShouldBe(PaymentStatus.Confirmed);
        history.TotalAmount.ShouldBe(25m);
        history.Currency.ShouldBe(Currency.Nzd);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SubscriptionFilterScenarioFixtureCustomizer)])]
    public async Task Append_Subscription_Events_Idempotently_And_Read_Them_Newest_First(
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await SubscriptionFilterScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        var sourceId = scenario.ActiveConfirmed.Subscription.Id;
        var older = CreateEvent("subscription-event-older", sourceId, DateTimeOffset.UtcNow.AddMinutes(-2),
            MarketplacePurchaseHistoryEventType.PurchaseCreated);
        var newer = CreateEvent("subscription-event-newer", sourceId, DateTimeOffset.UtcNow.AddMinutes(-1),
            MarketplacePurchaseHistoryEventType.SubscriptionRenewed);

        var first = await repositoryFactory.MarketplacePurchaseHistoryRepository.AppendEventAsync(older, "subscription-created-1", cancellationToken);
        var replay = await repositoryFactory.MarketplacePurchaseHistoryRepository.AppendEventAsync(older with
            {
                Id = "different-id",
            },
            "subscription-created-1", cancellationToken);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.AppendEventAsync(newer, "subscription-renewed-1", cancellationToken);

        replay.Id.ShouldBe(first.Id);
        var events = await repositoryFactory.MarketplacePurchaseHistoryRepository.GetEventsAsync(
            MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
            sourceId,
            cancellationToken);

        var eventIds = events.Select(item => item.Id).ToList();
        eventIds.ShouldContain(first.Id);
        eventIds.ShouldContain(newer.Id);
        events.Single(item => item.Id == first.Id).Type.ShouldBe(MarketplacePurchaseHistoryEventType.PurchaseCreated);
        events.Single(item => item.Id == newer.Id).Type.ShouldBe(MarketplacePurchaseHistoryEventType.SubscriptionRenewed);
        eventIds.IndexOf(newer.Id).ShouldBeLessThan(eventIds.IndexOf(first.Id));
    }

    private static MarketplacePurchaseHistoryEventModel CreateEvent(
        string id,
        string sourceId,
        DateTimeOffset occurredAt,
        MarketplacePurchaseHistoryEventType type) => new(
        id,
        sourceId,
        MarketplacePurchaseHistoryEligibleSourceType.Subscription,
        type,
        occurredAt,
        occurredAt,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        type == MarketplacePurchaseHistoryEventType.SubscriptionRenewed ? occurredAt : null,
        null);
}
