# Required Days Contract

`requiredDaysPerWeek` remains nullable. For longer-than-weekly cadences, non-null values use complete UTC calendar weeks. Reservations/subscriptions require exactly N bookings; entitlement redemptions allow at most N confirmed redemptions. `availableDays` constrains weekdays and partial boundary weeks are exempt.

Existing GraphQL and protobuf field names remain compatible. Any new API field must be added to its source definition and regenerated; exported schemas and Relay artifacts must not be hand-edited.
