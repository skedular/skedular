using System.Diagnostics;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Kafka;
using KafkaTelemetryKeys = Enterprise.Shared.Outbox.Kafka.TelemetryKeys;

namespace Enterprise.Shared.UnitTests.Database.OutboxTelemetryFilterTests;

public class ShouldTraceActivityShould
{
    private const string NpgsqlActivitySourceName = "Npgsql";

    [Fact]
    public void TraceNonOutboxDatabaseActivity()
    {
        using var listener = CreateListener();
        using var source = new ActivitySource(NpgsqlActivitySourceName);
        using var activity = source.StartActivity("SELECT bookingdb")!;
        activity.SetTag("db.statement", "SELECT * FROM \"Bookings\" WHERE \"Id\" = @id");

        OutboxTelemetryFilter.ShouldTraceActivity(activity).ShouldBeTrue();
    }

    [Fact]
    public void TraceOutboxDatabaseActivityWithoutOutboxParent()
    {
        using var listener = CreateListener();
        using var source = new ActivitySource(NpgsqlActivitySourceName);
        using var activity = source.StartActivity("SELECT bookingdb")!;
        activity.SetTag("db.statement", $"SELECT * FROM \"{nameof(KafkaOutbox)}\"");

        OutboxTelemetryFilter.ShouldTraceActivity(activity).ShouldBeTrue();
    }

    [Fact]
    public void TraceSuccessfulNpgsqlConnectWithoutOutboxParent()
    {
        using var listener = CreateListener();
        using var source = new ActivitySource(NpgsqlActivitySourceName);
        using var activity = source.StartActivity("CONNECT bookingdb")!;

        OutboxTelemetryFilter.ShouldTraceActivity(activity).ShouldBeTrue();
    }

    [Fact]
    public void FilterSuccessfulNpgsqlConnectWithOutboxParent()
    {
        using var listener = CreateListener();
        using var outboxSource = new ActivitySource(KafkaTelemetryKeys.KafkaActivitySourceName);
        using var dbSource = new ActivitySource(NpgsqlActivitySourceName);
        using var parent = outboxSource.StartActivity(KafkaTelemetryKeys.KafkaEventPoll)!;
        using var activity = dbSource.StartActivity("CONNECT bookingdb")!;

        OutboxTelemetryFilter.ShouldTraceActivity(activity).ShouldBeFalse();
    }

    [Fact]
    public void TraceErrorNpgsqlConnectWithOutboxParent()
    {
        using var listener = CreateListener();
        using var outboxSource = new ActivitySource(KafkaTelemetryKeys.KafkaActivitySourceName);
        using var dbSource = new ActivitySource(NpgsqlActivitySourceName);
        using var parent = outboxSource.StartActivity(KafkaTelemetryKeys.KafkaEventPoll)!;
        using var activity = dbSource.StartActivity("CONNECT bookingdb")!;
        activity.SetStatus(ActivityStatusCode.Error);

        OutboxTelemetryFilter.ShouldTraceActivity(activity).ShouldBeTrue();
    }

    [Fact]
    public void FilterExplicitOutboxActivity()
    {
        using var listener = CreateListener();
        using var source = new ActivitySource(KafkaTelemetryKeys.KafkaActivitySourceName);
        using var activity = source.StartActivity(KafkaTelemetryKeys.KafkaEventPoll)!;

        OutboxTelemetryFilter.ShouldTraceActivity(activity).ShouldBeFalse();
    }

    private static ActivityListener CreateListener()
    {
        var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            SampleUsingParentId = (ref _) => ActivitySamplingResult.AllDataAndRecorded
        };

        ActivitySource.AddActivityListener(listener);
        return listener;
    }
}
