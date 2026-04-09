using System.Text.RegularExpressions;

namespace Enterprise.Shared.Events;

/// <summary>
///     Where IEvent is inherited, the type that is inheriting needs the attribute <see cref="KafkaTopicAttribute" />
/// </summary>
public partial interface IEvent
{
    private static readonly Regex s_validKafkaTopicCharacters = ValidTopicNameRegex();
    string TopicName { get; }
    string RetryTopicNamePrefix { get; }
    int RetryTopicCount { get; }
    string DeadLetterTopicName { get; }
    string? CorrelationId => null;

    string GetTopicName(string environmentName) =>
        ValidateTopicName(string.IsNullOrWhiteSpace(environmentName) ? TopicName : $"{environmentName}.{TopicName}", nameof(environmentName));

    int GetRetryTopicCount() => RetryTopicCount;

    string GetRetryTopicName(string environmentName, int idx) =>
        ValidateTopicName(
            string.IsNullOrWhiteSpace(environmentName)
                ? $"{RetryTopicNamePrefix}.{idx}"
                : $"{environmentName}.{RetryTopicNamePrefix}.{idx}",
            nameof(environmentName));

    string GetDeadLetterTopicName(string environmentName) =>
        ValidateTopicName(
            string.IsNullOrWhiteSpace(environmentName) ? DeadLetterTopicName : $"{environmentName}.{DeadLetterTopicName}",
            nameof(environmentName));

    string? GetCorrelationId() => CorrelationId;

    private static string ValidateTopicName(string name, string parameterName) =>
        s_validKafkaTopicCharacters.IsMatch(name)
            ? name
            : throw new ArgumentException($"Invalid characters in topic name `{name}`", parameterName);

    [GeneratedRegex(@"^[a-z0-9\.\-_]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline, "en-NZ")]
    private static partial Regex ValidTopicNameRegex();
}
