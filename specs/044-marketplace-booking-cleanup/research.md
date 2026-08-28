# Research: Reliable Marketplace Booking Cleanup

## Decisions

- **Separate local release from external cleanup.** Existing recurring release invokes accounting before deleting generated bookings; local transaction must commit first. Retrying the combined activity would preserve the provider dependency.
- **Use a shared cleanup Temporal contract.** Existing payment and invoice workflows remain responsible for detecting terminal failure, but they delegate cleanup to one idempotent workflow/activity contract. This avoids replacing all workflow routes while preventing duplicated cleanup ordering and retry behavior.
- **Reuse existing failure and reconciliation infrastructure.** `MarketplaceBookingFailure`, failure events/delivery records, repository filters, and `MarketplaceRefundReconciliationHostedService` provide the established seams. Extend them before creating new storage.
- **Resolve effective payment ownership.** A booking may inherit state from a linked subscription or billed owner; booking-local status alone is insufficient. Record the resolved owner for repeatability and audit.
- **Make missing Stripe setup explicit.** Null product, pricing, customer, or session responses must enter durable failure and cleanup rather than silently returning.
- **Use five local retries plus immediate reconciliation.** Local-release activities use at most five delayed/exponential-backoff Temporal retries. Exhaustion creates an immediate reconciliation candidate and recurring reconciliation repairs worker loss and historical orphans. Provider/accounting cleanup is independently retryable after local release and does not share the local-release limit.
- **Treat durable failure records as cleanup evidence.** A rejected or expired effective payment is eligible, as is an invoice-generation, Xero, or Stripe setup terminal failure record when no payment record was created. Pending, confirmed, and no-payment-required cases remain ineligible, and confirmed entitlement always excludes cleanup.
- **Publish truthful UI state.** “Resources released” is valid only after local commit; provider follow-up is represented separately and Relay updates returned state without browser reload.
