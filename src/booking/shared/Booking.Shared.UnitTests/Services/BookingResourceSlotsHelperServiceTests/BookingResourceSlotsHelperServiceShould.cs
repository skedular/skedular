using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.BookingResourceSlotsHelperServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookingResourceSlotsHelperServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Clears_Customers_From_All_Slots_And_Removes_Slots_From_Booking(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        BookingResourceSlotsHelperService sut,
        IResourceBookingSlotRepository resourceBookingSlotRepository)
    {
        // Arrange
        IReadOnlyList<ResourceBookingSlot>? updatedSlots = null;
        var customer1 = new Customer
        {
            Id = "customer-1",
        };
        var customer2 = new Customer
        {
            Id = "customer-2",
        };
        var slot1 = new ResourceBookingSlot
        {
            Id = "slot-1",
            Customers = [customer1],
        };
        var slot2 = new ResourceBookingSlot
        {
            Id = "slot-2",
            Customers = [customer2],
        };
        var booking = new Database.Entities.Booking
        {
            Id = "booking-1",
            ResourceBookingSlots = [slot1, slot2],
        };

        A.CallTo(() => repositoryFactory.ResourceBookingSlotRepository).Returns(resourceBookingSlotRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => resourceBookingSlotRepository.UpdateRange(A<IEnumerable<ResourceBookingSlot>>._))
            .Invokes((IEnumerable<ResourceBookingSlot> slots) => updatedSlots = [.. slots]);

        // Act
        sut.RemoveAllSlotsFromBooking(booking);

        // Assert
        slot1.Customers.ShouldBeEmpty();
        slot2.Customers.ShouldBeEmpty();
        booking.ResourceBookingSlots.ShouldBeEmpty();
        updatedSlots.ShouldNotBeNull();
        updatedSlots.ShouldContain(slot1);
        updatedSlots.ShouldContain(slot2);
        A.CallTo(() => resourceBookingSlotRepository.UpdateRange(A<IEnumerable<ResourceBookingSlot>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingRepository.Update(booking)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handles_Empty_Slots_Collection(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        BookingResourceSlotsHelperService sut,
        IResourceBookingSlotRepository resourceBookingSlotRepository)
    {
        // Arrange
        var booking = new Database.Entities.Booking
        {
            Id = "booking-1",
            ResourceBookingSlots = [],
        };

        A.CallTo(() => repositoryFactory.ResourceBookingSlotRepository).Returns(resourceBookingSlotRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);

        // Act
        sut.RemoveAllSlotsFromBooking(booking);

        // Assert
        booking.ResourceBookingSlots.ShouldBeEmpty();
        A.CallTo(() => resourceBookingSlotRepository.UpdateRange(A<IEnumerable<ResourceBookingSlot>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingRepository.Update(booking)).MustHaveHappenedOnceExactly();
    }
}
