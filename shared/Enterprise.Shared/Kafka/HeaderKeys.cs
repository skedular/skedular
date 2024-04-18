namespace Enterprise.Shared.Kafka;

public static class HeaderKeys
{
    public const string RetryAttempt = "retry-attempt";
    public const string ConsumerGroupMatch = "retry-consumer-group";
    public const string LastException = "last-exception";
    public const string ClientId = "client-id";
}
