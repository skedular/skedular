using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class SpacesBookingUsageRepositoryShould(IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_For_NonExistent_Organization(CancellationToken cancellationToken)
    {
        var org = await repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync("nonexistent-org", cancellationToken);

        org.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Count_Only_Current_Period_Booking_Instances_For_Organization(
        string organizationId,
        string otherOrganizationId,
        CancellationToken cancellationToken)
    {
        var periodStart = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var periodEnd = periodStart.AddMonths(1);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            organizationId,
            cancellationToken);
        var otherOrganization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            otherOrganizationId,
            cancellationToken);

        repositoryFactory.BookingRepository.Add(CreateBooking("counted-1", periodStart.AddDays(1), organization));
        repositoryFactory.BookingRepository.Add(CreateBooking("counted-2", periodEnd.AddTicks(-1), organization));
        repositoryFactory.BookingRepository.Add(CreateBooking("before-period", periodStart.AddTicks(-1), organization));
        repositoryFactory.BookingRepository.Add(CreateBooking("after-period", periodEnd, organization));
        repositoryFactory.BookingRepository.Add(CreateBooking("other-organization", periodStart.AddDays(2), otherOrganization));
        repositoryFactory.BookingRepository.Add(CreateBooking("deleted", periodStart.AddDays(3), organization, periodStart.AddDays(4)));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var count = await repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
            organizationId,
            periodStart,
            periodEnd,
            cancellationToken);

        count.ShouldBe(2);
    }

    private static Shared.Database.Entities.Booking CreateBooking(
        string id,
        DateTimeOffset from,
        Organization organization,
        DateTimeOffset? deletedAt = null) =>
        new()
        {
            Id = id,
            From = from,
            Until = from.AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
            InvolvedOrganizations = [organization],
            DeletedAt = deletedAt
        };
}
