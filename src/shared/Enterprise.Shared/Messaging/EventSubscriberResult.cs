namespace Enterprise.Shared.Messaging;

public abstract record EventSubscriberResult
{
    internal EventSubscriberResult() { }
}

public sealed record SuccessEventSubscriberResult : EventSubscriberResult;

public static class EventSubscriberResults
{
    public static readonly SuccessEventSubscriberResult Success = new();
}
