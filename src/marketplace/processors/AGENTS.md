# Marketplace Processors Agent Notes

This file applies to `marketplace/processors/`.

## Purpose

- `Marketplace.Processors` is the Kafka event consumer host for the marketplace domain.
- It subscribes to Kafka topics published by other domains to keep marketplace-local state consistent and to react to
  cross-domain events that affect listings, products, or purchase state.

## Kafka Subscriptions

| Subscriber class         | Kafka topic / event source | Responsibility                                                                 |
|--------------------------|----------------------------|--------------------------------------------------------------------------------|
| `BookingSubscriber`      | `Booking.V1`               | Keeps marketplace aware of booking state changes that affect subscription/listing status |
| `CustomerSubscriber`     | `Customer.V1`              | Keeps replicated customer/identity state current for marketplace authorization |
| `OrganizationSubscriber` | `Organization.V1`          | Keeps replicated org state current for marketplace ownership and authorization |

## Important Behavior Notes

- Marketplace changes often affect booking, pricing, and checkout assumptions across domains.
- Auth-critical replicas (org, customer) are kept current here and must not be removed without a broader authorization
  redesign.
- Product/listing projections may be candidates for workflow-driven rebuilds, but auth-critical replicas are not.

## Relationship To Other Marketplace Hosts

- Does not drain outboxes (that is `Marketplace.Jobs`).
- Does not serve HTTP/GraphQL (that is `Marketplace.Api`).
- All shared marketplace logic lives in `marketplace/shared/Marketplace.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Marketplace changes here can have pricing and booking ripple effects — check cross-domain impact.
- Do not add outbox drain logic here; that belongs in `marketplace/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the marketplace domain app host.
