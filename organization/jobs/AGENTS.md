# Organization Jobs Agent Notes

This file applies to `organization/jobs/`.

## Purpose

- `Organization.Jobs` is the background jobs host for the organization domain.
- It is a separate host from `Organization.Api` and `Organization.Processors`.
- Primary responsibility: draining the transactional outbox tables (Kafka and Temporal) that are written during
  organization domain transactions.

## Key Responsibilities

- There are no explicit job services in this host beyond outbox drain at this time.
- Outbox drain ensures that Kafka events and Temporal workflow signals written during org transactions are reliably
  delivered even if an in-process publish fails.

## Relationship To Other Organization Hosts

- `Organization.Jobs` shares `Organization.Shared` as its domain library.
- It does not handle incoming Kafka events (that is `Organization.Processors`).
- It does not serve HTTP/GraphQL (that is `Organization.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- Do not add incoming event subscriber logic here; that belongs in `organization/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `organization/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the org domain app host rather than in job startup.
