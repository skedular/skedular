# Contract: Organization Event Projection for Spaces Trial

## Source

`api-definitions/events/skedular/organization_v1_value.proto`

The existing Organization upsert/offering event remains the cross-domain carrier. Add backward-compatible optional fields; do not introduce a separate topic.

## Offering payload additions

```protobuf
message Offering {
  // Existing fields retain their field numbers.
  optional google.protobuf.Timestamp spacesTrialStartedAt = <new>;
  optional google.protobuf.Timestamp spacesTrialEndsAt = <new>;
  bool spacesProductEnabled = <new>;
  optional google.protobuf.Timestamp spacesNextBillingAt = <new>;
}
```

Exact field numbers are allocated after the existing highest field and are never reused.

## Producer rules

- Organization computes `spacesTrialEndsAt = spacesTrialStartedAt + 14 days`.
- `spacesProductEnabled` is true only for Marketplace/Spaces offering codes.
- Dates are populated for Free and paid Spaces plans so future downgrade remains anchored.
- Teams/private offerings omit trial dates and set `spacesProductEnabled = false`.
- The producer publishes after creation, first enablement, plan transition, cancellation, renewal, or support correction through the existing transactional outbox. There is no startup/customer backfill publication path.

## Consumer rules

- Booking and Location ignore these fields when `spacesProductEnabled = false`.
- Consumers persist dates/plan inputs in their existing replicated offering JSON.
- Consumers derive current status from local time; they do not persist the event-time status as authoritative.
- Existing ordering/idempotency checks remain in effect.
- Older events with absent fields produce `MISSING_STATE` only for a Spaces organization and trigger operator logging/recovery, not automatic trial reset.

## Generation

```bash
api-definitions/events/generate.sh
```

Generated protobuf classes remain under build output and are not committed.
