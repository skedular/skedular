using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.UnitTests.Services.EntitlementPurchasePaymentServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class ConfirmManualBankTransferAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task ConfirmOnlyAuthorizedBankTransferPurchases(
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        [Frozen]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        EntitlementPurchasePaymentService sut,
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset confirmedAt,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchase
        {
            Id = purchaseId,
            CustomerId = actorCustomerId,
            OrganizationId = "organization-1",
            PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
        };
        A.CallTo(() => entitlementPurchaseReadService.GetAuthorizedAsync(purchaseId, actorCustomerId, cancellationToken))
            .Returns(purchase);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync(
                purchase.OrganizationId,
                actorCustomerId,
                cancellationToken))
            .Returns(true);
        A.CallTo(() => entitlementPurchaseService.UpdatePaymentStatusAsync(
                purchaseId,
                PaymentStatus.Confirmed,
                confirmedAt,
                cancellationToken))
            .Returns(purchase);

        var result = await sut.ConfirmManualBankTransferAsync(purchaseId, actorCustomerId, confirmedAt, cancellationToken);

        Assert.Same(purchase, result);
        A.CallTo(() => entitlementPurchaseService.UpdatePaymentStatusAsync(
                purchaseId,
                PaymentStatus.Confirmed,
                confirmedAt,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectCustomerConfirmationWithoutPaymentManagementPermission(
        [Frozen]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementPurchasePaymentService sut,
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset confirmedAt,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchase
        {
            Id = purchaseId,
            CustomerId = actorCustomerId,
            OrganizationId = "organization-1",
            PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
        };
        A.CallTo(() => entitlementPurchaseReadService.GetAuthorizedAsync(purchaseId, actorCustomerId, cancellationToken))
            .Returns(purchase);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync(
                purchase.OrganizationId,
                actorCustomerId,
                cancellationToken))
            .Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.ConfirmManualBankTransferAsync(
            purchaseId,
            actorCustomerId,
            confirmedAt,
            cancellationToken));

        A.CallTo(() => entitlementPurchaseService.UpdatePaymentStatusAsync(
                A<string>._,
                A<PaymentStatus>._,
                A<DateTimeOffset>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectCardPurchases(
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        [Frozen]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        EntitlementPurchasePaymentService sut,
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset confirmedAt,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchase
        {
            Id = purchaseId,
            PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
        };
        A.CallTo(() => entitlementPurchaseReadService.GetAuthorizedAsync(purchaseId, actorCustomerId, cancellationToken))
            .Returns(purchase);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ConfirmManualBankTransferAsync(
            purchaseId,
            actorCustomerId,
            confirmedAt,
            cancellationToken));

        A.CallTo(() => entitlementPurchaseService.UpdatePaymentStatusAsync(
                A<string>._,
                A<PaymentStatus>._,
                A<DateTimeOffset>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
