using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using EntitlementEntity = Booking.Shared.Database.Entities.Entitlement;
using EntitlementPurchaseEntity = Booking.Shared.Database.Entities.EntitlementPurchase;

namespace Booking.Shared.UnitTests.Services.Entitlements;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class EntitlementPurchaseServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task PersistPendingPurchaseWithoutCreatingBooking(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntitlementInvoiceService invoiceService,
        [Frozen]
        IEntitlementPurchaseModelMapper mapper,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 5,
            EntitlementValidityDays = 30,
            AcceptedPaymentMethods = [PaymentMethod.Card],
        };
        var model = new EntitlementPurchase
        {
            PaymentStatus = PaymentStatusConstants.Pending,
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.Ignored))
            .ReturnsLazily((EntitlementPurchaseEntity purchase) => purchase);
        A.CallTo(() => mapper.Map(A<EntitlementPurchaseEntity>.Ignored)).Returns(model);

        var result = await sut.CreatePendingAsync("customer", "organization", "product-version", pricing, "NZD",
            PaymentMethod.Card, TimeProvider.System.GetUtcNow().AddMinutes(30), null, [], cancellationToken);

        result.PaymentStatus.ShouldBe(PaymentStatusConstants.Pending);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.That.Matches(item => item.EntitlementId == null)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => historyRepository.RefreshForEntitlementPurchaseAsync(A<string>.Ignored, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => invoiceService.GenerateAsync(A<string>.Ignored, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => repositoryFactory.BookingRepository).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task KeepAutoRenewOnlyForSupportedPricing(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntitlementPurchaseModelMapper mapper,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 1,
            EntitlementValidityDays = 7,
            SupportsSubscriptionAutoRenewal = false,
            AcceptedPaymentMethods = [PaymentMethod.Card],
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.Ignored))
            .ReturnsLazily((EntitlementPurchaseEntity purchase) => purchase);
        A.CallTo(() => mapper.Map(A<EntitlementPurchaseEntity>.Ignored)).Returns(new EntitlementPurchase());

        await sut.CreatePendingAsync("customer", "organization", "product-version", pricing, "NZD", PaymentMethod.Card,
            TimeProvider.System.GetUtcNow().AddMinutes(30), TimeProvider.System.GetUtcNow(), null, [], true, cancellationToken);

        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.That.Matches(item => !item.AutoRenew)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectUnsupportedFulfillmentType(EntitlementPurchaseService sut, CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Reservation,
        };

        await Should.ThrowAsync<InvalidOperationException>(() => sut.CreatePendingAsync("customer", "organization", "product-version",
            pricing, "NZD", PaymentMethod.Card, TimeProvider.System.GetUtcNow().AddMinutes(30), null, [], cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectInvalidEntitlementConfiguration(EntitlementPurchaseService sut, CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 0,
            EntitlementValidityDays = 30,
            AcceptedPaymentMethods = [PaymentMethod.Card],
        };

        await Should.ThrowAsync<EntitlementPricingConfigurationInvalid>(() => sut.CreatePendingAsync("customer", "organization", "product-version",
            pricing, "NZD", PaymentMethod.Card, TimeProvider.System.GetUtcNow().AddMinutes(30), null, [], cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectUnacceptedPaymentMethod(EntitlementPurchaseService sut, CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 1,
            EntitlementValidityDays = 7,
            AcceptedPaymentMethods = [PaymentMethod.BankTransfer],
        };

        await Should.ThrowAsync<InvalidOperationException>(() => sut.CreatePendingAsync("customer", "organization", "product-version",
            pricing, "NZD", PaymentMethod.Card, TimeProvider.System.GetUtcNow().AddMinutes(30), null, [], cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task IgnoreAutoRenewWhenPricingSupportsIt(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntitlementPurchaseModelMapper mapper,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 1,
            EntitlementValidityDays = 7,
            SupportsSubscriptionAutoRenewal = true,
            AcceptedPaymentMethods = [PaymentMethod.Card],
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.Ignored))
            .ReturnsLazily((EntitlementPurchaseEntity purchase) => purchase);
        A.CallTo(() => mapper.Map(A<EntitlementPurchaseEntity>.Ignored)).Returns(new EntitlementPurchase());

        await sut.CreatePendingAsync("customer", "organization", "product-version", pricing, "NZD", PaymentMethod.Card,
            TimeProvider.System.GetUtcNow().AddMinutes(30), TimeProvider.System.GetUtcNow(), null, [], true, cancellationToken);

        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.That.Matches(item => !item.AutoRenew)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ConfirmAnAlreadyGrantedPurchaseIdempotently(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementService entitlementService,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            CustomerId = "customer",
            OrganizationId = "organization",
            ProductPricing = ProductPricing.Empty("pricing"),
            ServiceStartAt = TimeProvider.System.GetUtcNow(),
            Currency = "NZD",
            EntitlementId = "entitlement",
            AutoRenew = true,
        };
        var entitlement = new EntitlementModel
        {
            Id = "entitlement",
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => purchaseRepository.GetByIdAsync("purchase", cancellationToken)).Returns(purchase);
        A.CallTo(() => entitlementService.GrantAsync("purchase", "customer", "organization", purchase.ProductPricing,
            purchase.ServiceStartAt, "NZD", true, cancellationToken)).Returns(entitlement);

        var result = await sut.ConfirmAsync("purchase", TimeProvider.System.GetUtcNow(), cancellationToken);

        result.ShouldBe(entitlement);
        A.CallTo(() => entitlementService.GrantAsync("purchase", "customer", "organization", purchase.ProductPricing,
            purchase.ServiceStartAt, "NZD", true, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ExpireLatePaymentConfirmationWithoutGrantingCredits(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntitlementPurchaseModelMapper mapper,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            PaymentStatus = PaymentStatusConstants.Pending,
            PaymentExpiry = TimeProvider.System.GetUtcNow().AddMinutes(-1),
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.GetByIdAsync("purchase", cancellationToken)).Returns(purchase);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());
        A.CallTo(() => mapper.Map(A<EntitlementPurchaseEntity>.Ignored)).Returns(new EntitlementPurchase());

        var result = await sut.UpdatePaymentStatusAsync("purchase", PaymentStatus.Confirmed, TimeProvider.System.GetUtcNow(), cancellationToken);

        purchase.PaymentStatus.ShouldBe(PaymentStatusConstants.Expired);
        purchase.EntitlementId.ShouldBeNull();
        result.ShouldNotBeNull();
        A.CallTo(() => historyRepository.RefreshForEntitlementPurchaseAsync("purchase", cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task IgnoreMissingPurchaseWhenUpdatingPaymentStatus(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => purchaseRepository.GetByIdAsync("missing", cancellationToken)).Returns((EntitlementPurchaseEntity?)null);

        var result = await sut.UpdatePaymentStatusAsync("missing", PaymentStatus.Confirmed, TimeProvider.System.GetUtcNow(), cancellationToken);

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectCardConfirmationWithoutStripeContext(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementPurchaseModelMapper mapper,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            PaymentStatus = PaymentStatusConstants.Pending,
            PaymentMethod = PaymentMethodConstants.Card,
            PaymentExpiry = TimeProvider.System.GetUtcNow().AddMinutes(30),
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => purchaseRepository.GetByIdAsync("purchase", cancellationToken)).Returns(purchase);
        A.CallTo(() => mapper.Map(A<EntitlementPurchaseEntity>.Ignored)).Returns(new EntitlementPurchase());

        var result = await sut.UpdatePaymentStatusAsync("purchase", PaymentStatus.Confirmed, TimeProvider.System.GetUtcNow(), cancellationToken);

        result.ShouldNotBeNull();
        purchase.PaymentStatus.ShouldBe(PaymentStatusConstants.Pending);
        purchase.EntitlementId.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ExpirePendingPurchasesAndCancelTheirPayment(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementPurchasePaymentCancellationService paymentCancellationService,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            PaymentStatus = PaymentStatusConstants.Pending,
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.GetExpiredPendingAsync(A<DateTimeOffset>.Ignored, cancellationToken)).Returns([purchase]);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());

        var count = await sut.ExpirePendingAsync(cancellationToken);

        count.ShouldBe(1);
        purchase.PaymentStatus.ShouldBe(PaymentStatusConstants.Expired);
        A.CallTo(() => paymentCancellationService.CancelAsync(purchase, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => historyRepository.RefreshForEntitlementPurchaseAsync("purchase", cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task KeepLocalExpiryWhenPaymentCancellationFails(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementPurchasePaymentCancellationService paymentCancellationService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            PaymentStatus = PaymentStatusConstants.Pending,
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.GetExpiredPendingAsync(A<DateTimeOffset>.Ignored, cancellationToken)).Returns([purchase]);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());
        A.CallTo(() => paymentCancellationService.CancelAsync(purchase, cancellationToken)).ThrowsAsync(new InvalidOperationException());

        var count = await sut.ExpirePendingAsync(cancellationToken);

        count.ShouldBe(1);
        purchase.PaymentStatus.ShouldBe(PaymentStatusConstants.Expired);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task IgnoreConfirmationAfterTerminalPaymentStatus(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementPurchaseModelMapper mapper,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            PaymentStatus = PaymentStatusConstants.Expired,
            PaymentExpiry = TimeProvider.System.GetUtcNow().AddMinutes(-1),
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => purchaseRepository.GetByIdAsync("purchase", cancellationToken)).Returns(purchase);
        A.CallTo(() => mapper.Map(A<EntitlementPurchaseEntity>.Ignored)).Returns(new EntitlementPurchase());

        var result = await sut.UpdatePaymentStatusAsync("purchase", PaymentStatus.Confirmed, TimeProvider.System.GetUtcNow(), cancellationToken);

        result.ShouldNotBeNull();
        purchase.PaymentStatus.ShouldBe(PaymentStatusConstants.Expired);
        A.CallTo(() => repositoryFactory.UnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task PropagateInvoiceGenerationFailureAfterPersistingPendingPurchase(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntitlementInvoiceService invoiceService,
        [Frozen]
        IEntitlementPurchaseModelMapper mapper,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 1,
            EntitlementValidityDays = 7,
            AcceptedPaymentMethods = [PaymentMethod.Card],
        };
        var invoiceException = new InvalidOperationException("invoice failed");
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.Ignored))
            .ReturnsLazily((EntitlementPurchaseEntity purchase) => purchase);
        A.CallTo(() => mapper.Map(A<EntitlementPurchaseEntity>.Ignored)).Returns(new EntitlementPurchase());
        A.CallTo(() => invoiceService.GenerateAsync(A<string>.Ignored, cancellationToken)).ThrowsAsync(invoiceException);

        var thrown = await Should.ThrowAsync<InvalidOperationException>(() => sut.CreatePendingAsync("customer", "organization",
            "product-version", pricing, "NZD", PaymentMethod.Card, TimeProvider.System.GetUtcNow().AddMinutes(30), null, [], cancellationToken));

        thrown.ShouldBe(invoiceException);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task MarkSourceEntitlementWhenRenewalExpires(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchasePaymentCancellationService paymentCancellationService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var renewal = new EntitlementPurchaseEntity
        {
            Id = "renewal",
            PaymentStatus = PaymentStatusConstants.Pending,
            RenewalOfPurchaseId = "source-purchase",
        };
        var sourceEntitlement = new EntitlementEntity
        {
            Id = "entitlement",
        };
        var sourcePurchase = new EntitlementPurchaseEntity
        {
            Id = "source-purchase",
            EntitlementId = "entitlement",
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.GetExpiredPendingAsync(A<DateTimeOffset>.Ignored, cancellationToken)).Returns([renewal]);
        A.CallTo(() => purchaseRepository.GetByIdAsync("source-purchase", cancellationToken)).Returns(sourcePurchase);
        A.CallTo(() => entitlementRepository.GetByIdAsync("entitlement", cancellationToken)).Returns(sourceEntitlement);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());

        await sut.ExpirePendingAsync(cancellationToken);

        sourceEntitlement.RenewalFailureReason.ShouldBe("Payment was not confirmed before the entitlement purchase deadline.");
        sourceEntitlement.NextRenewalAt.ShouldBeNull();
        A.CallTo(() => paymentCancellationService.CancelAsync(renewal, cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
