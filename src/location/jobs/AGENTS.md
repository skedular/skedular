# Location Jobs Agent Notes

This file applies to `location/jobs/`.

## Purpose

- `Location.Jobs` is the background jobs host for the location domain.
- It drains the transactional outbox tables (Kafka and Temporal) written during location domain transactions, ensuring
  reliable event and workflow delivery.

## Key Responsibilities

- Kafka outbox drain: reliably publishes Kafka events written during location transactions.
- Temporal outbox drain: reliably starts or signals Temporal workflows from outbox records.

## Relationship To Other Location Hosts

- Shares `Location.Shared` as its domain library.
- Does not handle incoming Kafka events (that is `Location.Processors`).
- Does not serve HTTP/GraphQL (that is `Location.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- Do not add incoming event subscriber logic here; that belongs in `location/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `location/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the location domain app host rather than in job startup.
