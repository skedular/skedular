namespace Enterprise.Shared.Events;

/// <summary>
///     Where IEvent is inherited, the type that is inheriting needs the attribute <see cref="KafkaTopicAttribute" />
/// </summary>
public interface IEvent
{
    string GetTopicName(string environmentName);
    int GetRetryTopicCount();
    string GetRetryTopicName(string environmentName, int idx);
    string GetDeadLetterTopicName(string environmentName);
    string? GetCorrelationId();
}

public interface IMetadataEvent : IEvent;

public interface IMetadata;
