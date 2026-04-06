using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using ProductVersion = Booking.Shared.Models.ProductVersion;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_Throws_CustomerNotFound_When_Customers_Cannot_Be_Found(
        [Frozen] IRepositoryFactory repositoryFactory,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
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
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
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
            sut.DeleteAsync(existingBooking, deletedByCustomer, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_MarketplaceBookingCancellationNotAllowed_When_User_Delete_Has_No_Cancellation_Policy(
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingBooking = CreateMarketplaceBooking(
            now.AddHours(4),
            false,
            ProductPricingCancellationPolicyType.NoCancellation,
            []);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        // Act & Assert
        await Should.ThrowAsync<MarketplaceBookingCancellationNotAllowed>(() =>
            sut.DeleteAsync(existingBooking, deletedByCustomer, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_MarketplaceBookingCancellationNotAllowed_When_User_Delete_Is_After_Cancellation_Deadline(
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 30, 0, TimeSpan.Zero);
        var existingBooking = CreateMarketplaceBooking(
            now.AddMinutes(30),
            false,
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(45, 100)]);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        // Act & Assert
        await Should.ThrowAsync<MarketplaceBookingCancellationNotAllowed>(() =>
            sut.DeleteAsync(existingBooking, deletedByCustomer, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_User_Delete_When_Inside_Cancellation_Window(
        [Frozen] TimeProvider timeProvider,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen] IMapper mapper,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingBooking = CreateMarketplaceBooking(
            now.AddHours(4),
            false,
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(180, 100)]);
        var deletedBooking = new Models.Booking { Id = existingBooking.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => mapper.MapTo(existingBooking)).Returns(deletedBooking);

        // Act
        var result = await sut.DeleteAsync(existingBooking, deletedByCustomer, false, cancellationToken);

        // Assert
        result.ShouldBe(deletedBooking);
        existingBooking.DeletedByCustomer.ShouldBe(deletedByCustomer);
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(existingBooking, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_OrganizationOperator_Delete_After_Cancellation_Deadline(
        [Frozen] TimeProvider timeProvider,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen] IMapper mapper,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 30, 0, TimeSpan.Zero);
        var existingBooking = CreateMarketplaceBooking(
            now.AddMinutes(30),
            false,
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(45, 100)]);
        var deletedBooking = new Models.Booking { Id = existingBooking.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => mapper.MapTo(existingBooking)).Returns(deletedBooking);

        var result = await sut.DeleteAsync(existingBooking, deletedByCustomer, true, cancellationToken);

        result.ShouldBe(deletedBooking);
        existingBooking.DeletedByCustomer.ShouldBe(deletedByCustomer);
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(existingBooking, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task UpdateAsync_Does_Not_Recompute_Event_Resources_When_Booking_Window_Has_Not_Changed(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IMarketplaceEventResourceService marketplaceEventResourceService,
        [Frozen] IMapper mapper,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        IBookingRepository bookingRepository,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 3, 21, 9, 0, 0, TimeSpan.Zero);
        var until = from.AddHours(2);
        var existingResource = new Resource { Id = "resource-1", ResourceBookingSlots = [] };
        var existingBooking = new Database.Entities.Booking
        {
            Id = "booking-1",
            Channel = BookingChannelConstants.Marketplace,
            From = from,
            Until = until,
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            InvolvedResources = [existingResource],
            InvolvedOrganizations = [],
            InvolvedLocations = [],
            InvolvedTeams = [],
            MarketplaceBooking = new Database.Entities.MarketplaceBooking
            {
                ProductVersion = new Database.Entities.ProductVersion { Id = "product-version-1" },
                ProductPricing = ProductPricing.Empty("pricing-1") with { BookingCadence = ProductPricingCadence.OneTime },
                PaymentMethod = PaymentMethodConstants.Card
            }
        };
        var booking = new Models.Booking
        {
            Id = existingBooking.Id,
            From = from,
            Until = until,
            InvolvedCustomers = [new Models.Customer { Id = "customer-1" }],
            Resources = [],
            InvolvedOrganizations = [],
            InvolvedLocations = [],
            InvolvedTeams = [],
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion { Id = "product-version-1" },
                ProductPricing = ProductPricing.Empty("pricing-1") with { BookingCadence = ProductPricingCadence.OneTime },
                PaymentMethod = PaymentMethod.Card
            }
        };
        var lastModifiedByCustomer = new Customer { Id = "customer-1" };
        var productVersion = new Database.Entities.ProductVersion
        {
            Id = "product-version-1",
            Type = ProductTypeConstants.Event,
            OrganizationTags = [new OrganizationTag { Type = OrganizationTagTypeConstants.Product }],
            Product = new Product { Organization = new Organization { Id = "org-1" } }
        };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<ICollection<string>>.That.Contains("customer-1"), true, cancellationToken))
            .Returns([lastModifiedByCustomer]);
        A.CallTo(() => productVersionRepository.GetByIdAsync("product-version-1", cancellationToken)).Returns(productVersion);
        A.CallTo(() => mapper.MergeTo(
                booking,
                existingBooking,
                A<ICollection<Customer>>._,
                A<ICollection<Organization>>._,
                A<ICollection<Location>>.That.Matches(locations => locations.Count == 0),
                A<ICollection<Team>>._,
                A<ICollection<Resource>>.That.Matches(resources => resources.Count == 1 && resources.First().Id == "resource-1"),
                existingBooking.CreatedByCustomer,
                lastModifiedByCustomer,
                null,
                existingBooking.MarketplaceBooking,
                null))
            .Returns(existingBooking);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => mapper.MapTo(existingBooking)).Returns(booking);

        _ = await sut.UpdateAsync(booking, existingBooking, lastModifiedByCustomer, [], [], null, false, cancellationToken);

        A.CallTo(() => marketplaceEventResourceService.PickEventResourcesAsync(A<DateTimeOffset>._, A<DateTimeOffset>._,
                A<Database.Entities.ProductVersion>._, cancellationToken))
            .MustNotHaveHappened();
    }

    private static Database.Entities.Booking CreateMarketplaceBooking(
        DateTimeOffset from,
        bool isPaymentRequired,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        ICollection<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new()
        {
            Id = "booking-1",
            Channel = BookingChannelConstants.Marketplace,
            From = from,
            Until = from.AddHours(1),
            InvolvedResources = [],
            MarketplaceBooking = new Database.Entities.MarketplaceBooking
            {
                IsPaymentRequired = isPaymentRequired,
                PaymentMethod = PaymentMethodConstants.Card,
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    CancellationPolicyType = cancellationPolicyType, CancellationRefundRules = cancellationRefundRules
                }
            }
        };
}
