using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Logger;

public interface IKafkaLogger
{
    void SetLogHandler<TKey, TValue>(ConsumerBuilder<TKey, TValue> builder);
    void SetLogHandler<TKey, TValue>(ProducerBuilder<TKey, TValue> builder);
}

public class KafkaLogger(ILogger<KafkaLogger> logger) : IKafkaLogger
{
    public void SetLogHandler<TKey, TValue>(ConsumerBuilder<TKey, TValue> builder) => builder.SetLogHandler(
        (consumer, logMessage) =>
        {
            try
            {
                Log<TValue>(logMessage, "CONSUMER", consumer.Name);
            }
            catch
            {
                // ignored
            }
        });

    public void SetLogHandler<TKey, TValue>(ProducerBuilder<TKey, TValue> builder) => builder.SetLogHandler(
        (producer, logMessage) =>
        {
            try
            {
                Log<TValue>(logMessage, "PRODUCER", producer.Name);
            }
            catch
            {
                // ignored
            }
        });

    private void Log<TValue>(
        LogMessage logMessage,
        string serviceType,
        string instanceName)
    {
        var message = logMessage.Message;

        switch (logMessage.Level)
        {
            case SyslogLevel.Emergency:
            case SyslogLevel.Alert:
            case SyslogLevel.Critical:
                logger.LogCritical(
                    "[{ServiceType} {KafkaType} :: {KafkaName}]: {Message}", serviceType,
                    typeof(TValue).Name, instanceName, message);

                break;

            case SyslogLevel.Error:
                logger.LogError("[{ServiceType} {KafkaType} :: {KafkaName}]: {Message}",
                    serviceType,
                    typeof(TValue).Name, instanceName, message);

                break;

            case SyslogLevel.Notice:
            case SyslogLevel.Info:
                logger.LogInformation(
                    "[{ServiceType} {KafkaType} :: {KafkaName}]: {Message}", serviceType,
                    typeof(TValue).Name, instanceName, message);

                break;

            case SyslogLevel.Debug:
                logger.LogDebug("[{ServiceType} {KafkaType} :: {KafkaName}]: {Message}",
                    serviceType,
                    typeof(TValue).Name, instanceName, message);

                break;

            case SyslogLevel.Warning:
            default:
                logger.LogWarning("[{ServiceType} {KafkaType} :: {KafkaName}]: {Message}",
                    serviceType,
                    typeof(TValue).Name, instanceName, message);

                break;
        }
    }
}
