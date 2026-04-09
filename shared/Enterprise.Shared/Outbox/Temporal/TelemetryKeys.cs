namespace Enterprise.Shared.Outbox.Temporal;

public static class TelemetryKeys
{
    public const string TemporalActivitySourceName = "temporal_outbox";
    public const string TemporalEventSave = "temporal outbox save";
    public const string TemporalEventSend = "temporal outbox send";

    public const string TemporalSignalActivitySourceName = "temporal_outbox";
    public const string TemporalSignalEventSave = "temporal outbox save";
    public const string TemporalSignalEventSend = "temporal outbox send";
}
