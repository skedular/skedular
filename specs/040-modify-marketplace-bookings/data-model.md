# Data Model: Modify Marketplace Bookings

## Marketplace booking (existing, extended exposure)

| Field | Rule |
|---|---|
| `Id` / `EntityFrameworkVersion` | Command targets one booking and rejects stale submissions. |
| `From` / `Until` | Proposed values must be future, single-day, valid for the original purchased offer, and available. |
| Assigned resources | Explicit selection is optional; selected resources must be eligible and not exceed `quantity × resources-per-offer`. |
| Payment status | Only confirmed or no-payment-required bookings can change. |
| Subscription relation | A modified occurrence must stay in its current cycle and become an occurrence override. |

## Marketplace booking modification (new aggregate)

One immutable record per successful command.

| Field | Purpose |
|---|---|
| `Id`, `BookingId`, `OccurredAt` | Identity and timeline ordering. |
| `ActorCustomerId`, `ActorKind` | Identifies self-service versus organization-on-behalf action. |
| `Reason` | Required for an operator; optional for customer. |
| `OriginalFrom`, `OriginalUntil`, `ResultFrom`, `ResultUntil` | Auditable schedule transition. |
| Original/result resource ids | Auditable fulfillment transition. |
| `SubscriptionOccurrenceOverride` | Marks that the modification is an individual subscription exception. |

Relationships: one Booking has many modification records; a modification has many notification deliveries.

## Marketplace booking modification notification delivery (new aggregate)

| Field | Purpose |
|---|---|
| `Id`, `ModificationId`, `RecipientCustomerId` | Idempotent recipient scope. |
| `Status`, `AttemptCount`, `LastAttemptAt`, `SentAt`, `LastError` | Durable delivery/recovery lifecycle. |
| Delivery key | Prevents duplicate notification on retries. |

State transitions: `Pending → Sending → Sent`; transient failure returns to `Pending`; exhausted/ambiguous delivery becomes `RecoveryRequired`. A delivery failure never reverses the persisted booking modification.

## Validation invariants

1. Commercial fields (product/version/pricing/quantity/payment) are read from the persisted booking and never edited by this command.
2. The complete replacement is committed or neither schedule nor resources change.
3. Operator authorization is product-owner authority; customer authorization is persisted booking involvement.
4. A subscription override does not mutate parent recurring days or resource preferences.
