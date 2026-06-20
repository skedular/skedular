using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using OfferingModel = Api.Shared.Services.Models.Offering;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class SpacesRecurringBookingQuotaShould(
    IRepositoryFactory repositoryFactory,
    ISpacesBookingQuotaService spacesBookingQuotaService,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Enforce_Existing_One_Hundred_Instance_Limit_During_Active_Trial(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var periodStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var periodEnd = periodStart.AddMonths(1);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        organization.Type = OrganizationTypeConstants.Marketplace;
        organization.Offering = new OfferingModel
        {
            Code = OfferingCode.SpacesFreeTierV1,
            SpacesPlanCode = 1,
            SpacesQuotaLimit = 100,
            SpacesPeriodStart = periodStart,
            SpacesPeriodEnd = periodEnd,
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = now.AddDays(-1),
            SpacesTrialEndsAt = now.AddDays(13)
        };
        for (var index = 0; index < 99; index++)
        {
            repositoryFactory.BookingRepository.Add(CreateBooking($"{organizationId}-booking-{index}", now, organization));
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var oneHundredth = await spacesBookingQuotaService.CanCreateBookingInstanceAsync(organizationId, now, cancellationToken);
        repositoryFactory.BookingRepository.Add(CreateBooking($"{organizationId}-booking-99", now, organization));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        var oneHundredAndFirst = await spacesBookingQuotaService.CanCreateBookingInstanceAsync(organizationId, now, cancellationToken);

        oneHundredth.CanCreate.ShouldBeTrue();
        oneHundredth.AccessDecision?.Status.ShouldBe(SpacesSubscriptionStatus.TrialActive);
        oneHundredAndFirst.CanCreate.ShouldBeFalse();
        oneHundredAndFirst.ReasonCode.ShouldBe(SpacesQuotaReasonCode.FreeTierLimitExceeded);
        organization.Offering.SpacesTrialStartedAt.ShouldBe(now.AddDays(-1));
        organization.Offering.SpacesTrialEndsAt.ShouldBe(now.AddDays(13));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_At_Exact_Expiry_Without_Changing_Existing_Bookings(
        string organizationId,
        string existingBookingId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var periodStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var periodEnd = periodStart.AddMonths(1);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        organization.Type = OrganizationTypeConstants.Marketplace;
        organization.Offering = new OfferingModel
        {
            Code = OfferingCode.SpacesFreeTierV1,
            SpacesPlanCode = 1,
            SpacesQuotaLimit = 100,
            SpacesPeriodStart = periodStart,
            SpacesPeriodEnd = periodEnd,
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = now.AddDays(-14),
            SpacesTrialEndsAt = now
        };
        repositoryFactory.BookingRepository.Add(CreateBooking(existingBookingId, now.AddHours(-1), organization));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var decision = await spacesBookingQuotaService.CanCreateBookingInstanceAsync(organizationId, now, cancellationToken);
        var persistedCount = await repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
            organizationId,
            periodStart,
            periodEnd,
            cancellationToken);

        decision.CanCreate.ShouldBeFalse();
        decision.AccessDecision?.Status.ShouldBe(SpacesSubscriptionStatus.TrialExpired);
        decision.ReasonCode.ShouldBe(SpacesQuotaReasonCode.TrialExpired);
        persistedCount.ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Count_Current_Period_Booking_Instances_From_Booking_Rows(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var periodStart = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var periodEnd = periodStart.AddMonths(1);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            organizationId,
            cancellationToken);
        organization.Type = OrganizationTypeConstants.Marketplace;
        organization.Offering = new OfferingModel
        {
            SpacesPlanCode = 1, SpacesQuotaLimit = 100, SpacesPeriodStart = periodStart, SpacesPeriodEnd = periodEnd
        };

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var count = await repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
            organizationId,
            periodStart,
            periodEnd,
            cancellationToken);

        count.ShouldBe(0);
    }

    private static Shared.Database.Entities.Booking CreateBooking(
        string id,
        DateTimeOffset from,
        Organization organization) =>
        new()
        {
            Id = id,
            From = from,
            Until = from.AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
            InvolvedOrganizations = [organization]
        };
}
