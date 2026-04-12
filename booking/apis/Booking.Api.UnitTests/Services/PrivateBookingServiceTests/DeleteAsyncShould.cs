using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using SharedPrivateBookingService = Booking.Shared.Services.IPrivateBookingService;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;

namespace Booking.Api.UnitTests.Services.PrivateBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeleteAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Recurring_Series_When_Deleting_A_Single_Recurring_Instance(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IContext context,
        [Frozen] SharedPrivateBookingService sharedPrivateBookingService,
        PrivateBookingService sut,
        string bookingId,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity { Id = "customer-1" };
        var recurringBooking = new RecurringBooking { Id = "recurring-1" };
        var booking = new Shared.Database.Entities.Booking
        {
            Id = bookingId, RecurringBooking = recurringBooking, InvolvedOrganizations = [], InvolvedTeams = []
        };
        var deletedBooking = new Shared.Models.Booking { Id = bookingId };

        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByIdAsync(bookingId, cancellationToken)).Returns(booking);
        A.CallTo(() => sharedPrivateBookingService.DeleteAsync(booking, customer, true, cancellationToken)).Returns(deletedBooking);

        var result = await sut.DeleteAsync(bookingId, cancellationToken);

        result.ShouldBe(deletedBooking);
        A.CallTo(() => sharedPrivateBookingService.DeleteAsync(booking, customer, true, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_An_Involved_Organization_Cannot_Delete_The_Booking(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IContext context,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        PrivateBookingService sut,
        string bookingId,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity { Id = "customer-1" };
        var booking = new Shared.Database.Entities.Booking { Id = bookingId, InvolvedOrganizations = [new Organization { Id = "org-1" }] };
        var organization = new Organization { Id = "org-1" };

        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByIdAsync(bookingId, cancellationToken)).Returns(booking);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(A<ICollection<string>>._, null, false, false, cancellationToken))
            .Returns([organization]);
        A.CallTo(() => organizationAuthorizationService.CanDeleteBookingAsync("org-1", customer.Id, cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.DeleteAsync(bookingId, cancellationToken));
    }
}
