using Api.Shared.Services.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementPurchaseServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CompleteAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectReservationPricingBeforePersistence(
        [Frozen]
        IRepositoryFactory purchaseRepository,
        EntitlementPurchaseService sut,
        string customerId,
        string organizationId,
        string productVersionId,
        CancellationToken cancellationToken)
    {
        var reservationPricing = ProductPricing.Empty("pricing-1");

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreatePendingAsync(
            customerId, organizationId, productVersionId, reservationPricing, "NZD", PaymentMethod.Card,
            TimeProvider.System.GetUtcNow().AddMinutes(30), null, [], cancellationToken));
        A.CallTo(() => purchaseRepository.EntitlementPurchaseRepository).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectPaymentMethodNotAcceptedByEntitlementPricing(
        [Frozen]
        IRepositoryFactory purchaseRepository,
        EntitlementPurchaseService sut,
        string customerId,
        string organizationId,
        string productVersionId,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 1,
            EntitlementValidityDays = 30,
            AcceptedPaymentMethods = [PaymentMethod.BankTransfer],
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreatePendingAsync(
            customerId, organizationId, productVersionId, pricing, "NZD", PaymentMethod.Card,
            TimeProvider.System.GetUtcNow().AddMinutes(30), null, [], cancellationToken));

        A.CallTo(() => purchaseRepository.EntitlementPurchaseRepository).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GrantConfirmedEntitlementPurchase(
        [Frozen]
        IEntitlementService entitlementService,
        EntitlementPurchaseService sut,
        string purchaseReference,
        string customerId,
        string organizationId,
        string currency,
        CancellationToken cancellationToken)
    {
        var expected = new EntitlementModel
        {
            Id = "entitlement-1",
        };
        A.CallTo(() => entitlementService.GrantAsync(purchaseReference, customerId, organizationId, A<ProductPricing>._, A<DateTimeOffset>._,
            currency,
            cancellationToken)).Returns(expected);
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 1,
            EntitlementValidityDays = 30,
        };

        var result = await sut.CompleteAsync(purchaseReference, customerId, organizationId, pricing, PaymentStatus.Confirmed,
            TimeProvider.System.GetUtcNow(),
            currency, cancellationToken);

        Assert.Same(expected, result);
        A.CallTo(() => entitlementService.GrantAsync(purchaseReference, customerId, organizationId, pricing, A<DateTimeOffset>._, currency,
            cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task IgnorePendingPaymentAndReservationPricing(
        [Frozen]
        IEntitlementService entitlementService,
        EntitlementPurchaseService sut,
        string purchaseReference,
        string customerId,
        string organizationId,
        string currency,
        CancellationToken cancellationToken)
    {
        var entitlementPricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
        };
        var reservationPricing = entitlementPricing with
        {
            FulfillmentType = ProductPricingFulfillmentType.Reservation,
        };

        Assert.Null(await sut.CompleteAsync(purchaseReference, customerId, organizationId, entitlementPricing, PaymentStatus.Pending,
            TimeProvider.System.GetUtcNow(), currency, cancellationToken));
        Assert.Null(await sut.CompleteAsync($"{purchaseReference}-reservation", customerId, organizationId, reservationPricing,
            PaymentStatus.Confirmed,
            TimeProvider.System.GetUtcNow(), currency, cancellationToken));
        A.CallTo(() => entitlementService.GrantAsync(A<string>._, A<string>._, A<string>._, A<ProductPricing>._, A<DateTimeOffset>._, A<string>._,
            cancellationToken)).MustNotHaveHappened();
    }
}
