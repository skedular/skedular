using Confluent.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Kafka.Produce;

/// <summary>
///     Kafka producer factory decorator to build <see cref="ProducerClientIdDecorator{TKey,TValue}" /> decorator.
/// </summary>
public class ProducerFactoryClientIdDecorator(IProducerFactory producerFactory, IServiceProvider serviceProvider) : IProducerFactory
{
    /// <summary>
    ///     Create <see cref="ProducerClientIdDecorator{TKey,TValue}" /> decorator.
    /// </summary>
    /// <param name="kafkaConfiguration">
    ///     <see cref="KafkaConfiguration" />
    /// </param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns>&lt;see cref="ProducerClientIdDecorator{TKey,TValue}" /&gt;</returns>
    public IProducer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration) =>
        ActivatorUtilities.CreateInstance<ProducerClientIdDecorator<TKey, TValue>>(
            serviceProvider,
            producerFactory.Build<TKey, TValue>(kafkaConfiguration));
}
