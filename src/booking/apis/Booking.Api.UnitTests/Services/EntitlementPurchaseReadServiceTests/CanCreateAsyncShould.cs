using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.UnitTests.Services.EntitlementPurchaseReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class CanCreateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task AllowAuthenticatedCustomerWithoutOrganizationOperatorPermission(
        [Frozen]
        IEntitlementPurchaseService purchaseService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        EntitlementPurchaseReadService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await sut.CanCreateAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeTrue();
        A.CallTo(() => authorizationService.CanModifyPaymentMethodAsync(A<string>._, A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => purchaseService.GetByIdAsync(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
    }
}
