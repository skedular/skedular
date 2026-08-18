using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.UnitTests.Services.EntitlementReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class GetForCustomerAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_only_entitlements_in_organizations_the_operator_can_manage(
        [Frozen]
        IEntitlementService entitlementService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        EntitlementReadService sut,
        string customerId,
        string operatorCustomerId,
        string authorizedOrganizationId,
        string unauthorizedOrganizationId,
        CancellationToken cancellationToken)
    {
        var authorizedEntitlement = new EntitlementModel
        {
            Id = "entitlement-allowed",
            CustomerId = customerId,
            OrganizationId = authorizedOrganizationId,
        };
        var unauthorizedEntitlement = new EntitlementModel
        {
            Id = "entitlement-denied",
            CustomerId = customerId,
            OrganizationId = unauthorizedOrganizationId,
        };
        A.CallTo(() => entitlementService.GetForCustomerAsync(customerId, cancellationToken))
            .Returns([authorizedEntitlement, unauthorizedEntitlement]);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(
                authorizedOrganizationId, operatorCustomerId, cancellationToken))
            .Returns(true);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(
                unauthorizedOrganizationId, operatorCustomerId, cancellationToken))
            .Returns(false);

        var result = await sut.GetForCustomerAsync(customerId, operatorCustomerId, cancellationToken);

        result.ShouldBe([authorizedEntitlement]);
    }
}
