using Booking.Api.Services;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Api.UnitTests.Services.BookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetArrearsInvoicesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Load_Unauthorized_Invoices_From_Batch(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IOrganizationArrearsInvoiceRepository organizationArrearsInvoiceRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Database.Entities.Booking
        {
            Id = "booking-hidden",
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-owner",
                },
            ],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.OrganizationArrearsInvoiceRepository).Returns(organizationArrearsInvoiceRepository);
        A.CallTo(() => bookingRepository.GetByIdsForGraphQlBatchAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([booking]);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-viewer");

        var result = await sut.GetArrearsInvoicesAsync([booking.Id], cancellationToken);

        result[booking.Id].ShouldBeEmpty();
        A.CallTo(() => organizationArrearsInvoiceRepository.GetByBookingIdsUntrackedAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
