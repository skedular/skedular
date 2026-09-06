namespace Enterprise.Shared.Messaging.Nats.Consume;

public abstract record NatsMessageHandlerResult
{
    internal NatsMessageHandlerResult()
    {
    }
}

public sealed record SuccessNatsMessageHandlerResult : NatsMessageHandlerResult;

public static class NatsMessageHandlerResults
{
    public static readonly SuccessNatsMessageHandlerResult Success = new();
}
