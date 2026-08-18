using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.UnitTests.Services.EntitlementPurchaseReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class GetAuthorizedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_owned_purchase(
        [Frozen]
        IEntitlementPurchaseService entitlementPurchaseService,
        EntitlementPurchaseReadService sut,
        string purchaseId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchase
        {
            Id = purchaseId,
            CustomerId = customerId,
        };
        A.CallTo(() => entitlementPurchaseService.GetByIdAsync(purchaseId, cancellationToken)).Returns(purchase);

        var result = await sut.GetAuthorizedAsync(purchaseId, customerId, cancellationToken);

        result.ShouldBeSameAs(purchase);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_purchase_when_customer_can_modify_payment_method(
        [Frozen]
        IEntitlementPurchaseService purchaseService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        EntitlementPurchaseReadService sut,
        string purchaseId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchase
        {
            Id = purchaseId,
            OrganizationId = "organization-1",
        };
        A.CallTo(() => purchaseService.GetByIdAsync(purchaseId, cancellationToken)).Returns(purchase);
        A.CallTo(() => authorizationService.CanModifyPaymentMethodAsync(purchase.OrganizationId, customerId, cancellationToken))
            .Returns(true);

        var result = await sut.GetAuthorizedAsync(purchaseId, customerId, cancellationToken);

        result.ShouldBeSameAs(purchase);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_purchase_when_customer_cannot_modify_payment_method(
        [Frozen]
        IEntitlementPurchaseService purchaseService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        EntitlementPurchaseReadService sut,
        string purchaseId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var purchase = new EntitlementPurchase
        {
            Id = purchaseId,
            OrganizationId = "organization-1",
        };
        A.CallTo(() => purchaseService.GetByIdAsync(purchaseId, cancellationToken)).Returns(purchase);
        A.CallTo(() => authorizationService.CanModifyPaymentMethodAsync(purchase.OrganizationId, customerId, cancellationToken))
            .Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.GetAuthorizedAsync(purchaseId, customerId, cancellationToken));
    }
}
