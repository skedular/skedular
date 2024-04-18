namespace Enterprise.Shared.Kafka;

public class KafkaExitCodes
{
    // Kafka = 2000
    public const int FailedToSubscribe = 2000;
    public const int UncaughtException = 2010;
    public const int FailedToCloseConsumer = 2020;
    public const int InternalException = 2030;
}
