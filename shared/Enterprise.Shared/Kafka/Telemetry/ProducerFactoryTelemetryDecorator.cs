using Confluent.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Kafka.Telemetry;

public class ProducerFactoryTelemetryDecorator(
    IProducerFactory producerFactory,
    IServiceProvider serviceProvider)
    : IProducerFactory
{
    public IProducer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration)
    {
        var producer = producerFactory.Build<TKey, TValue>(kafkaConfiguration);

        producer =
            ActivatorUtilities.CreateInstance<ProducerTelemetryDecorator<TKey, TValue>>(
                serviceProvider, producer);

        return producer;
    }
}
