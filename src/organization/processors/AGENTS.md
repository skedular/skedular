# Organization Processors Agent Notes

This file applies to `organization/processors/`.

## Purpose

- `Organization.Processors` is the Kafka event consumer host for the organization domain.
- It subscribes to Kafka topics from other domains and from organization internal events, then keeps
  organization-local state consistent and triggers Temporal workflows as needed.

## Kafka Subscriptions

| Subscriber class                 | Kafka topic / event source        | Responsibility                                                               |
|----------------------------------|-----------------------------------|------------------------------------------------------------------------------|
| `OrganizationInternalSubscriber` | `OrganizationInternal.V1`         | Reacts to organization's own internal events (e.g. Xero webhook forwarding) |
| `BookingSubscriber`              | `Booking.V1`                      | Invalidates/recomputes booking-derived org analytics from booking events     |
| `CustomerSubscriber`             | `Customer.V1`                     | Keeps replicated customer/identity state in the org domain current           |

## Important Behavior Notes

- Booking events trigger local organization analytics recompute workflows (short-lived signal-with-start pattern).
- Do not store replicated booking rows in the org domain; only update compact precomputed snapshot tables.
- Customer replication in this processor supports local authorization and membership-aware access decisions.
- Xero connection-related events may be routed through `OrganizationInternalSubscriber` and handled by org shared services.

## Relationship To Other Organization Hosts

- `Organization.Processors` does not drain outboxes (that is `Organization.Jobs`).
- `Organization.Processors` does not serve HTTP/GraphQL (that is `Organization.Api`).
- All shared org logic lives in `organization/shared/Organization.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Do not reintroduce booking row replication; use precomputed analytics snapshot patterns instead.
- Do not add outbox drain logic here; that belongs in `organization/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the org domain app host.
- Auth-critical replicas (org members, customers) must be kept current; do not remove those replication handlers.
