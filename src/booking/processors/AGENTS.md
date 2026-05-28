# Booking Processors Agent Notes

This file applies to `booking/processors/`.

## Purpose

- `Booking.Processors` is the Kafka event consumer host for the booking domain.
- It subscribes to Kafka event topics published by other domains and by booking itself (internal events), then reacts to
  those events to keep booking-domain state consistent or to trigger workflows.

## Kafka Subscriptions

| Subscriber class            | Kafka topic / event source                     | Responsibility                                                   |
|-----------------------------|------------------------------------------------|------------------------------------------------------------------|
| `BookingInternalSubscriber` | `BookingInternal.V1` (booking's own internal events) | Handles Stripe connect account webhook events and Xero webhook events forwarded from the API fast-ingress path |
| `CustomerSubscriber`        | `Customer.V1`                                  | Reacts to customer identity/state changes relevant to booking    |
| `LocationSubscriber`        | `Location.V1`                                  | Reacts to location/resource changes affecting booking state      |
| `MarketplaceSubscriber`     | `Marketplace.V1`                               | Reacts to marketplace listing/product changes affecting bookings |
| `OrganizationSubscriber`    | `Organization.V1`                              | Reacts to org billing/settings changes affecting booking         |
| `TeamSubscriber`            | `Team.V1`                                      | Reacts to team membership changes relevant to booking            |

## Important Behavior Notes

- Stripe Connect webhook events received at the Booking API fast-ingress endpoint are published as
  `BookingInternal.V1` Kafka events, then processed here asynchronously.
- Xero webhook events follow the same fast-ingress-to-Kafka-to-processor pattern; `BookingInternalSubscriber`
  delegates them to `IXeroWebhookService`.
- Do not perform synchronous Stripe or Xero API calls inside the API request path; keep that in processors/shared.

## Relationship To Other Booking Hosts

- `Booking.Processors` does not drain outboxes (that is `Booking.Jobs`).
- `Booking.Processors` does not serve HTTP/GraphQL (that is `Booking.Api`).
- All shared billing, invoice, and workflow logic lives in `booking/shared/Booking.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Do not add outbox drain logic here; that belongs in `booking/jobs/`.
- If a subscriber needs to start a Temporal workflow, use the shared Temporal service from `booking/shared/`.
- Keep Aspire dependency readiness (`WaitFor`) in `Booking.Domain.AppHost/AppHost.cs`.
- If adding a new Kafka subscription, define the event contract in `api-definitions/events/` first.
