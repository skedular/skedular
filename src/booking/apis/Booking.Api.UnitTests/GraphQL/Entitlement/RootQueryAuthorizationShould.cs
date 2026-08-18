using Booking.Api.GraphQL.Entitlement;
using Booking.Api.Services;
using Booking.Shared.Services.Cache;

namespace Booking.Api.UnitTests.GraphQL.Entitlement;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RootQueryAuthorizationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectEntitlementDetailForAnotherCustomer(
        IEntitlementReadService entitlementReadService,
        ICachedCustomerService cachedCustomerService,
        RootQuery sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => entitlementReadService.GetAuthorizedAsync("entitlement-1", "other-1", cancellationToken))
            .Throws(new UnauthorizedAccessException());
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("other-1");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            sut.EntitlementAsync("entitlement-1", entitlementReadService, cachedCustomerService, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectOrganizationHistoryForUnauthorizedOperator(
        IEntitlementReadService entitlementReadService,
        ICachedCustomerService cachedCustomerService,
        RootQuery sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => entitlementReadService.GetForOrganizationAsync("organization-1", "operator-1", cancellationToken))
            .Throws(new UnauthorizedAccessException());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            sut.OrganizationEntitlementsAsync("organization-1", entitlementReadService, cachedCustomerService, cancellationToken));
    }
}
