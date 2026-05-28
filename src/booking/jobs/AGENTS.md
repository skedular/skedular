# Booking Jobs Agent Notes

This file applies to `booking/jobs/`.

## Purpose

- `Booking.Jobs` is the background jobs host for the booking domain.
- It drains the transactional outbox tables (Kafka outbox and Temporal outbox) to ensure reliable message delivery.
- It also hosts the `GraphQlTopicEventSender` service, which forwards GraphQL subscription events from the booking
  domain to the Booking API via gRPC, enabling real-time GraphQL subscriptions over the federation gateway.

## Key Responsibilities

- **Kafka outbox drain**: `AddKafkaOutboxBackgroundService<BookingDbContext>()` — reliably publishes Kafka events that
  were written to the outbox table during booking domain transactions.
- **Temporal outbox drain**: `AddTemporalOutboxBackgroundService<BookingDbContext>()` — reliably starts or signals
  Temporal workflows from outbox records written during transactions.
- **GraphQL topic events**: `GraphQlTopicEventSender` — calls the Booking API gRPC `RaiseGraphqlChange` method to push
  subscription topic changes initiated by background processing.

## Relationship To Other Booking Hosts

- `Booking.Jobs` is a separate host from `Booking.Api` and `Booking.Processors`.
- It shares `Booking.Shared` as its domain library.
- It does not handle incoming Kafka events (that is `Booking.Processors`).
- It does not serve HTTP/GraphQL (that is `Booking.Api`).

## Agent Rule

- Keep `Booking.Jobs` focused on outbox drain and background lifecycle tasks.
- Do not add event subscriber logic here; that belongs in `booking/processors/`.
- Do not add HTTP/GraphQL controller surfaces here; that belongs in `booking/apis/`.
- If Temporal outbox or Kafka outbox patterns change, update `booking/shared/` and ensure `Booking.Jobs` stays aligned.
- Keep Aspire dependency readiness (`WaitFor`) in `Booking.Domain.AppHost/AppHost.cs`, not in job startup.
