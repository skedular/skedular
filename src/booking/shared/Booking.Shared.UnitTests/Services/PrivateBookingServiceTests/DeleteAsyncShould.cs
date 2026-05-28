using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace Booking.Shared.UnitTests.Services.PrivateBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeleteAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Add_Skipped_Date_And_Signal_Workflow_When_Preserving_Recurring_Series(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ICachedBookingService cachedBookingService,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        PrivateBookingService sut,
        CancellationToken cancellationToken)
    {
        var deletedByCustomer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking { Id = "recurring-1", Channel = BookingChannelConstants.Private, SkippedDates = [] };
        var existingBooking = new Database.Entities.Booking
        {
            Id = "booking-1",
            Channel = BookingChannelConstants.Private,
            From = new DateTimeOffset(2026, 4, 20, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero),
            InvolvedResources = [],
            ResourceBookingSlots = [],
            RecurringBooking = recurringBooking
        };
        var deletedBooking = new Shared.Models.Booking { Id = existingBooking.Id };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => recurringBookingRepository.Update(recurringBooking)).Returns(recurringBooking);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(deletedBooking);

        var result = await sut.DeleteAsync(existingBooking, deletedByCustomer, true, cancellationToken);

        result.ShouldBe(deletedBooking);
        recurringBooking.LastModifiedByCustomer.ShouldBe(deletedByCustomer);
        recurringBooking.SkippedDates.ShouldContain(item => item == new DateTimeOffset(2026, 4, 20, 0, 0, 0, TimeSpan.Zero));
        A.CallTo(() => recurringBookingRepository.Update(recurringBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.SignalWorkflowBookPrivateRecurringResourcesUpdated(recurringBooking.Id, unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedBookingService.RemoveByIdAsync(deletedBooking.Id, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Update_Recurring_Series_When_PreserveRecurringSeries_Is_False(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        PrivateBookingService sut,
        CancellationToken cancellationToken)
    {
        var recurringBooking = new RecurringBooking { Id = "recurring-1", Channel = BookingChannelConstants.Private, SkippedDates = [] };
        var existingBooking = new Database.Entities.Booking
        {
            Id = "booking-1",
            Channel = BookingChannelConstants.Private,
            From = new DateTimeOffset(2026, 4, 20, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero),
            InvolvedResources = [],
            ResourceBookingSlots = [],
            RecurringBooking = recurringBooking
        };
        var deletedBooking = new Shared.Models.Booking { Id = existingBooking.Id };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(deletedBooking);

        var result = await sut.DeleteAsync(existingBooking, null, false, cancellationToken);

        result.ShouldBe(deletedBooking);
        recurringBooking.SkippedDates.ShouldBeEmpty();
        A.CallTo(() => recurringBookingRepository.Update(A<RecurringBooking>._)).MustNotHaveHappened();
        A.CallTo(() => temporalOutboxService.SignalWorkflowBookPrivateRecurringResourcesUpdated(A<string>._, unitOfWork)).MustNotHaveHappened();
    }
}
