# Webhook Events Contract

Payment webhooks must resolve standalone entitlement purchases and renewal purchases by their purchase reference and
must verify the selected pricing, amount, currency, and organization before changing local payment state.

Repeated webhook delivery is idempotent. Confirmed payment grants at most one entitlement cycle and one grant ledger
entry. Pending, rejected, canceled, or expired payment events never grant credits or create bookings.
