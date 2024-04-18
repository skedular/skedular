using System.Text;
using Confluent.Kafka;

namespace Enterprise.Shared.Kafka;

public static class HeaderExtensions
{
    public static string? Get(this Headers headers, string name)
    {
        var header = headers.FirstOrDefault(header => header.Key == name);

        return header is null ? null : Encoding.UTF8.GetString(header.GetValueBytes());
    }

    public static void Set(
        this Headers headers,
        string name,
        string value)
    {
        headers.Remove(name);
        var bytes = Encoding.UTF8.GetBytes(value);
        headers.Add(name, bytes);
    }

    public static string? Get<TKey, TValue>(this Message<TKey, TValue> message, string name) =>
        message.Headers?.Get(name);

    public static void Set<TKey, TValue>(
        this Message<TKey, TValue> message,
        string name,
        string value)
    {
        message.Headers ??= [];
        var headers = message.Headers;

        headers.Set(name, value);
    }

    public static int? GetRetryAttempt<TKey, TValue>(this Message<TKey, TValue> message)
    {
        var value = message.Get(HeaderKeys.RetryAttempt);

        return string.IsNullOrEmpty(value) ? null : int.Parse(value);
    }

    public static void SetRetryAttempt<TKey, TValue>(this Message<TKey, TValue> message, int retryAttempt) =>
        message.Set(HeaderKeys.RetryAttempt, retryAttempt.ToString());

    public static string? GetConsumerGroup<TKey, TValue>(this Message<TKey, TValue> message) =>
        message.Get(HeaderKeys.ConsumerGroupMatch);

    public static void SetConsumerGroup<TKey, TValue>(this Message<TKey, TValue> message, string consumerGroup) =>
        message.Set(HeaderKeys.ConsumerGroupMatch, consumerGroup);

    public static string? GetLastException<TKey, TValue>(this Message<TKey, TValue> message) =>
        message.Get(HeaderKeys.LastException);

    public static void SetLastException<TKey, TValue>(this Message<TKey, TValue> message, Exception exception) =>
        message.Set(HeaderKeys.LastException, exception.ToString());

    public static DateTime GetTimestamp<TKey, TValue>(this Message<TKey, TValue> message) =>
        message.Timestamp.UtcDateTime;

    public static void SetTimestamp<TKey, TValue>(this Message<TKey, TValue> message) =>
        message.Timestamp = new Timestamp(DateTimeOffset.UtcNow);
}
