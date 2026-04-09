# Customer Processors Agent Notes

This file applies to `customer/processors/`.

## Purpose

- `Customer.Processors` is the Kafka event consumer host for the customer domain.
- It subscribes to Kafka topics published by other domains to keep customer-domain state consistent.

## Kafka Subscriptions

| Subscriber class         | Kafka topic / event source | Responsibility                                                               |
|--------------------------|----------------------------|------------------------------------------------------------------------------|
| `LocationSubscriber`     | `Location.V1`              | Keeps replicated location/resource state current in the customer domain      |
| `OrganizationSubscriber` | `Organization.V1`          | Keeps replicated org/membership state current for customer authorization     |

## Important Behavior Notes

- Customer and customer-identity data are the source of truth for identity state replicated into many other domains.
- Changes to customer shape, identity semantics, or replication here propagate downstream to all consumers.
- Auth-critical replicas must be kept current; do not remove subscription handlers without a full cross-domain audit.

## Relationship To Other Customer Hosts

- Does not drain outboxes (that is `Customer.Jobs`).
- Does not serve HTTP/GraphQL (that is `Customer.Api`).
- All shared customer logic lives in `customer/shared/Customer.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Customer identity is a cross-domain dependency — treat changes carefully.
- Do not add outbox drain logic here; that belongs in `customer/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the customer domain app host.
