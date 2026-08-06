using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Microsoft.Extensions.Logging;
using Temporalio.Testing;
using Testing.Shared.Assertions;
using BookingModel = Booking.Shared.Models.Booking;
using CustomerModel = Booking.Shared.Models.Customer;
using OrganizationModel = Booking.Shared.Models.Organization;

namespace Booking.Shared.UnitTests.Activities.BookingIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AdjustRequiredResourcesForPrivateRecurringBookingAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Suppress_New_Instance_And_Keep_Workflow_Alive_When_Trial_Is_Expired(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen]
        IPrivateBookingService privateBookingService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        ILogger<PrivateRecurringBookingIntegrations> logger,
        PrivateRecurringBookingIntegrations sut,
        string recurringBookingId,
        string bookingId,
        DateTimeOffset trialStartedAt)
    {
        var missingDay = DateOnly.FromDateTime(trialStartedAt.AddDays(15).UtcDateTime);
        var recurringBooking = new RecurringBooking
        {
            Id = recurringBookingId,
            InvolvedCustomers = [new Customer()],
            InvolvedOrganizations = [new Organization()],
            InvolvedTeams = [],
            Bookings = [],
        };
        var booking = new BookingModel
        {
            Id = bookingId,
        };
        var accessDecision = new SpacesAccessDecision(
            false,
            SpacesSubscriptionStatus.TrialExpired,
            SpacesAccessReasonCode.TrialExpired,
            SpacesAccessAction.CreateBookingInstance,
            OfferingCode.SpacesFreeTierV1,
            trialStartedAt,
            trialStartedAt.AddDays(14),
            0,
            false,
            false,
            true,
            true,
            null,
            false);

        A.CallTo(() => repositoryFactory.RecurringBookingRepository.GetByIdAsync(recurringBookingId, A<CancellationToken>._))
            .Returns(recurringBooking);
        A.CallTo(() => repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId, A<DateTimeOffset>._, null, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Booking>>([]));
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking, A<DateTimeOffset>._, A<DateTimeOffset>._, A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([missingDay], [missingDay], [], true));
        A.CallTo(() => entityMapper.MapTo(recurringBooking, missingDay)).Returns(booking);
        A.CallTo(() => privateBookingService.AddAsync(
                booking,
                recurringBooking.InvolvedCustomers.First(),
                A<IReadOnlyList<Organization>>._,
                A<IReadOnlyList<Team>>._,
                recurringBooking,
                A<CancellationToken>._))
            .ThrowsAsync(new SpacesAccessDenied(accessDecision));

        var environment = new ActivityEnvironment();
        var result = await environment.RunAsync(() => sut.AdjustRequiredResourcesForPrivateRecurringBookingAsync(
            new AdjustRequiredResourcesForPrivateRecurringBookingInput(recurringBookingId)));

        result.ShouldBe(new AdjustRequiredResourcesForPrivateRecurringBookingAsyncResponse(false, false));
        A.CallTo(() => repositoryFactory.BookingRepository.Add(A<Database.Entities.Booking>._)).MustNotHaveHappened();
        LogAssertions.ACallToLogInfoContaining(logger, "recurring Spaces booking instance suppressed by access policy")
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Blocked_Recurring_Instance_When_Quota_Is_Exceeded(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen]
        IPrivateBookingService privateBookingService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        ILogger<PrivateRecurringBookingIntegrations> logger,
        PrivateRecurringBookingIntegrations sut)
    {
        var recurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
            From = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-1",
                },
            ],
            InvolvedOrganizations =
            [
                new Organization
                {
                    Id = "org-1",
                },
            ],
            InvolvedTeams = [],
            Bookings = [],
        };
        var missingDay = new DateOnly(2026, 6, 16);
        var booking = new BookingModel
        {
            Id = "booking-1",
            From = new DateTimeOffset(2026, 6, 16, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 6, 16, 10, 0, 0, TimeSpan.Zero),
            InvolvedCustomers =
            [
                new CustomerModel
                {
                    Id = "customer-1",
                },
            ],
            InvolvedOrganizations =
            [
                new OrganizationModel
                {
                    Id = "org-1",
                },
            ],
        };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository.GetByIdAsync(
                recurringBooking.Id, A<CancellationToken>._))
            .Returns(recurringBooking);
        A.CallTo(() => repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
                recurringBooking.Id, A<DateTimeOffset>._, null, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Booking>>([]));
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([missingDay], [missingDay], [], true));
        A.CallTo(() => entityMapper.MapTo(recurringBooking, missingDay)).Returns(booking);
        A.CallTo(() => privateBookingService.AddAsync(
                booking,
                recurringBooking.InvolvedCustomers.First(),
                A<IReadOnlyList<Organization>>._,
                A<IReadOnlyList<Team>>._,
                recurringBooking,
                A<CancellationToken>._))
            .ThrowsAsync(new SpacesBookingQuotaExceeded(SpacesQuotaReasonCode.FreeTierLimitExceeded, 100, 100, 1, 0, 0, []));

        var environment = new ActivityEnvironment();
        var result = await environment.RunAsync(() => sut.AdjustRequiredResourcesForPrivateRecurringBookingAsync(
            new AdjustRequiredResourcesForPrivateRecurringBookingInput(recurringBooking.Id)));

        result.Deleted.ShouldBeFalse();
        LogAssertions.ACallToLogInfoContaining(logger, "recurring Spaces booking instance blocked by quota")
            .MustHaveHappenedOnceExactly();
    }
}
