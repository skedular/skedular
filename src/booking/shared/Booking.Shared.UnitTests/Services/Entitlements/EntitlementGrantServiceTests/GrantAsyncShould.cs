using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementGrantServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GrantAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task SnapshotValidityAndRefundPolicyAtPurchaseTime(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRandomHelper randomHelper,
        EntitlementService sut,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => entitlementRepository.GetByPurchaseReferenceAsync("purchase-1", cancellationToken)).Returns<Entitlement?>(null);
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("entitlement-1", "ledger-1");
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        Entitlement? added = null;
        A.CallTo(() => entitlementRepository.Add(A<Entitlement>._)).Invokes((Entitlement value) => added = value)
            .ReturnsLazily((Entitlement value) => value);
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 4,
            EntitlementValidityDays = 30,
            Price = 100,
        };
        var activatesAt = new DateTimeOffset(2026, 8, 10, 0, 0, 0, TimeSpan.Zero);

        await sut.GrantAsync("purchase-1", "customer-1", "organization-1", pricing, activatesAt, "NZD", cancellationToken);

        Assert.NotNull(added);
        Assert.Equal(activatesAt.AddDays(30), added!.ExpiresAt);
        Assert.Equal(4, added.GrantedQuantity);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ReturnExistingGrantForDuplicatePurchaseReference(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IEntitlementModelMapper entitlementModelMapper,
        EntitlementService sut,
        CancellationToken cancellationToken)
    {
        var existing = new Entitlement
        {
            Id = "existing",
        };
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => entitlementRepository.GetByPurchaseReferenceAsync("purchase-1", cancellationToken)).Returns(existing);
        A.CallTo(() => entitlementModelMapper.Map(existing)).Returns(new EntitlementModel
        {
            Id = existing.Id,
        });

        var result = await sut.GrantAsync("purchase-1", "customer-1", "organization-1", ProductPricing.Empty("pricing-1") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 1,
            EntitlementValidityDays = 1,
        }, TimeProvider.System.GetUtcNow(), "NZD", cancellationToken);

        Assert.Equal(existing.Id, result.Id);
    }
}
