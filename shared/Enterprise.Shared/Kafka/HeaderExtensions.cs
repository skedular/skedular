using System.Text;
using Confluent.Kafka;

namespace Enterprise.Shared.Kafka;

public static class HeaderExtensions
{
    extension(Headers headers)
    {
        public string? Get(string name)
        {
            var header = headers.FirstOrDefault(header => header.Key == name);

            return header is null ? null : Encoding.UTF8.GetString(header.GetValueBytes());
        }

        public void Set(string name,
            string value)
        {
            headers.Remove(name);
            var bytes = Encoding.UTF8.GetBytes(value);
            headers.Add(name, bytes);
        }
    }

    extension<TKey, TValue>(Message<TKey, TValue> message)
    {
        public string? Get(string name) =>
            message.Headers?.Get(name);

        public void Set(string name,
            string value)
        {
            message.Headers ??= [];
            var headers = message.Headers;

            headers.Set(name, value);
        }

        public int? GetRetryAttempt()
        {
            var value = message.Get(HeaderKeys.RetryAttempt);

            return string.IsNullOrEmpty(value) ? null : int.Parse(value);
        }

        public void SetRetryAttempt(int retryAttempt) =>
            message.Set(HeaderKeys.RetryAttempt, retryAttempt.ToString());

        public string? GetConsumerGroup() =>
            message.Get(HeaderKeys.ConsumerGroupMatch);

        public void SetConsumerGroup(string consumerGroup) =>
            message.Set(HeaderKeys.ConsumerGroupMatch, consumerGroup);

        public string? GetLastException() =>
            message.Get(HeaderKeys.LastException);

        public void SetLastException(Exception exception) =>
            message.Set(HeaderKeys.LastException, exception.ToString());

        public DateTime GetTimestamp() =>
            message.Timestamp.UtcDateTime;

        public void SetTimestamp() =>
            message.Timestamp = new Timestamp(TimeProvider.System.GetUtcNow());
    }
}
