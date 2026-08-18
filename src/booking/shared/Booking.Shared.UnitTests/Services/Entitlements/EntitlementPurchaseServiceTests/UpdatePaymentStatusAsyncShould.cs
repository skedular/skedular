using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using EntitlementPurchase = Booking.Shared.Models.Entitlements.EntitlementPurchase;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementPurchaseServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class UpdatePaymentStatusAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task NotGrantAfterTerminalPaymentState(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseModelMapper purchaseModelMapper,
        [Frozen]
        IEntitlementService entitlementService,
        EntitlementPurchaseService sut,
        string purchaseId,
        CancellationToken cancellationToken)
    {
        var purchase = new Database.Entities.EntitlementPurchase
        {
            Id = purchaseId,
            PaymentStatus = PaymentStatus.Expired.ToPaymentStatus(),
            ProductPricing = ProductPricing.Empty("pricing-1"),
        };
        var model = new EntitlementPurchase
        {
            Id = purchaseId,
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken))
            .Returns(purchase);
        A.CallTo(() => purchaseModelMapper.Map(purchase)).Returns(model);

        var result = await sut.UpdatePaymentStatusAsync(
            purchaseId,
            PaymentStatus.Confirmed,
            TimeProvider.System.GetUtcNow(),
            cancellationToken);

        Assert.Same(model, result);
        A.CallTo(() => entitlementService.GrantAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<ProductPricing>._,
                A<DateTimeOffset>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ExpirePendingPurchasesWithoutGranting(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementService entitlementService,
        [Frozen]
        IUnitOfWork unitOfWork,
        EntitlementPurchaseService sut,
        string purchaseId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var purchase = new Database.Entities.EntitlementPurchase
        {
            Id = purchaseId,
            PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
            PaymentExpiry = now.AddMinutes(-1),
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository.GetExpiredPendingAsync(A<DateTimeOffset>._, cancellationToken))
            .Returns([purchase]);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);

        var count = await sut.ExpirePendingAsync(cancellationToken);

        Assert.Equal(1, count);
        Assert.Equal(PaymentStatus.Expired.ToPaymentStatus(), purchase.PaymentStatus);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => entitlementService.GrantAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<ProductPricing>._,
                A<DateTimeOffset>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
