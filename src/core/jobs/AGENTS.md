# Core Jobs Agent Notes

This file applies to `core/jobs/`.

## Purpose

- `Core.Jobs` is the background jobs host for the core domain.
- It drains the transactional outbox tables (Kafka and Temporal) written during core domain transactions, ensuring
  reliable event and workflow delivery.

## Key Responsibilities

- Kafka outbox drain: reliably publishes Kafka events written during core transactions.
- Temporal outbox drain: reliably starts or signals Temporal workflows from outbox records.
- File storage integration is registered here because core domain activities can produce file outputs.

## Relationship To Other Core Hosts

- Shares `Core.Shared` as its domain library.
- Does not handle incoming Kafka events (that is `Core.Processors`).
- Does not serve HTTP/GraphQL (that is `Core.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- Do not add incoming event subscriber logic here; that belongs in `core/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `core/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the core domain app host.
