using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Activities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using FakeItEasy;
using Temporalio.Testing;

namespace Booking.Shared.UnitTests.Activities.OrganizationArrearsBillingIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationArrearsBillingIntegrationsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Next_Monthly_Boundary_On_First_Of_Next_Month(
        [Frozen] TimeProvider timeProvider,
        OrganizationArrearsBillingIntegrations sut)
    {
        var now = new DateTimeOffset(2026, 3, 22, 10, 30, 0, TimeSpan.Zero);
        var configuration = new OrganizationArrearsBillingConfiguration(
            "org-1",
            OrganizationBillingCycle.Monthly);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var result = await sut.GetNextRunAtAsync(new GetOrganizationArrearsBillingNextRunAtInput(configuration));

        result.ShouldBe(new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Next_Weekly_Boundary_On_Monday(
        [Frozen] TimeProvider timeProvider,
        OrganizationArrearsBillingIntegrations sut)
    {
        var now = new DateTimeOffset(2026, 3, 22, 10, 30, 0, TimeSpan.Zero); // Sunday
        var configuration = new OrganizationArrearsBillingConfiguration(
            "org-1",
            OrganizationBillingCycle.Weekly);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var result = await sut.GetNextRunAtAsync(new GetOrganizationArrearsBillingNextRunAtInput(configuration));

        result.ShouldBe(new DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Next_Fortnightly_Boundary_On_Alternating_Monday(
        [Frozen] TimeProvider timeProvider,
        OrganizationArrearsBillingIntegrations sut)
    {
        var now = new DateTimeOffset(2026, 3, 24, 10, 30, 0, TimeSpan.Zero); // Tuesday in a fortnight starting Monday 2026-03-23
        var configuration = new OrganizationArrearsBillingConfiguration(
            "org-1",
            OrganizationBillingCycle.Fortnightly);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var result = await sut.GetNextRunAtAsync(new GetOrganizationArrearsBillingNextRunAtInput(configuration));

        result.ShouldBe(new DateTimeOffset(2026, 3, 30, 0, 0, 0, TimeSpan.Zero));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Previous_Full_Period_For_Regular_Monthly_Run(OrganizationArrearsBillingIntegrations sut)
    {
        var runAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
        var configuration = new OrganizationArrearsBillingConfiguration(
            "org-1",
            OrganizationBillingCycle.Monthly);

        var result = await sut.GetBillingPeriodForRunAtAsync(
            new GetOrganizationArrearsBillingPeriodInput(runAt, false, configuration));

        result.ShouldBe(new BillingPeriod(
            new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Current_Week_Period_For_Manual_Weekly_Run(
        [Frozen] TimeProvider timeProvider,
        OrganizationArrearsBillingIntegrations sut)
    {
        var now = new DateTimeOffset(2026, 3, 25, 10, 30, 0, TimeSpan.Zero); // Wednesday
        var configuration = new OrganizationArrearsBillingConfiguration(
            "org-1",
            OrganizationBillingCycle.Weekly);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var result = await sut.GetBillingPeriodForRunAtAsync(
            new GetOrganizationArrearsBillingPeriodInput(now, true, configuration));

        result.ShouldBe(new BillingPeriod(
            new DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero),
            now));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Current_Month_Period_For_Manual_Monthly_Run(
        [Frozen] TimeProvider timeProvider,
        OrganizationArrearsBillingIntegrations sut)
    {
        var now = new DateTimeOffset(2026, 3, 25, 10, 30, 0, TimeSpan.Zero);
        var configuration = new OrganizationArrearsBillingConfiguration(
            "org-1",
            OrganizationBillingCycle.Monthly);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var result = await sut.GetBillingPeriodForRunAtAsync(
            new GetOrganizationArrearsBillingPeriodInput(now, true, configuration));

        result.ShouldBe(new BillingPeriod(
            new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            now));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Query_In_Arrears_Bookings_Using_The_Full_Billing_Window(
        [Frozen] IRepositoryFactory repositoryFactory,
        OrganizationArrearsBillingIntegrations sut,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var billingPeriod = new BillingPeriod(
            new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => bookingRepository.GetInArrearsByOrganizationBeforeAsync(
                "org-1",
                billingPeriod.StartInclusive,
                billingPeriod.EndExclusive,
                environment.CancellationTokenSource.Token))
            .Returns([]);

        await environment.RunAsync(() =>
            sut.GenerateOrganizationArrearsInvoicesAsync(
                new GenerateOrganizationArrearsInvoicesInput("org-1", billingPeriod, OrganizationBillingCycle.Monthly)));
    }
}
