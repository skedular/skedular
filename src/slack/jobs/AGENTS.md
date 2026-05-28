# Slack Jobs Agent Notes

This file applies to `slack/jobs/`.

## Purpose

- `Slack.Jobs` is the background jobs host for the Slack integration domain.
- It drains the transactional outbox tables (Kafka and Temporal) written during Slack domain transactions, ensuring
  reliable event and workflow delivery.

## Key Responsibilities

- Kafka outbox drain: reliably publishes Kafka events written during Slack transactions.
- Temporal outbox drain: reliably starts or signals Temporal workflows from outbox records.

## Relationship To Other Slack Hosts

- Shares `Slack.Shared` as its domain library.
- Does not handle incoming Kafka events (that is `Slack.Processors`).
- Does not serve HTTP/GraphQL (that is `Slack.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- Do not add incoming event subscriber logic here; that belongs in `slack/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `slack/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the Slack domain app host.
