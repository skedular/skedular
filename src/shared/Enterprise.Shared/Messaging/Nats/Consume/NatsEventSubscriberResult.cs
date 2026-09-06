namespace Enterprise.Shared.Messaging.Nats.Consume;

public abstract record NatsEventSubscriberResult
{
    internal NatsEventSubscriberResult()
    {
    }
}

public sealed record SuccessNatsEventSubscriberResult : NatsEventSubscriberResult;

public static class NatsEventSubscriberResults
{
    public static readonly SuccessNatsEventSubscriberResult SuccessNats = new();
}
