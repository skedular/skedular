# GraphQL Contract: Automatic Marketplace Billing

Expose a shared read model for reservation subscriptions, entitlement purchases, and future AutoRenew purchase types:

- `autoRenew`
- `automaticPaymentStatus` (not-ready, pending, active, action-required, past-due, cancelled, disconnected)
- `stripeSubscriptionStatus`
- `nextBillingAt` / `paidThroughAt`
- `recoveryAction` (non-secret URL/action state and expiry)
- event-backed `paymentAndRenewalHistory`

Mutations for AutoRenew/cancellation/recovery return the stable purchase ID and the rendered automatic-payment fields. Relay updates use returned fields plus a targeted refetch/declarative connection update; never reload the page. No card details, Stripe client secrets, or Customer-profile payment-method status are exposed.
