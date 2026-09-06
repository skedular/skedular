using NATS.Client.JetStream.Models;

namespace Enterprise.Shared.Messaging.Nats.Configuration;

public sealed record NatsStreamDefinition(string Name, IReadOnlyCollection<string> Subjects)
{
    public StreamConfig ToClientConfig() => new(Name, Subjects.ToArray());
}

public sealed record NatsConsumerDefinition(
    string StreamName,
    string DurableName,
    string FilterSubject,
    TimeSpan AckWait,
    long MaxDeliver)
{
    public ConsumerConfig ToClientConfig() => new(DurableName)
    {
        FilterSubject = FilterSubject,
        AckWait = AckWait,
        MaxDeliver = MaxDeliver,
        MaxAckPending = 1,
    };
}
