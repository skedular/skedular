using System.Diagnostics.Metrics;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundOperationsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RecordDashboardMetricsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Read_The_Repository_Backend_Snapshot(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        MarketplaceRefundOperationsService sut,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetOperationsSnapshotAsync(A<DateTimeOffset>._, cancellationToken))
            .Returns(CreateSnapshot());

        await sut.RecordDashboardMetricsAsync(now, cancellationToken);

        A.CallTo(() => marketplaceRefundRepository.GetOperationsSnapshotAsync(
                A<DateTimeOffset>.That.Matches(value => value < now), cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_The_Repository_Snapshot_Values_To_The_Gauges(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        MarketplaceRefundOperationsService sut,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetOperationsSnapshotAsync(A<DateTimeOffset>._, cancellationToken))
            .Returns(CreateSnapshot());

        using var listener = new MeterListener();
        var measurements = new Dictionary<string, List<(long Value, KeyValuePair<string, object?>[] Tags)>>();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == "Skedular.Booking.Refunds")
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, _) =>
        {
            if (!measurements.TryGetValue(instrument.Name, out var values))
            {
                values = [];
                measurements[instrument.Name] = values;
            }

            values.Add((measurement, tags.ToArray()));
        });
        listener.Start();

        await sut.RecordDashboardMetricsAsync(now, cancellationToken);
        listener.RecordObservableInstruments();

        AssertMeasurement(measurements, "refund.queue.provider_pending", 2, "STRIPE", "ProviderPending", "org-1");
        AssertMeasurement(measurements, "refund.queue.processing", 3, "STRIPE", "Processing", "org-1");
        AssertMeasurement(measurements, "refund.queue.failed", 4, "XERO", "Failed", "org-2");
        AssertMeasurement(measurements, "refund.queue.reconciliation_required", 5, "STRIPE", "ReconciliationRequired", "org-3");
        AssertMeasurement(measurements, "refund.queue.overdue_bank_transfer", 6, "XERO", "Approved", "org-2");
        AssertMeasurement(measurements, "refund.queue.cancelled_without_decision", 7, "none", "NoRefundDecision", "org-4");
    }

    private static MarketplaceRefundOperationsSnapshot CreateSnapshot() => new(
        [
            new MarketplaceRefundOperationsMetric("STRIPE", "ProviderPending", "org-1", 2),
            new MarketplaceRefundOperationsMetric("STRIPE", "Processing", "org-1", 3),
            new MarketplaceRefundOperationsMetric("XERO", "Failed", "org-2", 4),
            new MarketplaceRefundOperationsMetric("STRIPE", "ReconciliationRequired", "org-3", 5)
        ],
        [new MarketplaceRefundOperationsMetric("XERO", "Approved", "org-2", 6)],
        [new MarketplaceRefundOperationsMetric("none", "NoRefundDecision", "org-4", 7)]);

    private static void AssertMeasurement(
        IReadOnlyDictionary<string, List<(long Value, KeyValuePair<string, object?>[] Tags)>> measurements,
        string instrumentName,
        long expectedValue,
        string expectedProvider,
        string expectedStatus,
        string expectedOrganizationId)
    {
        var measurement = measurements[instrumentName].ShouldHaveSingleItem();
        measurement.Value.ShouldBe(expectedValue);
        var tags = measurement.Tags.ToDictionary(item => item.Key, item => item.Value);
        tags["provider"].ShouldBe(expectedProvider);
        tags["status"].ShouldBe(expectedStatus);
        tags["organization.id"].ShouldBe(expectedOrganizationId);
    }
}
