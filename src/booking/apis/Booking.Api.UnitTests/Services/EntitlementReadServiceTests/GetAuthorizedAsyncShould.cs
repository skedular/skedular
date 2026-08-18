using Booking.Api.Services;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.UnitTests.Services.EntitlementReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class GetAuthorizedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_owned_entitlement(
        [Frozen]
        IEntitlementService entitlementService,
        EntitlementReadService sut,
        string entitlementId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var entitlement = new EntitlementModel
        {
            Id = entitlementId,
            CustomerId = customerId,
        };
        A.CallTo(() => entitlementService.GetByIdAsync(entitlementId, cancellationToken)).Returns(entitlement);

        var result = await sut.GetAuthorizedAsync(entitlementId, customerId, cancellationToken);

        result.ShouldBeSameAs(entitlement);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_unowned_entitlement(
        [Frozen]
        IEntitlementService entitlementService,
        EntitlementReadService sut,
        string entitlementId,
        string customerId,
        string ownerCustomerId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => entitlementService.GetByIdAsync(entitlementId, cancellationToken))
            .Returns(new EntitlementModel
            {
                Id = entitlementId,
                CustomerId = ownerCustomerId,
            });

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.GetAuthorizedAsync(entitlementId, customerId, cancellationToken));
    }
}
