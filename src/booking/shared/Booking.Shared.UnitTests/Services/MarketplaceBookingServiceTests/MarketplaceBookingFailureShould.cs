using System.Data;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using MarketplaceBookingFailureCategoryConstants = Booking.Shared.Models.MarketplaceBookingFailureCategoryConstants;
using MarketplaceBookingFailureCustomerActionConstants = Booking.Shared.Models.MarketplaceBookingFailureCustomerActionConstants;
using MarketplaceBookingFailureFinalization = Booking.Shared.Models.MarketplaceBookingFailureFinalization;
using MarketplaceBookingFailureScopeConstants = Booking.Shared.Models.MarketplaceBookingFailureScopeConstants;
using ProductVersion = Booking.Shared.Models.ProductVersion;
using ResourceSlotClaimResult = Booking.Shared.Models.ResourceSlotClaimResult;
using Offering = Api.Shared.Services.Models.Offering;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingFailureShould
{
    private static (
        Shared.Models.Booking Booking,
        Customer Customer,
        Database.Entities.ProductVersion ProductVersion,
        ProductPricing Pricing) BuildEventBookingScenario()
    {
        var from = new DateTimeOffset(2026, 7, 22, 9, 0, 0, TimeSpan.Zero);
        var organization = new Organization
        {
            Id = "org-1", Type = OrganizationTypeConstants.Marketplace, Offering = new Offering { Code = OfferingCode.SpacesFreeTierV1 }
        };
        var customer = new Customer { Id = "customer-1" };
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.OneTime,
            BookingCadence = ProductPricingCadence.OneTime,
            AcceptedPaymentMethods = [PaymentMethod.Card],
            BillingMode = ProductPricingBillingMode.Upfront,
            MaxAllowedResourcesLockTimePaidViaCard = 15,
            NumberOfResourcesToBook = 1
        };
        var productVersion = new Database.Entities.ProductVersion
        {
            Id = "product-version-1",
            Type = ProductTypeConstants.Event,
            PricingOptions = [pricing],
            OrganizationTags = [new OrganizationTag { Type = OrganizationTagTypeConstants.Product }],
            Product = new Product { Organization = organization },
            Currency = CurrencyConstants.Nzd
        };
        var booking = new Shared.Models.Booking
        {
            From = from,
            Until = from.AddHours(1),
            InvolvedCustomers = [new Shared.Models.Customer { Id = customer.Id }],
            Resources = [],
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion { Id = productVersion.Id }, ProductPricing = pricing, PaymentMethod = PaymentMethod.Card
            }
        };
        return (booking, customer, productVersion, pricing);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Finalize_An_Availability_Failure_And_Set_FailureId_When_Atomic_Claim_Returns_Conflict(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IMarketplaceEventResourceService marketplaceEventResourceService,
        [Frozen] IProductVersionHelperService productVersionHelperService,
        [Frozen] IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
        [Frozen] IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen] ILogger<MarketplaceBookingService> logger,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        IMarketplaceBookingRepository marketplaceBookingRepository,
        IBookingRepository bookingRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var (booking, customer, productVersion, pricing) = BuildEventBookingScenario();
        var from = booking.From;
        var resource = new Resource { Id = "resource-1" };
        var marketplaceBookingEntity = new Database.Entities.MarketplaceBooking { Id = "marketplace-booking-1" };
        var bookingEntity = new Database.Entities.Booking { Id = "booking-1" };
        var failure = new MarketplaceBookingFailure { Id = "failure-1" };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, IsolationLevel.Serializable, cancellationToken)).Returns(transaction);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<IReadOnlyList<string>>.That.Contains(customer.Id), true, cancellationToken))
            .Returns([customer]);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersion.Id, cancellationToken)).Returns(productVersion);
        A.CallTo(() => productVersionHelperService.FindMatchingPricing(A<IReadOnlyList<ProductPricing>>._, pricing)).Returns(pricing);
        var selectedDate = default(DateOnly);
        A.CallTo(() => marketplaceBookingAvailableDaysService.IsAvailable(A<ProductPricing>._, A<DateTimeOffset>._, out selectedDate)).Returns(true);
        A.CallTo(() => marketplaceEventResourceService.PickEventResourcesAsync(from, from.AddHours(1), productVersion, cancellationToken))
            .Returns([resource]);
        A.CallTo(() => entityMapper.MapTo(A<MarketplaceBooking>._, customer, null, productVersion, null)).Returns(marketplaceBookingEntity);
        A.CallTo(() => marketplaceBookingRepository.Add(marketplaceBookingEntity)).Returns(marketplaceBookingEntity);
        A.CallTo(() => entityMapper.MapTo(
                booking,
                A<IReadOnlyList<Customer>>._,
                A<IReadOnlyList<Organization>>._,
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
            .Returns(ResourceSlotClaimResult.Conflict([resource.Id]));
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(A<MarketplaceBookingFailureFinalization>._, cancellationToken))
            .Returns(failure);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var exception = await Should.ThrowAsync<MarketplaceBookingAvailabilityConflict>(() =>
            sut.AddAsync(booking, customer, [], [], null, true, true, false, cancellationToken));

        exception.FailureId.ShouldBe(failure.Id);
        exception.UnavailableResourceIds.ShouldContain(resource.Id);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(f =>
                    f.Category == MarketplaceBookingFailureCategoryConstants.AvailabilityConflict &&
                    f.Scope == MarketplaceBookingFailureScopeConstants.OneTimeBooking &&
                    f.CustomerAction == MarketplaceBookingFailureCustomerActionConstants.Rebook),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.RollbackAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => repositoryFactory.ResetChangeTracker()).MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "availability claim conflicted").MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Finalize_An_Availability_Failure_And_Set_FailureId_When_No_Resources_Are_Auto_Assigned(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IMarketplaceEventResourceService marketplaceEventResourceService,
        [Frozen] IProductVersionHelperService productVersionHelperService,
        [Frozen] IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
        [Frozen] IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceBookingService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        CancellationToken cancellationToken)
    {
        var (booking, customer, productVersion, pricing) = BuildEventBookingScenario();
        var from = booking.From;
        var failure = new MarketplaceBookingFailure { Id = "failure-2" };

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, IsolationLevel.Serializable, cancellationToken)).Returns(transaction);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<IReadOnlyList<string>>.That.Contains(customer.Id), true, cancellationToken))
            .Returns([customer]);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersion.Id, cancellationToken)).Returns(productVersion);
        A.CallTo(() => productVersionHelperService.FindMatchingPricing(A<IReadOnlyList<ProductPricing>>._, pricing)).Returns(pricing);
        var selectedDate = default(DateOnly);
        A.CallTo(() => marketplaceBookingAvailableDaysService.IsAvailable(A<ProductPricing>._, A<DateTimeOffset>._, out selectedDate)).Returns(true);
        // Auto-assign returns no resources → triggers no-capacity path
        A.CallTo(() => marketplaceEventResourceService.PickEventResourcesAsync(from, from.AddHours(1), productVersion, cancellationToken))
            .Returns([]);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(A<MarketplaceBookingFailureFinalization>._, cancellationToken))
            .Returns(failure);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var exception = await Should.ThrowAsync<MarketplaceBookingAvailabilityConflict>(() =>
            sut.AddAsync(booking, customer, [], [], null, true, true, false, cancellationToken));

        exception.FailureId.ShouldBe(failure.Id);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(f =>
                    f.Category == MarketplaceBookingFailureCategoryConstants.AvailabilityConflict &&
                    f.Scope == MarketplaceBookingFailureScopeConstants.OneTimeBooking),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.RollbackAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
