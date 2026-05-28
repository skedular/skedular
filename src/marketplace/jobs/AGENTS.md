# Marketplace Jobs Agent Notes

This file applies to `marketplace/jobs/`.

## Purpose

- `Marketplace.Jobs` is the background jobs host for the marketplace domain.
- It drains the transactional outbox tables (Kafka and Temporal) written during marketplace transactions, ensuring
  reliable event and workflow delivery.

## Key Responsibilities

- Kafka outbox drain: reliably publishes Kafka events written during marketplace transactions.
- Temporal outbox drain: reliably starts or signals Temporal workflows from outbox records.

## Relationship To Other Marketplace Hosts

- Shares `Marketplace.Shared` as its domain library.
- Does not handle incoming Kafka events (that is `Marketplace.Processors`).
- Does not serve HTTP/GraphQL (that is `Marketplace.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- Do not add incoming event subscriber logic here; that belongs in `marketplace/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `marketplace/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the marketplace domain app host rather than in job startup.
