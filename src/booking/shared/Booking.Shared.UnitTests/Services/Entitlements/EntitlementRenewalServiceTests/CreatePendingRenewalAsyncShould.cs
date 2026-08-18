using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using EntitlementPurchase = Booking.Shared.Models.Entitlements.EntitlementPurchase;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementRenewalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class CreatePendingRenewalAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task CreatePendingPurchaseUsingCurrentCompatiblePricing(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IProductVersionRepository productVersionRepository,
        [Frozen]
        IProductVersionHelperService productVersionHelperService,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementRenewalService sut,
        string entitlementId,
        string purchaseId,
        string productVersionId,
        DateTimeOffset paymentExpiry,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 5,
            EntitlementValidityDays = 30,
            SupportsSubscriptionAutoRenewal = true,
        };
        var entitlement = new Entitlement
        {
            Id = entitlementId,
            PurchaseReference = purchaseId,
            PricingId = pricing.Id,
            Status = EntitlementStatus.Active,
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(1),
            AutoRenew = true,
        };
        var sourcePurchase = new Database.Entities.EntitlementPurchase
        {
            Id = purchaseId,
            PaymentStatus = PaymentStatusConstants.Confirmed,
            CustomerId = "customer-1",
            OrganizationId = "organization-1",
            ProductVersionId = productVersionId,
            PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
            Currency = "NZD",
            ProductPricing = pricing,
            ProductVersion = new ProductVersion
            {
                Id = productVersionId,
                ProductId = "product-1",
                PricingOptions = [pricing],
            },
        };
        var renewal = new EntitlementPurchase
        {
            Id = "renewal-1",
        };

        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => productVersionHelperService.FindMatchingPricing(A<IEnumerable<ProductPricing>>._, pricing)).Returns(pricing);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlementId, cancellationToken)).Returns(entitlement);
        A.CallTo(() => purchaseRepository.GetByIdAsync(purchaseId, cancellationToken)).Returns(sourcePurchase);
        A.CallTo(() => productVersionRepository.GetCurrentByProductIdAsync("product-1", cancellationToken)).Returns(sourcePurchase.ProductVersion);
        A.CallTo(() => purchaseRepository.GetByRenewalReferenceAsync(A<string>._, cancellationToken))
            .Returns((Database.Entities.EntitlementPurchase?)null);
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                "customer-1",
                "organization-1",
                productVersionId,
                pricing,
                "NZD",
                PaymentMethod.BankTransfer,
                paymentExpiry,
                entitlement.ExpiresAt,
                sourcePurchase.CheckoutReturnUrl,
                A<IReadOnlyCollection<string>>._,
                true,
                cancellationToken))
            .Returns(renewal);
        A.CallTo(() => purchaseRepository.GetByIdAsync(renewal.Id, cancellationToken)).Returns(new Database.Entities.EntitlementPurchase
        {
            Id = renewal.Id,
            PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
        });

        var result = await sut.CreatePendingRenewalAsync(entitlementId, paymentExpiry, cancellationToken);

        Assert.Same(renewal, result);
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                "customer-1",
                "organization-1",
                productVersionId,
                pricing,
                "NZD",
                PaymentMethod.BankTransfer,
                paymentExpiry,
                entitlement.ExpiresAt,
                sourcePurchase.CheckoutReturnUrl,
                A<IReadOnlyCollection<string>>._,
                true,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task SkipNonRenewingPricing(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementRenewalService sut,
        string entitlementId,
        string purchaseId,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            SupportsSubscriptionAutoRenewal = false,
        };
        var entitlement = new Entitlement
        {
            Id = entitlementId,
            PurchaseReference = purchaseId,
            Status = EntitlementStatus.Active,
        };
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlementId, cancellationToken)).Returns(entitlement);
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken))
            .Returns(new Database.Entities.EntitlementPurchase
            {
                ProductPricing = pricing,
            });

        Assert.Null(await sut.CreatePendingRenewalAsync(entitlementId, TimeProvider.System.GetUtcNow().AddHours(1), cancellationToken));
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<ProductPricing>._,
                A<string>._,
                A<PaymentMethod>._,
                A<DateTimeOffset>._,
                A<string?>._,
                A<IReadOnlyCollection<string>>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task SkipEntitlementScheduledToCancelAtPeriodEnd(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementRenewalService sut,
        string entitlementId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlementId, cancellationToken)).Returns(new Entitlement
        {
            Id = entitlementId,
            Status = EntitlementStatus.Active,
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(1),
            AutoRenew = false,
            CancelAtPeriodEnd = true,
        });

        Assert.Null(await sut.CreatePendingRenewalAsync(entitlementId, TimeProvider.System.GetUtcNow().AddHours(1), cancellationToken));
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                A<string>._, A<string>._, A<string>._, A<ProductPricing>._, A<string>._, A<PaymentMethod>._,
                A<DateTimeOffset>._, A<string?>._, A<IReadOnlyCollection<string>>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task SkipMissingOrInactiveEntitlement(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementRenewalService sut,
        string entitlementId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlementId, cancellationToken)).Returns((Entitlement?)null);

        Assert.Null(await sut.CreatePendingRenewalAsync(entitlementId, TimeProvider.System.GetUtcNow().AddHours(1), cancellationToken));
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<ProductPricing>._,
                A<string>._,
                A<PaymentMethod>._,
                A<DateTimeOffset>._,
                A<string?>._,
                A<IReadOnlyCollection<string>>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task SkipWhenCurrentPricingNoLongerSupportsTokenRenewal(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementRenewalService sut,
        string entitlementId,
        string purchaseId,
        string productVersionId,
        CancellationToken cancellationToken)
    {
        var originalPricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 5,
            EntitlementValidityDays = 30,
            SupportsSubscriptionAutoRenewal = true,
        };
        var currentPricing = originalPricing with
        {
            SupportsSubscriptionAutoRenewal = false,
        };
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlementId, cancellationToken)).Returns(new Entitlement
        {
            Id = entitlementId,
            PurchaseReference = purchaseId,
            PricingId = originalPricing.Id,
            Status = EntitlementStatus.Active,
        });
        A.CallTo(() => purchaseRepository.GetByIdAsync(purchaseId, cancellationToken)).Returns(new Database.Entities.EntitlementPurchase
        {
            Id = purchaseId,
            ProductPricing = originalPricing,
            ProductVersion = new ProductVersion
            {
                Id = productVersionId,
                PricingOptions = [currentPricing],
            },
        });

        Assert.Null(await sut.CreatePendingRenewalAsync(entitlementId, TimeProvider.System.GetUtcNow().AddHours(1), cancellationToken));
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<ProductPricing>._,
                A<string>._,
                A<PaymentMethod>._,
                A<DateTimeOffset>._,
                A<string?>._,
                A<IReadOnlyCollection<string>>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task SkipWhenCurrentPricingIsReservationBased(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementRenewalService sut,
        string entitlementId,
        string purchaseId,
        CancellationToken cancellationToken)
    {
        var originalPricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 5,
            EntitlementValidityDays = 30,
            SupportsSubscriptionAutoRenewal = true,
        };
        var currentReservationPricing = originalPricing with
        {
            FulfillmentType = ProductPricingFulfillmentType.Reservation,
        };

        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlementId, cancellationToken)).Returns(new Entitlement
        {
            Id = entitlementId,
            PurchaseReference = purchaseId,
            PricingId = originalPricing.Id,
            Status = EntitlementStatus.Active,
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(1),
        });
        A.CallTo(() => purchaseRepository.GetByIdAsync(purchaseId, cancellationToken)).Returns(new Database.Entities.EntitlementPurchase
        {
            Id = purchaseId,
            ProductPricing = originalPricing,
            ProductVersion = new ProductVersion
            {
                Id = "product-version-1",
                PricingOptions = [currentReservationPricing],
            },
        });

        Assert.Null(await sut.CreatePendingRenewalAsync(entitlementId, TimeProvider.System.GetUtcNow().AddHours(1), cancellationToken));
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<ProductPricing>._,
                A<string>._,
                A<PaymentMethod>._,
                A<DateTimeOffset>._,
                A<string?>._,
                A<IReadOnlyCollection<string>>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task SkipExpiredActiveEntitlement(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        [Frozen]
        TimeProvider timeProvider,
        EntitlementRenewalService sut,
        string entitlementId,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 8, 12, 0, 0, 0, TimeSpan.Zero);
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlementId, cancellationToken)).Returns(new Entitlement
        {
            Id = entitlementId,
            Status = EntitlementStatus.Active,
            ExpiresAt = now.AddTicks(-1),
        });

        Assert.Null(await sut.CreatePendingRenewalAsync(entitlementId, now.AddHours(1), cancellationToken));
        A.CallTo(() => entitlementPurchaseService.CreatePendingAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<ProductPricing>._,
                A<string>._,
                A<PaymentMethod>._,
                A<DateTimeOffset>._,
                A<string?>._,
                A<IReadOnlyCollection<string>>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
