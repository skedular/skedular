using Api.Shared.Services;
using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using FakeItEasy;
using Shouldly;
using Testing.Shared;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using ProductVersion = Booking.Shared.Models.ProductVersion;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingServiceTests;

public class MarketplaceBookingServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_Throws_CustomerNotFound_When_Customers_Cannot_Be_Found(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var organizations = new List<Organization>();
        var teams = new List<Team>();
        var customer = new Customer();
        var booking = new Models.Booking
        {
            InvolvedCustomers = [new Models.Customer { Id = "customer-1" }],
            MarketplaceBooking = new MarketplaceBooking { ProductVersion = new ProductVersion { Id = "product-version-1" } }
        };
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<ICollection<string>>.That.Contains("customer-1"), true, cancellationToken))
            .Returns([]);

        // Act & Assert
        await Should.ThrowAsync<CustomerNotFound>(() =>
            sut.AddAsync(booking, customer, organizations, teams, null, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_Throws_ProductVersionNotFound_When_ProductVersion_Does_Not_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var organizations = new List<Organization>();
        var teams = new List<Team>();
        var customer = new Customer();
        var booking = new Models.Booking
        {
            InvolvedCustomers = [new Models.Customer { Id = "customer-1" }],
            MarketplaceBooking = new MarketplaceBooking { ProductVersion = new ProductVersion { Id = "product-version-1" } }
        };
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<ICollection<string>>.That.Contains("customer-1"), true, cancellationToken))
            .Returns([new Customer { Id = "customer-1" }]);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => productVersionRepository.GetByIdAsync("product-version-1", cancellationToken))
            .Returns((Database.Entities.ProductVersion?)null);

        // Act & Assert
        await Should.ThrowAsync<ProductVersionNotFound>(() =>
            sut.AddAsync(booking, customer, organizations, teams, null, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task UpdateAsync_Throws_BookingIsNotMarketplace_When_Booking_Is_Not_Marketplace(
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var booking = new Models.Booking();
        var organizations = new List<Organization>();
        var teams = new List<Team>();
        var lastModifiedByCustomer = new Customer();
        var existingBooking = new Database.Entities.Booking { Channel = BookingChannelConstants.Private };

        // Act & Assert
        await Should.ThrowAsync<BookingIsNotMarketplace>(() =>
            sut.UpdateAsync(booking, existingBooking, lastModifiedByCustomer, organizations, teams, null, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_BookingIsNotMarketplace_When_Booking_Is_Not_Marketplace(
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var existingBooking = new Database.Entities.Booking { Channel = BookingChannelConstants.Private };

        // Act & Assert
        await Should.ThrowAsync<BookingIsNotMarketplace>(() =>
            sut.DeleteAsync(existingBooking, deletedByCustomer, cancellationToken));
    }
}
