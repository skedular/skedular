# Team Jobs Agent Notes

This file applies to `team/jobs/`.

## Purpose

- `Team.Jobs` is the background jobs host for the team domain.
- It drains the transactional outbox tables (Kafka and Temporal) written during team domain transactions, ensuring
  reliable event and workflow delivery.

## Key Responsibilities

- Kafka outbox drain: reliably publishes Kafka events written during team transactions.
- Temporal outbox drain: reliably starts or signals Temporal workflows (e.g. invitation workflows) from outbox records.

## Relationship To Other Team Hosts

- Shares `Team.Shared` as its domain library.
- Does not handle incoming Kafka events (that is `Team.Processors`).
- Does not serve HTTP/GraphQL (that is `Team.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- Do not add incoming event subscriber logic here; that belongs in `team/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `team/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the team domain app host.
