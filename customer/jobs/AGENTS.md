# Customer Jobs Agent Notes

This file applies to `customer/jobs/`.

## Purpose

- `Customer.Jobs` is the background jobs host for the customer domain.
- It drains the transactional outbox tables (Kafka and Temporal) written during customer domain transactions, ensuring
  reliable event and workflow delivery.

## Key Responsibilities

- Kafka outbox drain: reliably publishes Kafka events written during customer transactions.
- Temporal outbox drain: reliably starts or signals Temporal workflows from outbox records.

## Relationship To Other Customer Hosts

- Shares `Customer.Shared` as its domain library.
- Does not handle incoming Kafka events (that is `Customer.Processors`).
- Does not serve HTTP/GraphQL (that is `Customer.Api`).

## Agent Rule

- Keep this host focused on outbox drain.
- Customer identity events published from here are consumed by many other domains; event contract changes require
  updating `api-definitions/events/skedular/customer_v1*.proto` and regenerating.
- Do not add incoming event subscriber logic here; that belongs in `customer/processors/`.
- Do not add HTTP/GraphQL surfaces here; that belongs in `customer/apis/`.
- Keep Aspire dependency readiness (`WaitFor`) in the customer domain app host.
