using System.Text.RegularExpressions;

namespace Api.Shared.Events;

public static class IEventExtensions
{
    private static readonly Regex s_validKafkaTopicCharacters = new(
        @"^[a-z0-9\.\-_]+$",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);

    public static string GetTopicName(this IEvent @event, string environmentName)
    {
        var topic = string.IsNullOrWhiteSpace(environmentName)
            ? @event.TopicName
            : $"{environmentName}.{@event.TopicName}";
        return ValidateTopicName(topic, nameof(environmentName));
    }

    public static int GetRetryTopicCount(this IEvent @event) => @event.RetryTopicCount;

    public static string GetRetryTopicName(this IEvent @event, string environmentName, int idx)
    {
        var topic = string.IsNullOrWhiteSpace(environmentName)
            ? $"{@event.RetryTopicNamePrefix}.{idx}"
            : $"{environmentName}.{@event.RetryTopicNamePrefix}.{idx}";
        return ValidateTopicName(topic, nameof(environmentName));
    }

    public static string GetDeadLetterTopicName(this IEvent @event, string environmentName)
    {
        var topic = string.IsNullOrWhiteSpace(environmentName)
            ? @event.DeadLetterTopicName
            : $"{environmentName}.{@event.DeadLetterTopicName}";
        return ValidateTopicName(topic, nameof(environmentName));
    }

    public static string? GetCorrelationId(this IEvent @event) => @event.CorrelationId;

    private static string ValidateTopicName(string name, string parameterName) =>
        s_validKafkaTopicCharacters.IsMatch(name)
            ? name
            : throw new ArgumentException($"Invalid characters in topic name `{name}`", parameterName);
}
