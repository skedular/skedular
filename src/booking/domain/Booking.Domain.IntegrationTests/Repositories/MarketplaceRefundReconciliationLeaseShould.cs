using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceRefundReconciliationLeaseShould(IRepositoryFactory repositoryFactory, IServiceScopeFactory scopeFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reserve_Refunds_Across_Split_Tenders_Without_Exceeding_Each_Source_Cap(
        string refundId, CancellationToken cancellationToken)
    {
        await EnsureOrganizationAsync(cancellationToken);
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund(refundId));
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund($"{refundId}-partial-stripe"));
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund($"{refundId}-partial-bank"));
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund($"{refundId}-partial-over-cap"));
        repositoryFactory.MarketplaceRefundRepository.AddAllocation(CreateSourceAllocation($"{refundId}-stripe", refundId, "stripe", 60m));
        repositoryFactory.MarketplaceRefundRepository.AddAllocation(CreateSourceAllocation($"{refundId}-bank", refundId, "bank", 40m));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var stripeRefund = await repositoryFactory.MarketplaceRefundRepository.ReserveAllocationAsync(
            $"{refundId}-partial-stripe", $"{refundId}-stripe", 60m, cancellationToken);
        var bankRefund = await repositoryFactory.MarketplaceRefundRepository.ReserveAllocationAsync(
            $"{refundId}-partial-bank", $"{refundId}-bank", 40m, cancellationToken);

        stripeRefund.AllocatedRefundAmount.ShouldBe(60m);
        bankRefund.AllocatedRefundAmount.ShouldBe(40m);
        await Should.ThrowAsync<InvalidOperationException>(() =>
            repositoryFactory.MarketplaceRefundRepository.ReserveAllocationAsync(
                $"{refundId}-partial-over-cap", $"{refundId}-stripe", 0.01m, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_Only_One_Concurrent_Reservation_To_Consume_The_Remaining_Source_Cap(
        string refundId, CancellationToken cancellationToken)
    {
        await EnsureOrganizationAsync(cancellationToken);
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund(refundId));
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund($"{refundId}-partial-a"));
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund($"{refundId}-partial-b"));
        repositoryFactory.MarketplaceRefundRepository.AddAllocation(CreateSourceAllocation($"{refundId}-stripe", refundId, "stripe", 100m));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await using var workerAScope = scopeFactory.CreateAsyncScope();
        await using var workerBScope = scopeFactory.CreateAsyncScope();
        var workerARepository = workerAScope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
        var workerBRepository = workerBScope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
        var reservations = await Task.WhenAll(
            TryReserveAsync(workerARepository, $"{refundId}-partial-a", $"{refundId}-stripe", 60m, cancellationToken),
            TryReserveAsync(workerBRepository, $"{refundId}-partial-b", $"{refundId}-stripe", 60m, cancellationToken));

        reservations.Count(item => item).ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_Only_One_Worker_To_Claim_And_Release_Completed_Claim(
        string refundId, CancellationToken cancellationToken)
    {
        await EnsureOrganizationAsync(cancellationToken);
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund(refundId));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var now = TimeProvider.System.GetUtcNow();
        await using var workerAScope = scopeFactory.CreateAsyncScope();
        await using var workerBScope = scopeFactory.CreateAsyncScope();
        var workerARepository = workerAScope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
        var workerBRepository = workerBScope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
        var claims = await Task.WhenAll(
            workerARepository.MarketplaceRefundRepository.TryClaimReconciliationAsync(refundId, "worker-a", now, TimeSpan.FromMinutes(5),
                cancellationToken),
            workerBRepository.MarketplaceRefundRepository.TryClaimReconciliationAsync(refundId, "worker-b", now, TimeSpan.FromMinutes(5),
                cancellationToken));

        claims.Count(item => item).ShouldBe(1);
        var winner = claims[0] ? "worker-a" : "worker-b";
        var loser = claims[0] ? "worker-b" : "worker-a";
        var winnerRepository = claims[0] ? workerARepository : workerBRepository;
        var loserRepository = claims[0] ? workerBRepository : workerARepository;
        await winnerRepository.MarketplaceRefundRepository.ReleaseReconciliationLeaseAsync(refundId, winner, cancellationToken);
        (await loserRepository.MarketplaceRefundRepository.RenewReconciliationLeaseAsync(refundId, loser, now, TimeSpan.FromMinutes(5),
            cancellationToken)).ShouldBeFalse();
        (await workerBRepository.MarketplaceRefundRepository.TryClaimReconciliationAsync(refundId, "worker-b", now, TimeSpan.FromMinutes(5),
            cancellationToken)).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Renew_An_Owned_Lease_And_Reclaim_An_Expired_Lease(
        string refundId, CancellationToken cancellationToken)
    {
        await EnsureOrganizationAsync(cancellationToken);
        repositoryFactory.MarketplaceRefundRepository.Add(CreateRefund(refundId));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var now = TimeProvider.System.GetUtcNow();
        (await repositoryFactory.MarketplaceRefundRepository.TryClaimReconciliationAsync(refundId, "worker-a", now, TimeSpan.FromMinutes(5),
            cancellationToken)).ShouldBeTrue();
        (await repositoryFactory.MarketplaceRefundRepository.RenewReconciliationLeaseAsync(refundId, "worker-a", now.AddMinutes(1),
            TimeSpan.FromMinutes(5), cancellationToken)).ShouldBeTrue();
        (await repositoryFactory.MarketplaceRefundRepository.TryClaimReconciliationAsync(refundId, "worker-b", now.AddMinutes(2),
            TimeSpan.FromMinutes(5), cancellationToken)).ShouldBeFalse();
        (await repositoryFactory.MarketplaceRefundRepository.TryClaimReconciliationAsync(refundId, "worker-b", now.AddMinutes(7),
            TimeSpan.FromMinutes(5), cancellationToken)).ShouldBeTrue();
    }

    private static MarketplaceRefund CreateRefund(string id) => new()
    {
        Id = id,
        OrganizationId = "org-1",
        LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
        LocalEntityId = $"booking-{id}",
        Status = MarketplaceRefundStatusConstants.ProviderPending,
        IdempotencyKey = $"reconciliation:{id}",
        RequestedAt = TimeProvider.System.GetUtcNow(),
        ReferenceTime = TimeProvider.System.GetUtcNow(),
        RefundKind = MarketplaceRefundKindConstants.Cancellation,
    };

    private static MarketplaceRefundPaymentAllocation CreateSourceAllocation(
        string id, string refundId, string provider, decimal amount) => new()
    {
        Id = id,
        MarketplaceRefundId = refundId,
        SourcePaymentProvider = provider,
        SourcePaymentReference = $"payment-{id}",
        SourceCapturedAmount = amount,
        Currency = "NZD",
        IsSourcePayment = true,
    };

    private static async Task<bool> TryReserveAsync(
        IRepositoryFactory repositoryFactory, string refundId, string allocationId, decimal amount,
        CancellationToken cancellationToken)
    {
        try
        {
            await repositoryFactory.MarketplaceRefundRepository.ReserveAllocationAsync(
                refundId, allocationId, amount, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    private async Task EnsureOrganizationAsync(CancellationToken cancellationToken)
    {
        if (await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                "org-1", null, false, false, cancellationToken) is null)
        {
            await repositoryFactory.OrganizationRepository.UpsertNakedAsync("org-1", cancellationToken);
        }
    }
}
