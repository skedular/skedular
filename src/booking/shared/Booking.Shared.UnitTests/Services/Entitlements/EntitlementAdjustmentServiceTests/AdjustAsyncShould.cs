using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementAdjustmentServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AdjustAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task ReturnTheExistingEntryForARepeatedIdempotencyKey(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementModelMapper entitlementModelMapper,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        EntitlementAdjustmentService sut,
        CancellationToken cancellationToken)
    {
        var existing = new CreditLedgerEntry
        {
            Id = "entry-existing",
            ReferenceKey = "adjustment-1",
            Quantity = 2,
            TransactionType = CreditLedgerTransactionType.Adjusted.ToPersistedValue(),
        };
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => entitlementRepository.GetByIdAsync("entitlement-1", cancellationToken)).Returns(new Entitlement
        {
            Id = "entitlement-1",
            GrantedQuantity = 1,
            LedgerEntries = [existing],
        });
        A.CallTo(() => entitlementModelMapper.Map(existing)).Returns(new CreditLedgerEntryModel
        {
            Id = existing.Id,
        });

        var result = await sut.AdjustAsync("entitlement-1", 2, "operator-1", "Correction", "adjustment-1", cancellationToken);

        Assert.Equal(existing.Id, result.Id);
        A.CallTo(() => entitlementRepository.AddLedgerEntry(A<CreditLedgerEntry>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task PersistPositiveAdjustmentWithActorAndReason(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRandomHelper randomHelper,
        EntitlementAdjustmentService sut,
        IUnitOfWork unitOfWork,
        IEntitlementRepository entitlementRepository,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("entry-1", "reference-1");
        A.CallTo(() => entitlementRepository.GetByIdAsync("entitlement-1", cancellationToken)).Returns(new Entitlement
        {
            Id = "entitlement-1",
            GrantedQuantity = 1,
        });
        CreditLedgerEntry? added = null;
        A.CallTo(() => entitlementRepository.AddLedgerEntry(A<CreditLedgerEntry>._)).Invokes((CreditLedgerEntry entry) => added = entry)
            .ReturnsLazily((CreditLedgerEntry entry) => entry);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.AdjustAsync("entitlement-1", 2, "operator-1", "Customer service correction", cancellationToken);

        Assert.NotNull(added);
        Assert.Equal(CreditLedgerTransactionType.Adjusted.ToPersistedValue(), added!.TransactionType);
        Assert.Equal("operator-1", added.Metadata!.ActorCustomerId);
        Assert.Equal("Customer service correction", added.Metadata.Reason);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectNegativeAdjustmentThatWouldCreateNegativeBalance(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        EntitlementAdjustmentService sut,
        IEntitlementRepository entitlementRepository,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => entitlementRepository.GetByIdAsync("entitlement-1", cancellationToken)).Returns(new Entitlement
        {
            Id = "entitlement-1",
            GrantedQuantity = 1,
        });

        await Assert.ThrowsAsync<EntitlementCreditUnavailable>(() =>
            sut.AdjustAsync("entitlement-1", -2, "operator-1", "Correction", cancellationToken));
    }
}
