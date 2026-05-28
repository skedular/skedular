# Outbox Module — Agent Notes

## Purpose

Implements the transactional outbox pattern for two brokers:

- **Kafka** (`Outbox/Kafka/`) — atomically writes a Kafka message to a database row, then a background
  service drains those rows and publishes them to Kafka.
- **Temporal** (`Outbox/Temporal/`) — atomically writes a workflow-start or workflow-signal intent to a
  database row, then a background service drains those rows and calls the Temporal server.

This guarantees at-least-once delivery even if the process crashes between the database commit and the
broker call.

## Sub-modules

| Sub-module      | Namespace                           | Registration                                                                                        |
|-----------------|-------------------------------------|-----------------------------------------------------------------------------------------------------|
| Kafka outbox    | `Enterprise.Shared.Outbox.Kafka`    | `services.AddKafkaOutboxBackgroundService<TDbContext>()` + `services.AddKafkaOutboxService()`       |
| Temporal outbox | `Enterprise.Shared.Outbox.Temporal` | `services.AddTemporalOutboxBackgroundService<TDbContext>()` + `services.AddTemporalOutboxService()` |

`AddKafkaOutboxService()` is called automatically by `AddKafka(...)`.
`AddTemporalOutboxService()` is called automatically by `AddTemporalWorker(...)`.
Only call these manually when the parent module is not registered.

## Database Entities

All outbox rows live in the same database as the application data, committed in the same transaction.

| Entity                 | Table control                | Used by                                 |
|------------------------|------------------------------|-----------------------------------------|
| `KafkaOutbox`          | `IKafkaOutboxStore`          | `KafkaOutboxBackgroundService`          |
| `TemporalOutbox`       | `ITemporalOutboxStore`       | `TemporalOutboxBackgroundService`       |
| `TemporalSignalOutbox` | `ITemporalSignalOutboxStore` | `TemporalSignalOutboxBackgroundService` |

Each `DbContext` that uses the outbox must implement the corresponding store interface and expose the
`DbSet`. The `*Configuration` class (e.g. `KafkaOutboxConfiguration`) must be applied via
`modelBuilder.ApplyConfiguration(new KafkaOutboxConfiguration())`.

## Writing to the Outbox

- Kafka: inject `IKafkaOutboxEventPublisher<TKey, TEvent>` and call `.Publish(key, event, unitOfWork)`.
- Temporal workflow start: inject `ITemporalOutboxWorkflowExecutor` and call `.Execute<TWorkflow>(...)`.
- Temporal signal: inject `ITemporalSignalOutboxWorkflowExecutor` and call `.Signal(...)`.

All three write inside the caller's unit-of-work transaction — do not call them after `SaveChanges`.

## Background Services

Each background service uses a claim-then-release strategy:

1. A short transaction claims a batch with `SKIP LOCKED` and advances `LastRetry` as a lease.
2. The transaction is committed before any broker call.
3. Successfully processed rows are deleted; failed rows increment `RetryCount` and log `ProcessingErrors`.
4. Rows that reach `CriticalRetryThreshold` (5) are logged at `Critical` level.

Do not modify the polling logic or the retry/lease strategy without understanding this ordering — changing
it can cause double-delivery or silent message loss.

## Telemetry

- Kafka outbox: activity source `kafka_outbox`
- Temporal outbox: activity source `temporal_outbox`

Propagation context is stored in the outbox row's `Headers` (Kafka) or propagated via dictionary (Temporal)
so that the background service can continue the originating trace.

## Rules

- Do not add non-outbox concerns to this folder.
- Do not bypass the outbox by calling Kafka or Temporal directly inside a database transaction.
- Use the focused sub-module extensions in `Outbox/Kafka/Extensions.cs` and `Outbox/Temporal/Extensions.cs`.
