# MsTeams Jobs Agent Notes

This file applies to `msteams/jobs/`.

## Purpose

- `MsTeams.Jobs` is the background jobs host for the Microsoft Teams integration domain.
- It drains the transactional outbox tables (Kafka and Temporal) written during MsTeams domain transactions, ensuring
  reliable event and workflow delivery.

## Key Responsibilities

- Kafka outbox drain: reliably publishes Kafka events written during MsTeams transactions.
- Temporal outbox drain: reliably starts or signals Temporal workflows from outbox records.

## Relationship To Other MsTeams Hosts

- Shares `MsTeams.Shared` as its domain library.
- Does not handle incoming Kafka events (that is `MsTeams.Processors`).
- Does not serve HTTP/GraphQL (that is `MsTeams.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- External Microsoft Teams API contracts can break production integrations quickly — keep host startup changes conservative.
- Do not add incoming event subscriber logic here; that belongs in `msteams/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `msteams/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the MsTeams domain app host.
