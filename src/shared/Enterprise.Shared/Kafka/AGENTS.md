# Kafka Module — Agent Notes

## Purpose

Provides the full Kafka producer/consumer stack built on Confluent.Kafka and Confluent Schema Registry with Protobuf
serialization, OpenTelemetry trace propagation, retry/dead-letter topic routing, and the transactional outbox publisher.

## Registration

```csharp
// From a named Aspire connection string
var kafkaConfig = services.AddKafka(configuration, connectionName: "kafka");

// Or from a raw connection string
var kafkaConfig = services.AddKafkaWithConnectionString(configuration, connectionString);
```

Both return `KafkaConfiguration` which is needed for consumer registration calls.

**Config section key:** `Kafka` — see `Kafka/Configurations/KafkaConfiguration.cs`.

**Prerequisites:** `AddCoreServices<TProgram>()` (registers `ApplicationConfiguration`).

## Consuming Events

```csharp
// Single-topic consumer (uses topic derived from TEvent attribute)
services.AddKafkaEventConsumers<MySubscriber, KeyType, EventType>(kafkaConfig);

// Explicit topic list
services.AddKafkaEventConsumers<MySubscriber, KeyType, EventType>(kafkaConfig, ["my.topic"]);

// With retry topics (configures retry-1, retry-2, ... dead-letter topics)
services.AddKafkaReliableEventConsumers<MySubscriber, KeyType, EventType>(kafkaConfig);
```

Every event type must carry `[KafkaTopic(...)]` (see `Events/KafkaTopicAttribute.cs`). Every subscriber implements
`IEventSubscriber<TKey, TEvent>`.

## Producing Events

Use the transactional outbox publisher — do **not** publish directly inside a database transaction:

```csharp
// Inject IKafkaOutboxEventPublisher<TKey, TEvent> and call inside the same unit-of-work
publisher.Publish(key, @event, unitOfWork);
```

For fire-and-forget (outside a transaction):

```csharp
// Inject IKafkaPublisher<TKey, TEvent>
await publisher.PublishAsync(key, @event, cancellationToken);
```

## Serialization

All messages use Protobuf via Confluent Schema Registry. Keys and values must implement `IMessage`
(Google.Protobuf). Serializers/deserializers are registered as open generics:
`ISerializer<T>` / `IDeserializer<T>`.

## Retry and Dead-Letter Topics

Topic naming: `{incomingTopicPrefix}.{topicName}` for the main topic,
`{incomingTopicPrefix}.{topicName}.retry-{n}` for retry topics, and
`{incomingTopicPrefix}.{topicName}.dead-letter` for the dead-letter topic.

`KafkaConfiguration.RetryTopicCount` controls how many retry levels exist.

## Telemetry

- Activity source: `kafka_consumer` / `kafka_producer` (see `Kafka/Telemetry/KafkaActivityTracer.cs`).
- Trace context propagated via Kafka message headers using `Headers` propagator functions.
- Can be disabled per-registration by passing `useTelemetry: false`.

## Key Types

| Interface/Type                             | Role                                          |
|--------------------------------------------|-----------------------------------------------|
| `IKafkaPublisher<TKey, TEvent>`            | Direct (non-outbox) publisher                 |
| `IKafkaOutboxEventPublisher<TKey, TEvent>` | Outbox publisher (transactional)              |
| `IEventSubscriber<TKey, TEvent>`           | Consumer handler interface                    |
| `IConsumerFactory`                         | Creates raw `IConsumer<TKey, TValue>`         |
| `IProducerFactory`                         | Creates raw `IProducer<TKey, TValue>`         |
| `KafkaConfiguration`                       | Bound from `appsettings.json` under `"Kafka"` |

## Health Check

```csharp
services.AddKafkaBrokerHealthCheck(kafkaConfig);
```

Adds a readiness check that verifies broker connectivity. Timeout defaults to 5 seconds.

## Rules

- Do not hand-edit generated Protobuf classes — regenerate from `.proto` sources.
- Topic prefix logic is centralised in `KafkaConfiguration.OutgoingTopicPrefix` /
  `KafkaConfiguration.IncomingTopicPrefix`; do not build topic names manually in calling code.
- Use `IPushToTopic<T>` for higher-level push abstractions inside domain code.
