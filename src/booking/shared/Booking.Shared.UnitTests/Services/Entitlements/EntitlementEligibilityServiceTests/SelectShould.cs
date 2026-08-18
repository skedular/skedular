using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementEligibilityServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SelectShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task QueryOnlyTheRequestingCustomerAtTheRequestedBookingTime(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        EntitlementEligibilityService sut,
        CancellationToken cancellationToken)
    {
        var bookingAt = new DateTimeOffset(2030, 6, 12, 9, 30, 0, TimeSpan.Zero);
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => entitlementRepository.GetActiveForCustomerAsync("customer-42", bookingAt, cancellationToken)).Returns([]);

        await sut.SelectAsync("customer-42", "pricing-42", bookingAt, cancellationToken);

        A.CallTo(() => entitlementRepository.GetActiveForCustomerAsync("customer-42", bookingAt, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => entitlementRepository.GetActiveForCustomerAsync("other-customer", A<DateTimeOffset>._, cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task SelectTheEarliestExpiringEntitlementWithBalance(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        [Frozen]
        IEntitlementModelMapper entitlementModelMapper,
        EntitlementEligibilityService sut,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        CancellationToken cancellationToken)
    {
        var first = new Entitlement
        {
            Id = "first",
            PricingId = "pricing-1",
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(1),
        };
        var second = new Entitlement
        {
            Id = "second",
            PricingId = "pricing-1",
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(2),
        };
        A.CallTo(() => entitlementRepository.GetActiveForCustomerAsync("customer-1", A<DateTimeOffset>._, cancellationToken))
            .Returns([first, second]);
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => creditLedgerService.GetAvailableCredits(first)).Returns(0);
        A.CallTo(() => creditLedgerService.GetAvailableCredits(second)).Returns(1);
        A.CallTo(() => entitlementModelMapper.Map(second)).Returns(new EntitlementModel
        {
            Id = second.Id,
        });

        var result = await sut.SelectAsync("customer-1", "pricing-1", TimeProvider.System.GetUtcNow(), cancellationToken);

        Assert.Equal(second.Id, result?.Id);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ReturnNullWhenEveryActiveEntitlementHasZeroBalance(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        EntitlementEligibilityService sut,
        IEntitlementRepository entitlementRepository,
        CancellationToken cancellationToken)
    {
        var entitlement = new Entitlement
        {
            Id = "empty",
            PricingId = "pricing-1",
        };
        A.CallTo(() => entitlementRepository.GetActiveForCustomerAsync("customer-1", A<DateTimeOffset>._, cancellationToken)).Returns([entitlement]);
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => creditLedgerService.GetAvailableCredits(entitlement)).Returns(0);

        var result = await sut.SelectAsync("customer-1", "pricing-1", TimeProvider.System.GetUtcNow(), cancellationToken);

        Assert.Null(result);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ReturnNullWhenNoActiveEntitlementsExist(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        EntitlementEligibilityService sut,
        IEntitlementRepository entitlementRepository,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => entitlementRepository.GetActiveForCustomerAsync("customer-1", A<DateTimeOffset>._, cancellationToken)).Returns([]);

        var result = await sut.SelectAsync("customer-1", "pricing-1", TimeProvider.System.GetUtcNow(), cancellationToken);

        Assert.Null(result);
    }
}
