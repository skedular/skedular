using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Api.UnitTests.Services.MarketplaceBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetHistoriesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_History_Without_Reading_Modifications_For_Unauthorized_Booking(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IContext context,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceBookingModificationRepository modificationRepository,
        MarketplaceBookingModificationService sut,
        string token,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = "customer-viewer",
        };
        var booking = new BookingEntity
        {
            Id = "booking-hidden",
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-owner",
                },
            ],
        };
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingModificationRepository).Returns(modificationRepository);
        A.CallTo(() => context.GetVerifiableToken()).Returns(token);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(token, true, cancellationToken)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByIdsForGraphQlBatchAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([booking]);

        var result = await sut.GetHistoriesAsync([booking.Id], cancellationToken);

        result[booking.Id].ShouldBeEmpty();
        A.CallTo(() => modificationRepository.GetByBookingIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_History_When_Authenticated_Customer_Is_Missing(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IContext context,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IMarketplaceBookingModificationRepository modificationRepository,
        MarketplaceBookingModificationService sut,
        string token,
        string bookingId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingModificationRepository).Returns(modificationRepository);
        A.CallTo(() => context.GetVerifiableToken()).Returns(token);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(token, true, cancellationToken))
            .Returns((Customer?)null);

        var result = await sut.GetHistoriesAsync([bookingId], cancellationToken);

        result[bookingId].ShouldBeEmpty();
        A.CallTo(() => modificationRepository.GetByBookingIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
