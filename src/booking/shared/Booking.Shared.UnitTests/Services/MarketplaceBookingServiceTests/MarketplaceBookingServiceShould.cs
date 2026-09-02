using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore.Storage;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using MarketplaceBookingModificationActorKind = Booking.Shared.Models.MarketplaceBookingModificationActorKind;
using MarketplaceBookingModificationRequest = Booking.Shared.Models.MarketplaceBookingModificationRequest;
using ResourceSlotClaimResult = Booking.Shared.Models.ResourceSlotClaimResult;
using Offering = Api.Shared.Services.Models.Offering;
using ProductVersion = Booking.Shared.Models.ProductVersion;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_Throws_CustomerNotFound_When_Customers_Cannot_Be_Found(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        // Arrange
        var customer = new Customer();
        var booking = new Shared.Models.Booking
        {
            InvolvedCustomers =
            [
                new Shared.Models.Customer
                {
                    Id = "customer-1",
                },
            ],
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Id = "product-version-1",
                },
            },
        };
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<IReadOnlyList<string>>.That.Contains("customer-1"), true, cancellationToken))
            .Returns([]);

        // Act & Assert
        await Should.ThrowAsync<CustomerNotFound>(() =>
            sut.AddAsync(booking, customer, [], [], null, true, true, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_Throws_ProductVersionNotFound_When_ProductVersion_Does_Not_Exist(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        CancellationToken cancellationToken)
    {
        // Arrange
        var customer = new Customer();
        var booking = new Shared.Models.Booking
        {
            InvolvedCustomers =
            [
                new Shared.Models.Customer
                {
                    Id = "customer-1",
                },
            ],
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Id = "product-version-1",
                },
            },
        };
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<IReadOnlyList<string>>.That.Contains("customer-1"), true, cancellationToken))
            .Returns([
                new Customer
                {
                    Id = "customer-1",
                },
            ]);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => productVersionRepository.GetByIdAsync("product-version-1", cancellationToken))
            .Returns((Database.Entities.ProductVersion?)null);

        // Act & Assert
        await Should.ThrowAsync<ProductVersionNotFound>(() =>
            sut.AddAsync(booking, customer, [], [], null, true, true, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_Throws_SpacesBookingQuotaExceeded_When_Quota_Blocked(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IMarketplaceEventResourceService marketplaceEventResourceService,
        [Frozen]
        IProductVersionHelperService productVersionHelperService,
        [Frozen]
        ISpacesBookingQuotaService spacesBookingQuotaService,
        [Frozen]
        IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        IMarketplaceBookingRepository marketplaceBookingRepository,
        IBookingRepository bookingRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero);
        var organization = new Organization
        {
            Id = "org-1",
            Type = OrganizationTypeConstants.Marketplace,
            Offering = new Offering
            {
                Code = OfferingCode.SpacesFreeTierV1,
            },
        };
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            AcceptedPaymentMethods = [PaymentMethod.Card],
            BillingMode = ProductPricingBillingMode.Upfront,
            MaxAllowedResourcesLockTimePaidViaCard = 15,
            NumberOfResourcesToBook = 1,
        };
        var booking = new Shared.Models.Booking
        {
            From = from,
            Until = from.AddHours(1),
            InvolvedCustomers =
            [
                new Shared.Models.Customer
                {
                    Id = customer.Id,
                },
            ],
            Resources = [],
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Id = "product-version-1",
                },
                ProductPricing = pricing,
                PaymentMethod = PaymentMethod.Card,
            },
        };
        var productVersion = new Database.Entities.ProductVersion
        {
            Id = "product-version-1",
            Type = ProductTypeConstants.Event,
            PricingOptions = [pricing],
            OrganizationTags =
            [
                new OrganizationTag
                {
                    Type = OrganizationTagTypeConstants.Product,
                },
            ],
            Product = new Product
            {
                Organization = organization,
            },
            Currency = CurrencyConstants.Nzd,
        };
        var marketplaceBookingEntity = new Database.Entities.MarketplaceBooking
        {
            Id = "marketplace-booking-1",
        };
        var bookingEntity = new BookingEntity
        {
            Id = "booking-1",
        };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<IReadOnlyList<string>>.That.Contains(customer.Id), true, cancellationToken))
            .Returns([customer]);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersion.Id, cancellationToken)).Returns(productVersion);
        A.CallTo(() => productVersionHelperService.FindMatchingPricing(A<IReadOnlyList<ProductPricing>>._, pricing)).Returns(pricing);
        var selectedBookingDate = default(DateOnly);
        A.CallTo(() => marketplaceBookingAvailableDaysService.IsAvailable(
                A<ProductPricing>._,
                A<DateTimeOffset>._,
                out selectedBookingDate))
            .Returns(true);
        A.CallTo(() => marketplaceEventResourceService.PickEventResourcesAsync(from, from.AddHours(1), productVersion, cancellationToken))
            .Returns([]);
        A.CallTo(() => entityMapper.MapTo(A<MarketplaceBooking>._, customer, null, productVersion, null)).Returns(marketplaceBookingEntity);
        A.CallTo(() => marketplaceBookingRepository.Add(marketplaceBookingEntity)).Returns(marketplaceBookingEntity);
        A.CallTo(() => entityMapper.MapTo(
                booking,
                A<IReadOnlyList<Customer>>._,
                A<IReadOnlyList<Organization>>.That.Matches(items => items.Any(item => item.Id == organization.Id)),
                A<IReadOnlyList<Location>>._,
                A<IReadOnlyList<Team>>._,
                A<IReadOnlyList<Resource>>._,
                customer,
                null,
                null,
                marketplaceBookingEntity,
                null))
            .Returns(bookingEntity);
        A.CallTo(() => bookingRepository.Add(bookingEntity)).Returns(bookingEntity);
        A.CallTo(() => resourceRepository.TryClaimCompleteSlotSetAsync(bookingEntity, A<IReadOnlyCollection<string>>._, cancellationToken))
            .Returns(ResourceSlotClaimResult.Success());
        A.CallTo(() => spacesBookingQuotaService.TryReserveBookingInstancesAsync(
                organization.Id,
                A<IReadOnlyList<DateTimeOffset>>.That.Matches(values => values.Single() == from.ToUniversalTime()),
                cancellationToken))
            .Returns(new SpacesQuotaDecision(
                false,
                SpacesQuotaReasonCode.FreeTierLimitExceeded,
                1,
                100,
                100,
                1,
                0,
                0,
                new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero)));

        await Should.ThrowAsync<SpacesBookingQuotaExceeded>(() =>
            sut.AddAsync(booking, customer, [], [], null, true, true, false, cancellationToken));

        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task AdjustRequiredResourcesAsync_Only_Queries_Resources_Compatible_With_The_Product(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IMarketplaceBookingPreferenceService marketplaceBookingPreferenceService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        IResourceRepository resourceRepository,
        IBookingRepository bookingRepository,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var productTag = new OrganizationTag
        {
            Id = "product-tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var requestedResource = new Resource
        {
            Id = "requested-resource-1",
        };
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            NumberOfResourcesToBook = 1,
        };
        var productVersion = new Database.Entities.ProductVersion
        {
            Id = "product-version-1",
            Type = ProductTypeConstants.Resource,
            PricingOptions = [pricing],
            OrganizationTags = [productTag],
        };
        var booking = new BookingEntity
        {
            Id = "booking-1",
            From = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
            InvolvedCustomers = [customer],
            MarketplaceBooking = new Database.Entities.MarketplaceBooking
            {
                ProductVersion = productVersion,
                ProductPricing = pricing,
            },
            RecurringBooking = new RecurringBooking
            {
                RequestedResources = [requestedResource],
            },
        };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => customerRepository.GetByIdsAsync(new[] { customer.Id }, true, cancellationToken)).Returns([customer]);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersion.Id, cancellationToken)).Returns(productVersion);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                booking.From,
                booking.Until,
                A<IReadOnlyList<string>>.That.Matches(resourceIds => resourceIds.SequenceEqual(new[] { requestedResource.Id })),
                A<IReadOnlyList<string>>.That.Matches(tagIds => tagIds.SequenceEqual(new[] { productTag.Id })),
                Array.Empty<string>(),
                Array.Empty<string>(),
                cancellationToken))
            .Returns([]);
        A.CallTo(() => marketplaceBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                customer,
                booking.From,
                booking.Until,
                productVersion,
                1,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => entityMapper.MapTo(booking)).Returns(new Shared.Models.Booking
        {
            Id = booking.Id,
        });
        A.CallTo(() => bookingRepository.Update(booking)).Returns(booking);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.AdjustRequiredResourcesAsync(booking, cancellationToken);

        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                booking.From,
                booking.Until,
                A<IReadOnlyList<string>>.That.Matches(resourceIds => resourceIds.SequenceEqual(new[] { requestedResource.Id })),
                A<IReadOnlyList<string>>.That.Matches(tagIds => tagIds.SequenceEqual(new[] { productTag.Id })),
                Array.Empty<string>(),
                Array.Empty<string>(),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task UpdateAsync_Throws_BookingIsNotMarketplace_When_Booking_Is_Not_Marketplace(
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var booking = new Shared.Models.Booking();
        var lastModifiedByCustomer = new Customer();
        var existingBooking = new BookingEntity
        {
            Channel = BookingChannelConstants.Private,
        };

        // Act & Assert
        await Should.ThrowAsync<BookingIsNotMarketplace>(() =>
            sut.UpdateAsync(booking, existingBooking, lastModifiedByCustomer, [], [], null, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_BookingIsNotMarketplace_When_Booking_Is_Not_Marketplace(
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var existingBooking = new BookingEntity
        {
            Channel = BookingChannelConstants.Private,
        };

        // Act & Assert
        await Should.ThrowAsync<BookingIsNotMarketplace>(() =>
            sut.DeleteAsync(existingBooking, deletedByCustomer, false, null, true, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_MarketplaceBookingCancellationNotAllowed_When_User_Delete_Has_No_Cancellation_Policy(
        [Frozen]
        TimeProvider timeProvider,
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
            sut.DeleteAsync(existingBooking, deletedByCustomer, false, null, true, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_MarketplaceBookingCancellationNotAllowed_When_User_Delete_Is_For_A_Past_Booking(
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 30, 0, TimeSpan.Zero);
        var existingBooking = CreateMarketplaceBooking(
            now.StartOfDay().AddMinutes(-30),
            false,
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(45, 100)]);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        // Act & Assert
        await Should.ThrowAsync<MarketplaceBookingCancellationNotAllowed>(() =>
            sut.DeleteAsync(existingBooking, deletedByCustomer, false, null, true, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_User_Delete_For_Todays_Booking_After_Refund_Cutoff(
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IEntitlementCancellationService entitlementCancellationService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
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
        existingBooking.MarketplaceBooking!.EntitlementId = "entitlement-1";
        var deletedBooking = new Shared.Models.Booking
        {
            Id = existingBooking.Id,
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(deletedBooking);
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(existingBooking, deletedByCustomer, cancellationToken))
            .Returns(Task.FromResult<MarketplaceRefund?>(null));

        var result = await sut.DeleteAsync(existingBooking, deletedByCustomer, false, null, true, cancellationToken);

        result.ShouldBe(deletedBooking);
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(existingBooking, deletedByCustomer, cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(existingBooking, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => entitlementCancellationService.CancelBookingAsync(existingBooking.Id, false, "Marketplace booking cancelled.",
                true,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_User_Delete_When_Inside_Cancellation_Window(
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
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
        var deletedBooking = new Shared.Models.Booking
        {
            Id = existingBooking.Id,
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(deletedBooking);

        // Act
        var result = await sut.DeleteAsync(existingBooking, deletedByCustomer, false, null, true, cancellationToken);

        // Assert
        result.ShouldBe(deletedBooking);
        existingBooking.DeletedByCustomer.ShouldBe(deletedByCustomer);
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(existingBooking, deletedByCustomer, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(existingBooking, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_OrganizationOperator_Delete_After_Cancellation_Deadline(
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 30, 0, TimeSpan.Zero);
        var existingBooking = CreateMarketplaceBooking(
            now.AddMinutes(30),
            false,
            ProductPricingCancellationPolicyType.NoCancellation,
            []);
        var deletedBooking = new Shared.Models.Booking
        {
            Id = existingBooking.Id,
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(deletedBooking);

        var result = await sut.DeleteAsync(existingBooking, deletedByCustomer, true, "Operator approved cancellation.", true, cancellationToken);

        result.ShouldBe(deletedBooking);
        existingBooking.DeletedByCustomer.ShouldBe(deletedByCustomer);
        existingBooking.CancellationPolicyOverridden.ShouldBeTrue();
        existingBooking.CancellationOverrideReason.ShouldBe("Operator approved cancellation.");
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(existingBooking, deletedByCustomer, cancellationToken, true))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(existingBooking, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Does_Not_Create_Refund_When_Recurring_Cleanup_Disables_Refund_Creation(
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        CancellationToken cancellationToken)
    {
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingBooking = CreateMarketplaceBooking(
            now.AddHours(4),
            false,
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(180, 100)]);
        existingBooking.RecurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
        };
        var deletedBooking = new Shared.Models.Booking
        {
            Id = existingBooking.Id,
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => bookingRepository.Remove(existingBooking)).Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(deletedBooking);

        var result = await sut.DeleteAsync(existingBooking, deletedByCustomer, false, null, false, cancellationToken);

        result.ShouldBe(deletedBooking);
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(existingBooking, deletedByCustomer, cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(existingBooking, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task UpdateAsync_Does_Not_Recompute_Event_Resources_When_Booking_Window_Has_Not_Changed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IMarketplaceEventResourceService marketplaceEventResourceService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        IBookingRepository bookingRepository,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 3, 21, 9, 0, 0, TimeSpan.Zero);
        var until = from.AddHours(2);
        var existingResource = new Resource
        {
            Id = "resource-1",
            ResourceBookingSlots = [],
        };
        var existingBooking = new BookingEntity
        {
            Id = "booking-1",
            Channel = BookingChannelConstants.Marketplace,
            From = from,
            Until = until,
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-1",
                },
            ],
            InvolvedResources = [existingResource],
            InvolvedOrganizations = [],
            InvolvedLocations = [],
            InvolvedTeams = [],
            MarketplaceBooking = new Database.Entities.MarketplaceBooking
            {
                ProductVersion = new Database.Entities.ProductVersion
                {
                    Id = "product-version-1",
                },
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                },
                PaymentMethod = PaymentMethodConstants.Card,
            },
        };
        var booking = new Shared.Models.Booking
        {
            Id = existingBooking.Id,
            From = from,
            Until = until,
            InvolvedCustomers =
            [
                new Shared.Models.Customer
                {
                    Id = "customer-1",
                },
            ],
            Resources = [],
            InvolvedOrganizations = [],
            InvolvedLocations = [],
            InvolvedTeams = [],
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Id = "product-version-1",
                },
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                },
                PaymentMethod = PaymentMethod.Card,
            },
        };
        var lastModifiedByCustomer = new Customer
        {
            Id = "customer-1",
        };
        var productVersion = new Database.Entities.ProductVersion
        {
            Id = "product-version-1",
            Type = ProductTypeConstants.Event,
            OrganizationTags =
            [
                new OrganizationTag
                {
                    Type = OrganizationTagTypeConstants.Product,
                },
            ],
            Product = new Product
            {
                Organization = new Organization
                {
                    Id = "org-1",
                },
            },
        };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<IReadOnlyList<string>>.That.Contains("customer-1"), true, cancellationToken))
            .Returns([lastModifiedByCustomer]);
        A.CallTo(() => productVersionRepository.GetByIdAsync("product-version-1", cancellationToken)).Returns(productVersion);
        A.CallTo(() => entityMapper.MergeTo(
                booking,
                existingBooking,
                A<IReadOnlyList<Customer>>._,
                A<IReadOnlyList<Organization>>._,
                A<IReadOnlyList<Location>>.That.Matches(locations => locations.Count == 0),
                A<IReadOnlyList<Team>>._,
                A<IReadOnlyList<Resource>>.That.Matches(resources => resources.Count == 1 && resources.First().Id == "resource-1"),
                existingBooking.CreatedByCustomer,
                lastModifiedByCustomer,
                null,
                existingBooking.MarketplaceBooking,
                null))
            .Returns(existingBooking);
        A.CallTo(() => bookingRepository.Update(existingBooking)).Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(booking);

        _ = await sut.UpdateAsync(booking, existingBooking, lastModifiedByCustomer, [], [], null, false, cancellationToken);

        A.CallTo(() => marketplaceEventResourceService.PickEventResourcesAsync(A<DateTimeOffset>._, A<DateTimeOffset>._,
                A<Database.Entities.ProductVersion>._, cancellationToken))
            .MustNotHaveHappened();
    }

    private static BookingEntity CreateMarketplaceBooking(
        DateTimeOffset from,
        bool isPaymentRequired,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
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
                    CancellationPolicyType = cancellationPolicyType,
                    CancellationRefundRules = cancellationRefundRules,
                },
            },
        };

    #region Fixture Builders for Marketplace Booking Modification

    private static BookingEntity CreateConfirmedMarketplaceBooking(
        string id = "booking-1",
        DateTimeOffset? from = null,
        DateTimeOffset? until = null,
        PaymentStatus? paymentStatus = null,
        string? customerId = null,
        string? productVersionId = null,
        ProductPricing? pricing = null,
        int quantity = 1,
        uint entityFrameworkVersion = 3)
    {
        var now = TimeProvider.System.GetUtcNow();
        return new BookingEntity
        {
            Id = id,
            Channel = BookingChannelConstants.Marketplace,
            EntityFrameworkVersion = entityFrameworkVersion,
            From = from ?? now.AddDays(1),
            Until = until ?? now.AddDays(1).AddHours(1),
            MarketplaceBooking = new Database.Entities.MarketplaceBooking
            {
                PaymentStatus = paymentStatus?.ToString() ?? PaymentStatusConstants.Confirmed,
                ProductVersion = new Database.Entities.ProductVersion
                {
                    Id = productVersionId ?? "product-version-1",
                },
                ProductPricing = pricing ?? ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Daily,
                    NumberOfResourcesToBook = 1,
                },
                Quantity = quantity,
                PaymentMethod = PaymentMethodConstants.Card,
            },
            InvolvedCustomers = customerId != null
                ?
                [
                    new Customer
                    {
                        Id = customerId,
                    },
                ]
                : [],
        };
    }

    private static BookingEntity CreateConfirmedSubscriptionOccurrenceBooking(
        string id = "booking-1",
        DateTimeOffset? from = null,
        DateTimeOffset? until = null,
        string? parentRecurringBookingId = null,
        bool hasRecurringInstanceOverrides = false)
    {
        var now = TimeProvider.System.GetUtcNow();
        return new BookingEntity
        {
            Id = id,
            Channel = BookingChannelConstants.Marketplace,
            EntityFrameworkVersion = 3,
            From = from ?? now.AddDays(1),
            Until = until ?? now.AddDays(1).AddHours(1),
            MarketplaceBooking = new Database.Entities.MarketplaceBooking
            {
                PaymentStatus = PaymentStatusConstants.Confirmed,
                ProductVersion = new Database.Entities.ProductVersion
                {
                    Id = "product-version-1",
                },
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Weekly,
                    NumberOfResourcesToBook = 1,
                },
                Quantity = 1,
                PaymentMethod = PaymentMethodConstants.Card,
            },
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-1",
                },
            ],
            HasRecurringInstanceOverrides = hasRecurringInstanceOverrides,
        };
    }

    private static Database.Entities.ProductVersion CreateProductVersion(
        string id = "product-version-1",
        ProductType? type = null,
        Currency currency = Currency.Nzd,
        Organization? organization = null,
        List<ProductPricing>? pricingOptions = null) =>
        new()
        {
            Id = id,
            Type = type?.ToString() ?? ProductTypeConstants.Event,
            Currency = currency.ToString(),
            Product = new Product
            {
                Id = "product-1",
                Organization = organization ?? new Organization
                {
                    Id = "org-1",
                    Type = OrganizationTypeConstants.Marketplace,
                },
            },
            OrganizationTags =
            [
                new OrganizationTag
                {
                    Id = "product-tag-1",
                    Type = OrganizationTagTypeConstants.Product,
                },
            ],
            PricingOptions = pricingOptions ?? [ProductPricing.Empty("pricing-1")],
        };

    private static Resource CreateResource(
        string id = "resource-1",
        string? locationId = null,
        List<string>? tagIds = null) =>
        new()
        {
            Id = id,
            Location = new Location
            {
                Id = locationId ?? "location-1",
            },
            OrganizationTags = tagIds?.Select(tagId => new OrganizationTag
            {
                Id = tagId,
                Type = OrganizationTagTypeConstants.Product,
            }).ToList() ?? [],
        };

    private static MarketplaceBookingModificationRequest CreateModificationRequest(
        string bookingId = "booking-1",
        uint expectedVersion = 3,
        DateTimeOffset? newFrom = null,
        DateTimeOffset? newUntil = null,
        List<string>? resourceIds = null,
        string? reason = null,
        string? actorCustomerId = "customer-1",
        MarketplaceBookingModificationActorKind actorKind = MarketplaceBookingModificationActorKind.Customer)
    {
        var now = TimeProvider.System.GetUtcNow();
        return new MarketplaceBookingModificationRequest(
            bookingId,
            expectedVersion,
            newFrom ?? now.AddDays(2),
            newUntil ?? now.AddDays(2).AddHours(1),
            resourceIds,
            reason,
            actorCustomerId ?? "customer-1",
            actorKind);
    }

    #endregion
}
