using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using EntitlementPurchaseEntity = Booking.Shared.Database.Entities.EntitlementPurchase;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementPurchaseServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class ConfirmStripeSubscriptionInvoiceAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Ignore_a_repeated_invoice_when_the_renewal_purchase_already_exists(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IEntitlementService entitlementService,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 5,
            EntitlementValidityDays = 30,
        };
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            AutoRenew = true,
            PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
            EntitlementId = "initial-entitlement",
            CustomerId = "customer",
            OrganizationId = "organization",
            ProductVersionId = "product-version",
            ProductPricing = pricing,
            Currency = "NZD",
            StripeCustomerId = "cus_1",
            StripeSubscriptionId = "sub_1",
        };
        var invoiceId = "in_1";
        var renewal = new EntitlementPurchaseEntity
        {
            Id = $"stripe-subscription-invoice:acct_1:{invoiceId}",
            EntitlementId = "renewed-entitlement",
            ProductPricing = pricing,
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => purchaseRepository.GetByIdAsync("purchase", cancellationToken)).Returns(purchase);
        A.CallTo(() => purchaseRepository.GetByIdAsync(renewal.Id, cancellationToken)).Returns(renewal);
        A.CallTo(() => entitlementService.GrantAsync(
                renewal.Id,
                purchase.CustomerId,
                purchase.OrganizationId,
                pricing,
                A<DateTimeOffset>._,
                purchase.Currency,
                cancellationToken))
            .Returns(new EntitlementModel
            {
                Id = renewal.EntitlementId,
            });

        await sut.ConfirmStripeSubscriptionInvoiceAsync(
            purchase.Id,
            "acct_1",
            invoiceId,
            DateTimeOffset.UtcNow,
            "pi_1",
            cancellationToken);

        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>._)).MustNotHaveHappened();
        A.CallTo(() => entitlementService.GrantAsync(
                renewal.Id,
                purchase.CustomerId,
                purchase.OrganizationId,
                pricing,
                A<DateTimeOffset>._,
                purchase.Currency,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reload_the_winning_purchase_when_duplicate_invoice_delivery_races_on_insert(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IEntitlementService entitlementService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IProductVersionRepository productVersionRepository,
        EntitlementPurchaseService sut,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            Price = 100,
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 5,
            EntitlementValidityDays = 30,
        };
        var purchase = new EntitlementPurchaseEntity
        {
            Id = "purchase",
            AutoRenew = true,
            PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
            EntitlementId = "initial-entitlement",
            CustomerId = "customer",
            OrganizationId = "organization",
            ProductVersionId = "product-version",
            ProductPricing = pricing,
            Currency = "NZD",
            StripeCustomerId = "cus_1",
            StripeSubscriptionId = "sub_1",
        };
        var invoiceId = "in_1";
        var reference = $"stripe-subscription-invoice:acct_1:{invoiceId}";
        var winningPurchase = new EntitlementPurchaseEntity
        {
            Id = reference,
            EntitlementId = "renewed-entitlement",
            ProductPricing = pricing,
        };

        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => purchaseRepository.GetByIdAsync("purchase", cancellationToken)).Returns(purchase);
        A.CallTo(() => purchaseRepository.GetByIdAsync(reference, cancellationToken))
            .ReturnsNextFromSequence(null, null, winningPurchase);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>._))
            .ReturnsLazily((EntitlementPurchaseEntity item) => item);
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.That.Matches(item => item.Amount == pricing.Price)))
            .ReturnsLazily((EntitlementPurchaseEntity item) => item);
        A.CallTo(() => entitlementService.GrantAsync(
                reference,
                purchase.CustomerId,
                purchase.OrganizationId,
                pricing,
                A<DateTimeOffset>._,
                purchase.Currency,
                cancellationToken))
            .Returns(new EntitlementModel
            {
                Id = winningPurchase.EntitlementId,
            });
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken))
            .Throws(new DbUpdateException("The invoice purchase was inserted by another delivery."));

        await sut.ConfirmStripeSubscriptionInvoiceAsync(
            purchase.Id,
            "acct_1",
            invoiceId,
            DateTimeOffset.UtcNow,
            "pi_1",
            cancellationToken);

        A.CallTo(() => repositoryFactory.ResetChangeTracker()).MustHaveHappenedOnceExactly();
        A.CallTo(() => purchaseRepository.Add(A<EntitlementPurchaseEntity>.That.Matches(item => item.Amount == pricing.Price)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => purchaseRepository.GetByIdAsync(reference, cancellationToken)).MustHaveHappened(3, Times.Exactly);
        A.CallTo(() => historyRepository.AppendEventAsync(A<MarketplacePurchaseHistoryEventModel>._,
                $"stripe-invoice-paid:acct_1:{invoiceId}", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
