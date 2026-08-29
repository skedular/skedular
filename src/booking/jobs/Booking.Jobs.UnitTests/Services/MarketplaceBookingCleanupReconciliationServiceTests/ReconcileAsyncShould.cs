using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;

namespace Booking.Jobs.UnitTests.Services.MarketplaceBookingCleanupReconciliationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReconcileAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Enqueue_Claimed_Cleanup_Candidates(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceBookingCleanupReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(0);
        A.CallTo(() => failureRepository.ReleaseCleanupLeaseAsync(failure.Id, A<string>._, cancellationToken))
            .Returns(Task.CompletedTask);
        A.CallTo(() => failureRepository.GetCleanupCandidatesAsync(100, cancellationToken))
            .Returns([failure]);
        A.CallTo(() => failureRepository.TryClaimCleanupAsync(
                failure.Id,
                A<string>._,
                A<DateTimeOffset>._,
                A<TimeSpan>._,
                cancellationToken))
            .Returns(true);

        await sut.ReconcileAsync(cancellationToken);

        A.CallTo(() => temporalOutboxService.StartWorkflowMarketplaceBookingCleanup(
            A<MarketplaceBookingCleanupInput>.That.Matches(input => input.FailureId == failure.Id),
            repositoryFactory.UnitOfWork)).MustHaveHappenedOnceExactly();
        A.CallTo(() => failureRepository.ReleaseCleanupLeaseAsync(
            failure.Id, A<string>._, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Skip_Candidates_Claimed_By_Another_Worker(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceBookingCleanupReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => failureRepository.ReleaseCleanupLeaseAsync(failure.Id, A<string>._, cancellationToken))
            .Returns(Task.CompletedTask);
        A.CallTo(() => failureRepository.GetCleanupCandidatesAsync(100, cancellationToken))
            .Returns([failure]);
        A.CallTo(() => failureRepository.TryClaimCleanupAsync(
                failure.Id, A<string>._, A<DateTimeOffset>._, A<TimeSpan>._, cancellationToken))
            .Returns(false);

        await sut.ReconcileAsync(cancellationToken);

        A.CallTo(() => temporalOutboxService.StartWorkflowMarketplaceBookingCleanup(
                A<MarketplaceBookingCleanupInput>._, A<IUnitOfWork>._))
            .MustNotHaveHappened();
    }
}
