using System.Text.RegularExpressions;

namespace Api.Shared.Events.Kafka;

public static class IKafkaEventExtensions
{
    private static readonly Regex s_validKafkaTopicCharacters = new(
        @"^[a-z0-9\.\-_]+$",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);

    private static string ValidateTopicName(string name, string parameterName) =>
        s_validKafkaTopicCharacters.IsMatch(name)
            ? name
            : throw new ArgumentException($"Invalid characters in topic name `{name}`", parameterName);

    extension(IKafkaEvent kafkaEvent)
    {
        public string GetTopicName(string environmentName)
        {
            var topic = string.IsNullOrWhiteSpace(environmentName)
                ? kafkaEvent.TopicName
                : $"{environmentName}.{kafkaEvent.TopicName}";
            return ValidateTopicName(topic, nameof(environmentName));
        }

        public int GetRetryTopicCount() => kafkaEvent.RetryTopicCount;

        public string GetRetryTopicName(string environmentName, int idx)
        {
            var topic = string.IsNullOrWhiteSpace(environmentName)
                ? $"{kafkaEvent.RetryTopicNamePrefix}.{idx}"
                : $"{environmentName}.{kafkaEvent.RetryTopicNamePrefix}.{idx}";
            return ValidateTopicName(topic, nameof(environmentName));
        }

        public string GetDeadLetterTopicName(string environmentName)
        {
            var topic = string.IsNullOrWhiteSpace(environmentName)
                ? kafkaEvent.DeadLetterTopicName
                : $"{environmentName}.{kafkaEvent.DeadLetterTopicName}";
            return ValidateTopicName(topic, nameof(environmentName));
        }

        public string? GetCorrelationId() => kafkaEvent.CorrelationId;
    }
}
