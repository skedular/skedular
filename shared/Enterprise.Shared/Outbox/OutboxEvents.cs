namespace Enterprise.Shared.Outbox;

public static class OutboxEvents
{
    public static event EventHandler? ItemAdded;

    public static void OnTransactionCommit() => ItemAdded?.Invoke(null, EventArgs.Empty);
}
