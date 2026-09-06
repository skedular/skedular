# Kafka Event Contracts - Agent Notes

This file applies to `src/shared/Api.Shared/Events/Kafka`.

## Purpose

This directory contains the shared Kafka event contracts and metadata infrastructure in the `Api.Shared` library, which
targets **netstandard2.0** for cross-platform compatibility (.NET Framework, .NET Core, .NET 5+).

## Portability Rule

- This library targets **netstandard2.0** explicitly; do not add framework-specific dependencies.
- No references to `System.Reflection.Emit`, platform-specific APIs, or modern .NET-only packages.
- Dependencies must be compatible with netstandard2.0: `Google.Protobuf` for timestamp support.
- This ensures `Api.Shared.Clients` and any domain needing to consume events can target netstandard2.0 for broader
  runtime compatibility.

## Event Contract Ownership

This library owns the read-only Kafka event abstraction:

- **`IKafkaEvent`** interface: Base contract for all Kafka events with topic name, retry/dead-letter topic metadata, and
  correlation ID.
- **`IKafkaEventExtensions`** static extension methods: `GetTopicName()`, `GetRetryTopicName()`,
  `GetDeadLetterTopicName()`,
  `GetRetryTopicCount()`, `GetCorrelationId()`.
- **`KafkaTopicAttribute`**: Attribute applied to event types specifying topic partition configuration.
- **`KafkaTopicAttributeHelper`**: Reflection-based helper to extract Kafka topic metadata from decorated event types.
- **`KafkaEventMetadataFactory`**: Factory for creating `IKafkaEventMetadata<TType>` instances with automatic
  ID/timestamp/correlation ID generation.

Do not add kafka-specific producer/consumer logic here; that belongs in `Enterprise.Shared.Kafka` or domain event
handlers.

## Default Interface Implementation Rule

- No default method implementations in `IKafkaEvent`; all behavior is in `IKafkaEventExtensions`.
- This ensures netstandard2.0 compatibility (C# 8 default interface implementations are unsupported).
- Extension methods provide the same API surface without netstandard2.0 limitations.

## Correlation ID Rule

- `IKafkaEvent.CorrelationId` is nullable (string?).
- Events may or may not carry correlation context; use `IKafkaEventExtensions.GetCorrelationId()` which returns null if
  not set.
- `KafkaEventMetadataFactory` generates a UUID for missing/null correlation IDs automatically.

## Topic Validation Rule

- Topic names must match Kafka naming conventions: alphanumeric, dots, hyphens, underscores only.
- `IKafkaEventExtensions` validate topic names using regex; invalid names throw `ArgumentException`.
- Topic prefixes (e.g., `bookings.`, `orgs.`) are environment-specific and injected at infrastructure setup time.

## Usage Pattern

Event implementations in domain event projects:

```csharp
namespace MyDomain.Events.MyEvent.V1;

using Api.Shared.Events.Kafka;

[KafkaTopicAttribute(TopicPartitionCount = 3, RetryTopicCount = 1, ...)]
public class MyEventMetadata : IKafkaEventMetadata<MyEvent>
{
    public string TopicName => "my-events";
    public string? CorrelationId { get; set; }
    // ... other properties
}

// Usage
var topic = MyEventMetadata.TopicName.GetTopicName(); // Applies environment prefix
var retryTopic = MyEventMetadata.TopicName.GetRetryTopicName(); // Prefixed retry topic
```

## No Domain-Specific Logic

- Do not add booking-specific, organization-specific, or domain-specific event logic here.
- All event behavior belongs in the event metadata companions under `Api.Shared.Clients/Events/Dsst/**`.
- This library is the portable skeleton; domain projects flesh it out.

## Testing

- Unit tests live in `Api.Shared.UnitTests`.
- Test event implementations must implement the full `IKafkaEvent` contract including nullable `CorrelationId`.
- Test coverage for extension methods, attribute extraction, and factory behavior.

## Dependency Chart

```
Api.Shared (netstandard2.0)
  ↑
  ├─ Api.Shared.Clients (netstandard2.0) — for event metadata
  ├─ Enterprise.Shared (net10.0) — for Kafka infrastructure
  └─ (All domains consuming Kafka events)
```

Projects consuming `Api.Shared` can target any compatible runtime (netstandard2.0, .NET Framework, .NET 5+).
