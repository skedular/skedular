# Data Model

Retain nullable `ProductPricing.RequiredDaysPerWeek`, `AvailableDays`, `PurchaseCadence`, and `FulfillmentType`; booking duration follows Spec 047 duration limits and no ProductPricing migration is required.

Use a shared UTC week key. Booking and entitlement queries interpret timestamps in UTC and do not store or consult a booking/location timezone. A week is complete only when the offer period interval `[start, end)` covers the full Monday-Sunday UTC window; boundary partial weeks are exempt.

Reuse the existing Booking-owned credit-ledger entries as the durable entitlement redemption history. The repository counts consumed entries by entitlement and UTC week and excludes entries with a release or forfeiture transition; no new usage table or migration is required. The serializable repository transaction protects the weekly aggregate check and consumption. Canceled, permanently failed, or refunded redemptions release the allowance; retries are idempotent.

Rules: reservations/subscriptions require exactly N booking occurrences; entitlements allow at most N successful redemptions; empty `availableDays` means all seven days; N is 1 through min(7, available-day count). Supported purchase cadences are weekly and longer; daily hides the setting. Entitlements are cadence-free and use validity periods.
