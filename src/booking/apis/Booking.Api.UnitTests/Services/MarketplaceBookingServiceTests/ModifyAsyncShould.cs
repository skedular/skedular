using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using MarketplaceBookingModificationActorKind = Booking.Shared.Models.MarketplaceBookingModificationActorKind;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Api.UnitTests.Services.MarketplaceBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ModifyAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_A_Non_Owner_Without_Product_Operator_Permission(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IContext context,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceBookingModificationService sut,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository,
        IProductVersionRepository productVersionRepository,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var booking = new BookingEntity
        {
            Id = "booking-1",
            InvolvedCustomers = [],
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Id = "product-version-1",
                },
            },
        };
        var productVersion = new ProductVersion
        {
            Id = "product-version-1",
            Product = new Product
            {
                Organization = new Organization
                {
                    Id = "organization-1",
                },
            },
        };
        var command = new MarketplaceBookingModificationCommand(
            booking.Id, 3,
            new DateTimeOffset(2026, 8, 9, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 9, 10, 0, 0, TimeSpan.Zero),
            null, "Operator change", MarketplaceBookingModificationActorKind.OrganizationOperator);

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByIdAsync(booking.Id, cancellationToken)).Returns(booking);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersion.Id, cancellationToken)).Returns(productVersion);
        A.CallTo(() => organizationAuthorizationService.CanOverrideCancellationPolicyAsync(
                productVersion.Product.Organization.Id, customer.Id, cancellationToken))
            .Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.ModifyAsync(command, cancellationToken));
    }
}
