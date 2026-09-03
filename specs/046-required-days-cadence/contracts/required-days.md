# Required Days Contract

`requiredDaysPerWeek` remains nullable. For weekly and longer purchase cadences, non-null values use complete UTC calendar weeks and `[start, end)` period bounds. Reservations/subscriptions require exactly N booking occurrences; cadence-free entitlement redemptions, within their validity period, allow at most N successful redemptions. Canceled, permanently failed, or refunded redemptions release allowance, and retries are idempotent. `availableDays` constrains scheduled weekdays; entitlement redemption does not ask for weekday selection. No booking or location timezone is stored or consulted.

Existing GraphQL and protobuf field names remain compatible. Any new API field must be added to its source definition and regenerated; exported schemas and Relay artifacts must not be hand-edited.
