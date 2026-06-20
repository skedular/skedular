using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Domain.IntegrationTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingAvailabilityFailureShould(IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Retain_The_Losing_Submission_Failure_Without_A_Resource_Association(
        string bookingId,
        string failureId,
        CancellationToken cancellationToken)
    {
        var booking = repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            From = new DateTimeOffset(2026, 7, 24, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 7, 24, 10, 0, 0, TimeSpan.Zero),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = []
        });
        repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = failureId,
            FailureKey = $"availability-{failureId}",
            BookingId = booking.Id,
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.OneTimeBooking,
            FinalizedAt = DateTimeOffset.UtcNow,
            CustomerAction = MarketplaceBookingFailureCustomerActionConstants.Rebook
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var failure = await repositoryFactory.MarketplaceBookingFailureRepository.GetByBookingIdAsync(booking.Id, cancellationToken);
        var persistedBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);

        failure.ShouldNotBeNull();
        failure.Category.ShouldBe(MarketplaceBookingFailureCategoryConstants.AvailabilityConflict);
        persistedBooking.ShouldNotBeNull();
        persistedBooking.InvolvedResources.ShouldBeEmpty();
    }
}
