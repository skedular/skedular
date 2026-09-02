using System.Data;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using EntitlementPurchaseEntity = Booking.Shared.Database.Entities.EntitlementPurchase;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementCancellationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CancelBookingAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task CreateACancellationRefundForEligibleUnusedCredits(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseRepository entitlementPurchaseRepository,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        IMarketplacePurchaseHistoryRepository marketplacePurchaseHistoryRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IRandomHelper randomHelper,
        EntitlementCancellationService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 8, 16, 0, 0, 0, TimeSpan.Zero);
        var entitlement = new Entitlement
        {
            Id = "entitlement-1",
            PurchaseReference = "purchase-1",
            Status = EntitlementStatus.Active,
            GrantedQuantity = 4,
            ExpiresAt = now.AddDays(10),
            LedgerEntries = [],
        };
        var purchase = new EntitlementPurchaseEntity
        {
            Id = entitlement.PurchaseReference,
            PaymentStatus = PaymentStatusConstants.Confirmed,
            ProductPricing = new ProductPricing(
                "pricing-1",
                0,
                ListingMetadata.Empty,
                ProductPricingCadence.Daily,
                100m,
                true,
                false,
                [],
                ProductPricingBillingMode.Upfront,
                null,
                null,
                10,
                10,
                1,
                ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
                []),
        };
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            RefundAmount = 50,
        };
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(entitlementPurchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(marketplacePurchaseHistoryRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, IsolationLevel.Serializable, cancellationToken)).Returns(transaction);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)).Returns(entitlement);
        A.CallTo(() => entitlementPurchaseRepository.GetByIdAsync(purchase.Id, cancellationToken)).Returns(purchase);
        A.CallTo(() => creditLedgerService.GetAvailableCredits(entitlement)).Returns(2);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("ledger-1", "refund-link-1");
        A.CallTo(() => marketplaceRefundService.CreateEntitlementCancellationRefundAsync(entitlement, purchase, 2, cancellationToken))
            .Returns(refund);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        CreditLedgerEntry? addedEntry = null;
        A.CallTo(() => entitlementRepository.AddLedgerEntry(A<CreditLedgerEntry>._))
            .Invokes((CreditLedgerEntry entry) => addedEntry = entry)
            .ReturnsLazily((CreditLedgerEntry entry) => entry);

        await sut.CancelEntitlementAsync(entitlement.Id, "customer request", cancellationToken);

        Assert.Equal(EntitlementStatus.Cancelled, entitlement.Status);
        Assert.Single(entitlement.RefundLinks);
        Assert.Equal(CreditLedgerTransactionType.Expired.ToPersistedValue(), addedEntry?.TransactionType);
        A.CallTo(() => marketplaceRefundService.CreateEntitlementCancellationRefundAsync(entitlement, purchase, 2, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task PersistAForfeitureWhenCreditRestorationIsNotAllowed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRandomHelper randomHelper,
        EntitlementCancellationService sut,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var entitlement = new Entitlement
        {
            Id = "entitlement-1",
            LedgerEntries = [],
        };
        var consumed = new CreditLedgerEntry
        {
            Id = "consumed",
            EntitlementId = entitlement.Id,
            BookingId = "booking-1",
            Quantity = 2,
            TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
            Entitlement = entitlement,
        };
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => entitlementRepository.GetConsumedByBookingIdAsync("booking-1", cancellationToken)).Returns(consumed);
        A.CallTo(() => randomHelper.Generate()).Returns("forfeited-1");
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        CreditLedgerEntry? added = null;
        A.CallTo(() => entitlementRepository.AddLedgerEntry(A<CreditLedgerEntry>._)).Invokes((CreditLedgerEntry entry) => added = entry)
            .ReturnsLazily((CreditLedgerEntry entry) => entry);

        await sut.CancelBookingAsync("booking-1", false, "outside restoration window", cancellationToken);

        Assert.Equal(CreditLedgerTransactionType.Forfeited.ToPersistedValue(), added?.TransactionType);
        Assert.Equal(2, added?.Quantity);
        Assert.Equal("outside restoration window", added?.Metadata?.Reason);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ReturnTheExistingReleaseOnARepeatRequest(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntitlementModelMapper entitlementModelMapper,
        EntitlementCancellationService sut,
        CancellationToken cancellationToken)
    {
        var entitlement = new Entitlement
        {
            Id = "entitlement-1",
            LedgerEntries = [],
        };
        var consumed = new CreditLedgerEntry
        {
            Id = "consumed",
            EntitlementId = entitlement.Id,
            BookingId = "booking-1",
            Quantity = 1,
            TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
            Entitlement = entitlement,
        };
        var released = new CreditLedgerEntry
        {
            Id = "released",
            EntitlementId = entitlement.Id,
            BookingId = "booking-1",
            Quantity = 1,
            TransactionType = CreditLedgerTransactionType.Released.ToPersistedValue(),
            ReferenceKey = "booking:booking-1:released",
        };
        entitlement.LedgerEntries.Add(released);
        A.CallTo(() => entitlementModelMapper.Map(released)).Returns(new CreditLedgerEntryModel
        {
            Id = released.Id,
        });
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => entitlementRepository.GetConsumedByBookingIdAsync("booking-1", cancellationToken)).Returns(consumed);

        var result = await sut.CancelBookingAsync("booking-1", true, "customer request", cancellationToken);

        Assert.Equal(released.Id, result?.Id);
        A.CallTo(() => entitlementRepository.AddLedgerEntry(A<CreditLedgerEntry>._)).MustNotHaveHappened();
    }
}
