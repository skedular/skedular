using Confluent.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Kafka.Telemetry;

public class ConsumerFactoryTelemetryDecorator(
    IConsumerFactory consumerFactory,
    IServiceProvider serviceProvider)
    : IConsumerFactory
{
    public IConsumer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration)
    {
        var consumer = consumerFactory.Build<TKey, TValue>(kafkaConfiguration);

        // Telemetry 
        consumer =
            ActivatorUtilities.CreateInstance<ConsumerTelemetryDecorator<TKey, TValue>>(
                serviceProvider, consumer);

        return consumer;
    }
}
