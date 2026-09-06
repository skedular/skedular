namespace Api.Shared.Events.Nats;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class NatsSubjectAttribute(int retryCount) : Attribute
{
    public int RetryCount { get; } = retryCount;
}
