using System.Diagnostics.Metrics;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundOperationsService
{
    Task<int> LogOverdueBankTransferRefundsAsync(DateTimeOffset now, CancellationToken cancellationToken);
    Task RecordDashboardMetricsAsync(DateTimeOffset now, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<(MarketplaceExternalRefundReconciliationModel Node, string Cursor)>, int)> GetExternalRefundsAsync(
        string organizationId,
        string? provider,
        string? status,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<(MarketplaceExternalRefundReconciliationModel Node, string Cursor)>, int)> GetUnassignedExternalRefundsAsync(
        string? provider,
        string? status,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken);

    Task<MarketplaceExternalRefundReconciliationModel> ResolveUnassignedExternalRefundAsync(
        string provider,
        string externalRefundId,
        string status,
        string reason,
        string actorCustomerId,
        string? correlationId,
        CancellationToken cancellationToken);

    Task<MarketplaceExternalRefundReconciliationModel> ResolveExternalRefundAsync(string provider, string externalRefundId, string status,
        string reason,
        string organizationId,
        string actorCustomerId,
        string? correlationId,
        CancellationToken cancellationToken);
}

public sealed class MarketplaceRefundOperationsService(
    IRepositoryFactory repositoryFactory,
    ILogger<MarketplaceRefundOperationsService> logger) : IMarketplaceRefundOperationsService
{
    private const int DefaultExternalRefundPageSize = 50;
    private const int MaxExternalRefundPageSize = 100;
    private static readonly Meter Meter = new("Skedular.Booking.Refunds", "1.0");
    private static readonly Counter<long> OverdueBankTransfers = Meter.CreateCounter<long>("refund.bank_transfer.overdue");
    private static IReadOnlyList<Measurement<long>> _providerPendingMeasurements = [];
    private static IReadOnlyList<Measurement<long>> _processingMeasurements = [];
    private static IReadOnlyList<Measurement<long>> _failedMeasurements = [];
    private static IReadOnlyList<Measurement<long>> _reconciliationRequiredMeasurements = [];
    private static IReadOnlyList<Measurement<long>> _overdueBankTransferMeasurements = [];
    private static IReadOnlyList<Measurement<long>> _cancelledWithoutDecisionMeasurements = [];

    static MarketplaceRefundOperationsService()
    {
        Meter.CreateObservableGauge("refund.queue.provider_pending", () => Volatile.Read(ref _providerPendingMeasurements));
        Meter.CreateObservableGauge("refund.queue.processing", () => Volatile.Read(ref _processingMeasurements));
        Meter.CreateObservableGauge("refund.queue.failed", () => Volatile.Read(ref _failedMeasurements));
        Meter.CreateObservableGauge("refund.queue.reconciliation_required", () => Volatile.Read(ref _reconciliationRequiredMeasurements));
        Meter.CreateObservableGauge("refund.queue.overdue_bank_transfer", () => Volatile.Read(ref _overdueBankTransferMeasurements));
        Meter.CreateObservableGauge("refund.queue.cancelled_without_decision", () => Volatile.Read(ref _cancelledWithoutDecisionMeasurements));
    }

    public async Task RecordDashboardMetricsAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var snapshot = await repositoryFactory.MarketplaceRefundRepository.GetOperationsSnapshotAsync(
            now.AddBusinessDays(-3), cancellationToken);
        Volatile.Write(ref _providerPendingMeasurements, CreateMeasurements(snapshot.Refunds, MarketplaceRefundStatusConstants.ProviderPending));
        Volatile.Write(ref _processingMeasurements, CreateMeasurements(snapshot.Refunds, MarketplaceRefundStatusConstants.Processing));
        Volatile.Write(ref _failedMeasurements, CreateMeasurements(snapshot.Refunds, MarketplaceRefundStatusConstants.Failed));
        Volatile.Write(ref _reconciliationRequiredMeasurements,
            CreateMeasurements(snapshot.Refunds, MarketplaceRefundStatusConstants.ReconciliationRequired));
        Volatile.Write(ref _overdueBankTransferMeasurements, CreateMeasurements(snapshot.OverdueBankTransfers));
        Volatile.Write(ref _cancelledWithoutDecisionMeasurements, CreateMeasurements(snapshot.CancelledWithoutDecision));
    }

    public async Task<int> LogOverdueBankTransferRefundsAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var threshold = now.AddBusinessDays(-3);
        var overdue = (await repositoryFactory.MarketplaceRefundRepository.GetApprovedBankTransferRefundsBeforeAsync(threshold, cancellationToken))
            .ToArray();
        foreach (var refund in overdue)
        {
            OverdueBankTransfers.Add(
                1,
                new KeyValuePair<string, object?>("provider", refund.PaymentProvider ?? "unknown"),
                new KeyValuePair<string, object?>("status", refund.Status),
                new KeyValuePair<string, object?>("organization.id", refund.OrganizationId));
            logger.LogWarning(
                "Bank-transfer refund is overdue. RefundId={RefundId}, OrganizationId={OrganizationId}, Amount={Amount}, RequestedAt={RequestedAt}, Threshold={Threshold}",
                refund.Id,
                refund.OrganizationId,
                refund.RefundAmount,
                refund.RequestedAt,
                threshold);
        }

        return overdue.Length;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<(MarketplaceExternalRefundReconciliationModel Node, string Cursor)>, int)>
        GetExternalRefundsAsync(
            string organizationId,
            string? provider,
            string? status,
            PaginationInputParam paginationInputParam,
            CancellationToken cancellationToken)
    {
        var normalizedPagination = NormalizeExternalRefundPagination(paginationInputParam);
        var result = await repositoryFactory.MarketplaceRefundRepository.GetExternalReconciliationsAsync(
            organizationId, provider, status, normalizedPagination, cancellationToken);
        return (result.Item1, result.Item2.Select(edge => (ToModel(edge.Node), edge.Cursor)).ToList(), result.Item3);
    }

    public async Task<(PaginatedInfo, IReadOnlyList<(MarketplaceExternalRefundReconciliationModel Node, string Cursor)>, int)>
        GetUnassignedExternalRefundsAsync(
            string? provider,
            string? status,
            PaginationInputParam paginationInputParam,
            CancellationToken cancellationToken)
    {
        var result = await repositoryFactory.MarketplaceRefundRepository.GetUnassignedExternalReconciliationsAsync(
            provider, status, NormalizeExternalRefundPagination(paginationInputParam), cancellationToken);
        return (result.Item1, result.Item2.Select(edge => (ToModel(edge.Node), edge.Cursor)).ToList(), result.Item3);
    }

    public async Task<MarketplaceExternalRefundReconciliationModel> ResolveUnassignedExternalRefundAsync(
        string provider,
        string externalRefundId,
        string status,
        string reason,
        string actorCustomerId,
        string? correlationId,
        CancellationToken cancellationToken)
    {
        var record = await repositoryFactory.MarketplaceRefundRepository.GetExternalReconciliationAsync(
                         provider, externalRefundId, null, cancellationToken)
                     ?? throw new InvalidOperationException("The unassigned external refund reconciliation record was not found.");
        if (record.OrganizationId is not null)
        {
            throw new InvalidOperationException("The external refund reconciliation record is organization-owned.");
        }

        if (status is not ("Resolved" or "Rejected"))
        {
            throw new ArgumentException("External refund status must be Resolved or Rejected.", nameof(status));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        record.Status = status;
        record.ResolutionReason = reason;
        record.ResolutionActorCustomerId = actorCustomerId;
        record.ResolutionCorrelationId = correlationId;
        repositoryFactory.MarketplaceRefundRepository.UpdateExternalReconciliation(record);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return ToModel(record);
    }

    public async Task<MarketplaceExternalRefundReconciliationModel> ResolveExternalRefundAsync(
        string provider,
        string externalRefundId,
        string status,
        string reason,
        string organizationId,
        string actorCustomerId,
        string? correlationId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (status is not ("Resolved" or "Rejected"))
        {
            throw new ArgumentException("External refund status must be Resolved or Rejected.", nameof(status));
        }

        var record = await repositoryFactory.MarketplaceRefundRepository.GetExternalReconciliationAsync(
                         provider, externalRefundId, organizationId, cancellationToken)
                     ?? throw new InvalidOperationException("The external refund reconciliation record was not found.");
        record.Status = status;
        record.ResolutionReason = reason;
        record.ResolutionActorCustomerId = actorCustomerId;
        record.ResolutionCorrelationId = correlationId;
        repositoryFactory.MarketplaceRefundRepository.UpdateExternalReconciliation(record);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "External refund reconciliation resolved. Provider={Provider}, ExternalRefundId={ExternalRefundId}, Status={Status}",
            record.Provider, record.ExternalRefundId, record.Status);
        return ToModel(record);
    }

    private static MarketplaceExternalRefundReconciliationModel ToModel(MarketplaceExternalRefundReconciliation src) => new()
    {
        Id = src.Id,
        OrganizationId = src.OrganizationId,
        StripeAccountId = src.StripeAccountId,
        Provider = src.Provider.ToMarketplaceExternalRefundReconciliationProvider(),
        ExternalRefundId = src.ExternalRefundId,
        Amount = src.Amount,
        Currency = src.Currency.ToNullableCurrency(),
        Status = src.Status.ToMarketplaceExternalRefundReconciliationStatus(),
        FirstSeenAt = src.FirstSeenAt,
        LastSeenAt = src.LastSeenAt,
        RetryCount = src.RetryCount,
        NextRetryAt = src.NextRetryAt,
        ResolutionReason = src.ResolutionReason,
        ResolutionActorCustomerId = src.ResolutionActorCustomerId,
        ResolutionCorrelationId = src.ResolutionCorrelationId
    };

    private static Measurement<long>[] CreateMeasurements(
        IEnumerable<MarketplaceRefundOperationsMetric> metrics,
        string? status = null) =>
        metrics
            .Where(item => status is null || item.Status == status)
            .Select(item => new Measurement<long>(
                item.Count,
                new KeyValuePair<string, object?>("provider", item.Provider),
                new KeyValuePair<string, object?>("status", item.Status),
                new KeyValuePair<string, object?>("organization.id", item.OrganizationId ?? "unknown")))
            .ToArray();

    private static PaginationInputParam NormalizeExternalRefundPagination(PaginationInputParam paginationInputParam)
    {
        if (paginationInputParam.First is { } first)
        {
            if (first <= MaxExternalRefundPageSize)
            {
                return paginationInputParam;
            }

            return new PaginationInputParam(
                paginationInputParam.After,
                Math.Min(first, MaxExternalRefundPageSize),
                null,
                null);
        }

        if (paginationInputParam.Last is { } last)
        {
            if (last <= MaxExternalRefundPageSize)
            {
                return paginationInputParam;
            }

            return new PaginationInputParam(
                null,
                null,
                paginationInputParam.Before,
                Math.Min(last, MaxExternalRefundPageSize));
        }

        return string.IsNullOrWhiteSpace(paginationInputParam.Before)
            ? new PaginationInputParam(paginationInputParam.After, DefaultExternalRefundPageSize, null, null)
            : new PaginationInputParam(null, null, paginationInputParam.Before, DefaultExternalRefundPageSize);
    }
}

internal static class DateTimeOffsetBusinessDayExtensions
{
    public static DateTimeOffset AddBusinessDays(this DateTimeOffset value, int days)
    {
        var direction = Math.Sign(days);
        var remaining = Math.Abs(days);
        var result = value;
        while (remaining > 0)
        {
            result = result.AddDays(direction);
            if (result.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
            {
                remaining--;
            }
        }

        return result;
    }
}
