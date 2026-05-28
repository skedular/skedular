# Location Processors Agent Notes

This file applies to `location/processors/`.

## Purpose

- `Location.Processors` is the Kafka event consumer host for the location domain.
- It subscribes to Kafka topics published by other domains to keep location-local state consistent and to trigger
  analytics recompute workflows.

## Kafka Subscriptions

| Subscriber class          | Kafka topic / event source | Responsibility                                                                 |
|---------------------------|----------------------------|--------------------------------------------------------------------------------|
| `BookingSubscriber`       | `Booking.V1`               | Triggers location analytics recompute when booking state changes               |
| `CustomerSubscriber`      | `Customer.V1`              | Keeps replicated customer/identity state in the location domain current        |
| `MarketplaceSubscriber`   | `Marketplace.V1`           | Keeps replicated marketplace/product state current (for availability context)  |
| `OrganizationSubscriber`  | `Organization.V1`          | Keeps replicated org state current for location authorization                  |

## Important Behavior Notes

- Booking events trigger the location analytics recompute workflow (short-lived signal-with-start pattern in
  `Location.Shared`).
- Do not store replicated booking rows; only update compact local analytics snapshot tables.
- Auth-critical replicas (org, org members, customer, customer-identity) must remain current.

## Relationship To Other Location Hosts

- Does not drain outboxes (that is `Location.Jobs`).
- Does not serve HTTP/GraphQL (that is `Location.Api`).
- All shared location logic lives in `location/shared/Location.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Do not reintroduce booking row replication; use precomputed analytics snapshot patterns instead.
- Do not add outbox drain logic here; that belongs in `location/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the location domain app host.
