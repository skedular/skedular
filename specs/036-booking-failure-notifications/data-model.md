# Data Model: Booking Failure Communications

## MarketplaceBookingFailure

Booking-owned aggregate representing one final, customer-meaningful failure outcome.

| Field | Purpose / validation |
| --- | --- |
| `Id` | Stable aggregate identifier. |
| `FailureKey` | Unique idempotency key: submission/series/occurrence identity plus final category. |
| `Category` | `AvailabilityConflict`, `PaymentFailed`, or `PaymentExpired`; technical errors remain operator-only unless safely classified. |
| `Scope` | `OneTimeBooking`, `InitialSeries`, `RecurringOccurrence`, or `RecurringCycle`. |
| `BookingId` / `RecurringBookingId` / `SubscriptionId` | Optional owning links; at least one applicable scope link is required. |
| `RequestedFrom` / `RequestedUntil` | The affected booking window when applicable. |
| `RequestedResourceIds` | Snapshot of requested resources/places, not an allocation claim. |
| `FinalizedAt` | Set once when the outcome becomes final. |
| `CorrelationId` | Trace and replay correlation. |
| `CustomerAction` | `Rebook`, `ReviewSubscription`, or `None`; supports safe UI CTA. |

Unique index: `FailureKey`. Index ownership links and `FinalizedAt` for history queries.

## MarketplaceBookingFailureEvent

Append-only audit event following the refund-event pattern.

| Field | Purpose |
| --- | --- |
| `MarketplaceBookingFailureId` | Parent aggregate. |
| `EventType` | `Detected`, `Finalized`, `CapacityReleased`, `DispatchQueued`, `DeliverySucceeded`, or `DeliveryFailed`. |
| `OccurredAt` | Event time. |
| `Reason` / `LastError` | Safe operator explanation; no payment credentials or private payloads. |
| `ActorCustomerId` | Nullable actor where relevant. |

Index: parent, occurrence time, creation time.

## MarketplaceBookingFailureDelivery

Idempotent recipient/channel delivery state; it is not a general notification platform.

| Field | Purpose / validation |
| --- | --- |
| `MarketplaceBookingFailureId` | Parent aggregate. |
| `RecipientCustomerId` / `RecipientEmail` | Recipient identity/snapshot; email must be verified/allowed before creation. |
| `Audience` | `Customer`, `SpacesStakeholder`, or `HostStakeholder`. |
| `Channel` | `InApplication` or `Email`. |
| `Status` | `Pending`, `Sent`, `Skipped`, or `Failed`. |
| `AttemptCount`, `LastAttemptAt`, `LastError`, `SentAt` | Retry and support audit. |

Unique index: failure + recipient identity/email + channel. A delivery record is created only after the parent failure is finalized.

## Relationships and Lifecycle

```text
MarketplaceBookingFailure 1 ── * MarketplaceBookingFailureEvent
MarketplaceBookingFailure 1 ── * MarketplaceBookingFailureDelivery
MarketplaceBookingFailure ── 0..1 Booking / RecurringBooking / MarketplaceBookingSubscription
```

1. Submission crosses the existing public mutation boundary.
2. Repository allocation either atomically claims the complete required set or returns conflict.
3. Finalizer inserts/loads failure by `FailureKey`, appends finalization/release event, releases capacity when required, creates deduplicated delivery records, and queues dispatch in the same unit of work.
4. Dispatch updates delivery status; retry never creates a second recipient/channel row.
5. One-time payment failure releases the booking capacity but preserves the immutable booking and failure record. A recurring-cycle payment failure releases child bookings in that cycle while preserving subscription configuration. Later availability failure affects one recurring occurrence only.

## Existing Data Reused

- `Booking`, `MarketplaceBooking`, `RecurringBooking`, and `MarketplaceBookingSubscription` remain the lifecycle source.
- `ResourceBookingSlot` remains the allocation projection.
- Replicated `OrganizationMember`/`Customer.Identity` data resolves authorized Spaces and Host owners/administrators plus verified email recipients.
- Existing Kafka/Temporal outbox rows remain the reliable mechanism to begin asynchronous work.
