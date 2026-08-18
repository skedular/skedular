# Operations and Metrics Contract

The implementation must emit structured logs for purchase creation, payment confirmation, entitlement grant, renewal
creation and failure, expiry, refund projection, credit consumption/restoration/forfeiture, and operator actions.

Operational dashboards should be able to distinguish pending, confirmed, rejected, expired, renewal-failed, refunded,
and settlement-pending states, and correlate each event by purchase, entitlement, booking, organization, customer, and
idempotency/reference key.
