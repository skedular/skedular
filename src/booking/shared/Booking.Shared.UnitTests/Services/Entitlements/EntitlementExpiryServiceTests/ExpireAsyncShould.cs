using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using EntitlementPurchase = Booking.Shared.Database.Entities.EntitlementPurchase;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementExpiryServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ExpireAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task AddOneForfeitureTransitionForAnExpiredNonRefundableEntitlement(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementExpiryService sut,
        CancellationToken cancellationToken)

    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => randomHelper.Generate()).Returns("ledger-1");
        A.CallTo(() => creditLedgerService.GetAvailableCredits(A<Entitlement>._)).Returns(3);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 8, 12, 0, 0, 0, TimeSpan.Zero));
        var entitlement = new Entitlement
        {
            Id = "entitlement-1",
            Status = EntitlementStatus.Active,
            ExpiresAt = new DateTimeOffset(2026, 8, 11, 0, 0, 0, TimeSpan.Zero),
            GrantedQuantity = 3,
        };
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)).Returns(entitlement);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        CreditLedgerEntry? addedEntry = null;
        A.CallTo(() => entitlementRepository.AddLedgerEntry(A<CreditLedgerEntry>._)).Invokes((CreditLedgerEntry entry) => addedEntry = entry)
            .ReturnsLazily((CreditLedgerEntry entry) => entry);

        var result = await sut.ExpireAsync(entitlement.Id, cancellationToken);

        Assert.True(result);
        Assert.Equal(EntitlementStatus.Expired, entitlement.Status);
        Assert.NotNull(addedEntry);
        Assert.Equal(CreditLedgerTransactionType.Forfeited.ToPersistedValue(), addedEntry!.TransactionType);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task CreateProratedRefundForUnusedConfirmedCredits(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseRepository entitlementPurchaseRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementExpiryService sut,
        CancellationToken cancellationToken)

    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("ledger-1", "refund-link-1");
        A.CallTo(() => creditLedgerService.GetAvailableCredits(A<Entitlement>._)).Returns(2);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 8, 12, 0, 0, 0, TimeSpan.Zero));
        var entitlement = new Entitlement
        {
            Id = "entitlement-1",
            PurchaseReference = "booking-1",
            Status = EntitlementStatus.Active,
            ExpiresAt = new DateTimeOffset(2026, 8, 11, 0, 0, 0, TimeSpan.Zero),
            GrantedQuantity = 4,
            NetPurchaseAmount = 100,
            LedgerEntries =
            [
                new CreditLedgerEntry
                {
                    Id = "consumed-1",
                    Quantity = 2,
                    TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
                },
            ],
        };
        var purchase = new EntitlementPurchase
        {
            Id = "booking-1",
            PaymentStatus = PaymentStatusConstants.Confirmed,
            ProductPricing = ProductPricing.Empty("pricing-1") with
            {
                CancellationPolicyType = ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
                CancellationRefundRules = [new ProductPricingCancellationRefundRule(0, 100)],
            },
        };
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            RefundAmount = 50,
        };
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)).Returns(entitlement);
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(entitlementPurchaseRepository);
        A.CallTo(() => entitlementPurchaseRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(purchase);
        A.CallTo(() => marketplaceRefundService.CreateEntitlementExpiryRefundAsync(entitlement, purchase, 2, cancellationToken)).Returns(refund);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.ExpireAsync(entitlement.Id, cancellationToken);

        Assert.Single(entitlement.RefundLinks);
        A.CallTo(() => marketplaceRefundService.CreateEntitlementExpiryRefundAsync(entitlement, purchase, 2, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DoNotCreateRefundForUnconfirmedPayment(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        EntitlementExpiryService sut,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)

    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => randomHelper.Generate()).Returns("ledger-1");
        A.CallTo(() => creditLedgerService.GetAvailableCredits(A<Entitlement>._)).Returns(1);
        var entitlement = new Entitlement
        {
            Id = "entitlement-pending",
            PurchaseReference = "booking-pending",
            Status = EntitlementStatus.Active,
            ExpiresAt = new DateTimeOffset(2026, 8, 11, 0, 0, 0, TimeSpan.Zero),
            GrantedQuantity = 1,
        };
        var booking = new Database.Entities.Booking
        {
            Id = "booking-pending",
            MarketplaceBooking = new MarketplaceBooking
            {
                PaymentStatus = PaymentStatusConstants.Pending,
            },
        };
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)).Returns(entitlement);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-pending", cancellationToken)).Returns(booking);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.ExpireAsync(entitlement.Id, cancellationToken);

        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(A<Database.Entities.Booking>._, A<Customer?>._,
                cancellationToken))
            .MustNotHaveHappened();
        Assert.Empty(entitlement.RefundLinks);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task KeepLocalExpiryWhenRefundProjectionFails(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementExpiryService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => randomHelper.Generate()).Returns("ledger-1");
        A.CallTo(() => creditLedgerService.GetAvailableCredits(A<Entitlement>._)).Returns(1);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 8, 12, 0, 0, 0, TimeSpan.Zero));
        var entitlement = new Entitlement
        {
            Id = "entitlement-failure",
            PurchaseReference = "booking-failure",
            Status = EntitlementStatus.Active,
            ExpiresAt = new DateTimeOffset(2026, 8, 11, 0, 0, 0, TimeSpan.Zero),
            GrantedQuantity = 1,
        };
        var booking = new Database.Entities.Booking
        {
            Id = "booking-failure",
            MarketplaceBooking = new MarketplaceBooking
            {
                PaymentStatus = PaymentStatusConstants.Confirmed,
            },
        };
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)).Returns(entitlement);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-failure", cancellationToken)).Returns(booking);
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(booking, null, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("provider unavailable"));
        CreditLedgerEntry? addedEntry = null;
        A.CallTo(() => entitlementRepository.AddLedgerEntry(A<CreditLedgerEntry>._))
            .ReturnsLazily((CreditLedgerEntry entry) => addedEntry = entry);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.ExpireAsync(entitlement.Id, cancellationToken);

        Assert.True(result);
        Assert.Equal(EntitlementStatus.Expired, entitlement.Status);
        addedEntry.ShouldNotBeNull();
        addedEntry!.Metadata!.RefundError.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DoNotReprojectAnExistingRefundLink(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IRandomHelper randomHelper,
        EntitlementExpiryService sut,
        IEntitlementRepository entitlementRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)

    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => randomHelper.Generate()).Returns("ledger-1");
        var entitlement = new Entitlement
        {
            Id = "entitlement-existing",
            PurchaseReference = "booking-existing",
            Status = EntitlementStatus.Active,
            ExpiresAt = TimeProvider.System.GetUtcNow().AddMinutes(-1),
            GrantedQuantity = 1,
            RefundLinks =
            [
                new EntitlementRefundLink
                {
                    Id = "link-1",
                    MarketplaceRefundId = "refund-1",
                },
            ],
        };
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)).Returns(entitlement);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.ExpireAsync(entitlement.Id, cancellationToken);

        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(A<Database.Entities.Booking>._, A<Customer?>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
