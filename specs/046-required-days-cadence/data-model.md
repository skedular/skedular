# Data Model

Retain nullable `ProductPricing.RequiredDaysPerWeek`, `AvailableDays`, `PurchaseCadence`, `BookingCadence`, and `FulfillmentType`; no ProductPricing migration is required.

Use a shared UTC week key. A week is complete only when the offer period covers the full Monday-Sunday UTC window; boundary partial weeks are exempt.

Count confirmed entitlement redemption bookings by entitlement, pricing rule, and UTC week through a Booking-owned repository query or durable usage record. Credit claiming and the count check must be concurrency-safe.

Rules: reservations/subscriptions require exactly N booking occurrences; entitlements allow at most N confirmed redemptions; empty `availableDays` means all seven days; N is 1 through min(7, available-day count). All existing booking cadences are eligible, including hourly and other time-based cadences.
