using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Random;

namespace Testing.Shared.IntegrationTests.Kafka;

public class KafkaTestEventPublisher<TEvent>(IRandomHelper randomHelper, IProducerFactory eventPusher, KafkaConfiguration kafkaConfiguration)
    where TEvent : class, IEvent, new()
{
    private readonly IProducer<string, TEvent> _eventPusher = eventPusher.Build<string, TEvent>(kafkaConfiguration);
    private readonly string _topic = new TEvent().GetTopicName(kafkaConfiguration.IncomingTopicPrefix);

    public async Task PublishAsync(
        TEvent @event,
        CancellationToken cancellationToken,
        string? key = null,
        Timestamp timestamp = new(),
        Headers? headers = null)
    {
        var message = new Message<string, TEvent> { Key = key ?? randomHelper.Generate(), Headers = headers, Timestamp = timestamp, Value = @event };

        await _eventPusher.ProduceAsync(_topic, message, cancellationToken);
    }
}
